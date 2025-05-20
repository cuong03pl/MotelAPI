using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using MongoDB.Bson;
using MongoDB.Driver;
using Motel.DTO;
using Motel.Interfaces;
using Motel.Models;
using Motel.Services;
using System.Data;

namespace Motel.Repository
{
    public class RoleRepository : IRoleRepository
    {
        public UserManager<ApplicationUser> _userManager;
        public SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration configuration;
        private readonly RoleManager<ApplicationRole> _roleManager;
        public MotelService _motelService { get; set; }
        public RoleRepository(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager,
                IConfiguration configuration, SignInManager<ApplicationUser> signInManager, MotelService motelService
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            this.configuration = configuration;
            _motelService = motelService;
        }
        public int Count()
        {
            return _roleManager.Roles.Count();
        }

        public async Task<bool> CreateRole(string roleName)
        {
            var existingRole = await _motelService.GetRoleCollection().Find(r => r.Name == roleName).FirstOrDefaultAsync();
            if (existingRole != null)
            {
                return false; 
            }
            var newRole = new ApplicationRole
            {
                NormalizedName = roleName.ToUpper(),
                Name = roleName
            };

            await _motelService.GetRoleCollection().InsertOneAsync(newRole);
            return true;
        }


        public async Task<bool> DeleteRole(Guid id)
        {
            var result = await _motelService.GetRoleCollection().DeleteOneAsync(r => r.Id == id);
            return result.DeletedCount > 0;
        }

        public List<ApplicationRole> GetRoles()
        {
            return _roleManager.Roles.ToList();
        }

        public Task<IList<IdentityRole>> GetRolesByUser(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SetRole(Guid userId, List<Guid> rolesId)
        {
            var user = _userManager.Users.Where(u => u.Id == userId).FirstOrDefault();
            if (user == null)
            {
                return false;
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            var allRoles = _roleManager.Roles.ToList();
            var rolesToAdd = new List<string>();
            var rolesToRemove = new List<string>();

            foreach (var role in allRoles)
            {
                if (rolesId.Contains(role.Id))
                {
                    if (!currentRoles.Contains(role.Name))
                    {
                        rolesToAdd.Add(role.Name);
                    }
                }
                else
                {
                    // nếu mà danh sachs roleid mà không có role đang duyệt này thì sẽ xóa
                    // xóa bằng cách kiểm tra nếu tất cả role của user này mà có cái role đang duyệt này thì mới thêm vào role remove 
                    // vì nếu tất cả role của user này mà không có cái role đang duyệt thì sẽ không xóa được
                    if (currentRoles.Contains(role.Name))
                    {
                        rolesToRemove.Add(role.Name);
                    }
                }
            }

            foreach (var roleName in rolesToRemove)
            {
                await _userManager.RemoveFromRoleAsync(user, roleName);
            }

            foreach (var roleName in rolesToAdd)
            {
                await _userManager.AddToRoleAsync(user, roleName);
            }

            return true;
        }

        public async Task<bool> UpdateRole(Guid id, RoleDTO role)
        {
            var updateDefinition = Builders<ApplicationRole>.Update
                 .Set(r => r.Name, role.RoleName);

            var result = await _motelService.GetRoleCollection()
        .UpdateOneAsync(n => n.Id == id, updateDefinition);

            return result.ModifiedCount > 0;
        }

    }
}
