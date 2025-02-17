using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Motel.DTO;
using Motel.Interfaces;
using Motel.Models;
using Motel.Repository;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Motel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        public IUserRepository _userRepository { get; set; }
        public UsersController(IUserRepository userRepository) { 
            _userRepository = userRepository;
        }
        // GET: api/<UsersController>
        [HttpGet]
        public async Task<object> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 1)
        {
           return await _userRepository.GetUsers(page, pageSize);
        }

        // GET api/<UsersController>/5
        [HttpGet("{id}")]
        public ApplicationUser Get(Guid id)
        {
            return _userRepository.GetUser(id);
        }

        [HttpGet("countPost/{id}")]
        public long CountPost(Guid id)
        {
            return _userRepository.CountPost(id);
        }
        [HttpPut("{id}")]
        public void Put(Guid id, [FromBody] ApplicationUser user)
        {
            _userRepository.UpdateUser(id, user);
        }

        // DELETE api/<UsersController>/5
        [HttpDelete("{id}")]
        public void Delete(Guid id)
        {
            _userRepository.DeleteUser(id);
        }

        [HttpPut("block")]
        public void Block(Guid id, bool is_block)
        {
            _userRepository.BlockUser(id, is_block );
        }

        [HttpPost("AddFavoritePost")]
        public async Task<ActionResult> AddFavoritePost([FromQuery] Guid userId, [FromQuery] string postId)
        {
           if(await  _userRepository.AddFavoritePost(userId, postId))
            {
                return Ok("okk");
            }
           else { return BadRequest(); }
        }
        [HttpGet("CheckFavorite")]
        public  ActionResult CheckFavorite([FromQuery]  Guid userId, [FromQuery] string postId)
        {
            if ( _userRepository.CheckFavorite(userId, postId))
            {
                return Ok(true);
            }
            else { return Ok(false); }
        }
        [HttpGet("GetUserFavorite/{id}")]
        public async Task<object> GetUserFavorite(Guid id)
        {
            return  await _userRepository.GetUserFavorite(id);
        }

        [HttpGet("GetUserPosts/{id}")]
        public async Task<object> GetUserPosts(Guid id)
        {
            return await _userRepository.GetUserPosts(id);
        }

        [HttpGet("GetCount")]
        public long GetCount()
        {
            return  _userRepository.GetCount();
        }

        [HttpGet("GetPostCountsByMonth")]
        public async Task<List<PostCountByMonthDTO>> GetPostCountsByMonth()
        {
            return await _userRepository.GetPostCountsByMonth(2025);
        }
    }
}
