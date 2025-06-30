using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Motel.DTO;
using Motel.Helpers;
using Motel.Interfaces;
using Motel.Models;
using Motel.Repository;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Motel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public UserManager<ApplicationUser> _userManager;
        public SignInManager<ApplicationUser> _signInManager;
        public RandomImage _randomImage;
        public IRoleRepository _roleRepository;
        private readonly IConfiguration _configuration;
        public ILoginHistoryRepository _loginHistoryRepository;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IEmailService _emailService;

        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            RandomImage randomImage, IConfiguration configuration, 
            ILoginHistoryRepository loginHistoryRepository, RoleManager<ApplicationRole> roleManager, IRoleRepository roleRepository,
            IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _randomImage = randomImage;
            _configuration = configuration;
            _loginHistoryRepository = loginHistoryRepository;
            _roleManager = roleManager;
            _roleRepository = roleRepository;
            _emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if user already exists with the same email
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    return BadRequest(new { Message = "Email is already registered" });
                }
                
                var user = new ApplicationUser { 
                    Email = model.Email, 
                    UserName = model.Email, 
                    FullName = model.FullName, 
                    PhoneNumber = model.PhoneNumber, 
                    Avatar = _randomImage.randomImage() 
                };
                
                var result = await _userManager.CreateAsync(user, model.Password);
                var roleExists = await _roleManager.RoleExistsAsync("User");
                if (!roleExists)
                {
                    var role = new ApplicationRole { Name = "User" };
                    await _roleManager.CreateAsync(role);
                }

                await _userManager.AddToRoleAsync(user, "User");
                
                if (result.Succeeded)
                {
                    return Ok(new { Message = "User registered successfully!" });
                }

                return BadRequest(result.Errors);
            }

            return BadRequest("Invalid data.");
        }

        [HttpPost("login")]
        public async Task<string> Login([FromBody] LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);

                
                if (result.Succeeded)
                {

                    var user = await _userManager.FindByEmailAsync(model.Email);
                    var roles = await _userManager.GetRolesAsync(user);
                    var authClaims = new List<Claim>
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                        new Claim(JwtRegisteredClaimNames.Name, user.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    };
                    foreach (var role in roles)
                    {
                        authClaims.Add(new Claim("role", role));
                    }
                    var token = GetToken(authClaims);

                    var record = new LoginHistory()
                    {
                        IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                        UserAgent = Request.Headers["User-Agent"].ToString(),
                        LoginTime = DateTime.Now,
                        UserId = user.Id,
                    };
                    _loginHistoryRepository.CreateLoginHistory(record);
                    // create JWT string
                    return new JwtSecurityTokenHandler().WriteToken(token);
                }

                return null;
                
            }

            return null;
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }

        [HttpGet("GetLoginHistory")]
        public object GetLoginHistory(int page, int pageSize)
        {
            return _loginHistoryRepository.GetLoginHistory(page, pageSize);
        }
        [HttpPost("SetRole/{userId}")]
        public async Task<IActionResult> SetRole(Guid userId, [FromForm] List<Guid> roles)
        {
            if (await _roleRepository.SetRole(userId, roles))
            {
                return Ok("Success");
            }
            else return BadRequest();
        }

        [HttpGet("GetRoles")]
        public async Task<IActionResult> GetRoles()
        {
            return Ok(_roleRepository.GetRoles());
        }

        [HttpDelete("DeleteRole/{id}")]
        public async Task<IActionResult> DeleteRole(Guid id)
        {
            return Ok(await _roleRepository.DeleteRole(id));
        }
        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRole([FromBody] RoleDTO role)
        {
            return Ok(await _roleRepository.CreateRole(role.RoleName));
        }
        [HttpPut("UpdateRole/{id}")]
        public async Task<IActionResult> UpdateRole(Guid id, [FromBody] RoleDTO role)
        {
            return Ok(await _roleRepository.UpdateRole(id, role));
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return Ok(new { Message = "If your email is registered, you will receive a password reset link." });
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(token);
            var resetLink = $"{_configuration["JWT:ValidAudience"]}/reset-password?email={model.Email}&token={encodedToken}";

            var message = $@"
                <h2>Reset Your Password</h2>
                <p>Please click the link below to reset your password:</p>
                <p><a href='{resetLink}'>Reset Password</a></p>
                <p>If you did not request this reset, please ignore this email.</p>";

            await _emailService.SendEmailAsync(model.Email, "Reset Password", message);

            return Ok(new { Message = "If your email is registered, you will receive a password reset link." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest(new { Message = "Invalid request" });
            }

            try
            {
                var decodedToken = WebUtility.UrlDecode(model.Token);
                var result = await _userManager.ResetPasswordAsync(user, decodedToken, model.NewPassword);

                if (result.Succeeded)
                {
                    return Ok(new { Message = "Password has been reset successfully." });
                }

                var errors = result.Errors.Select(e => e.Description).ToList();
                return BadRequest(new { Message = "Password reset failed.", Errors = errors });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Invalid token format.", Error = ex.Message });
            }
        }
    }
}
