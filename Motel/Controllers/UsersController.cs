using Microsoft.AspNetCore.Mvc;
using Motel.Interfaces;
using Motel.Models;

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
        public IEnumerable<ApplicationUser> Get()
        {
           return _userRepository.GetUsers();
        }

        // GET api/<UsersController>/5
        [HttpGet("{id}")]
        public ApplicationUser Get(Guid id)
        {
            return _userRepository.GetUser(id);
        }

        

        [HttpPut("{id}")]
        public void Put(Guid id, [FromForm] ApplicationUser user)
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
    }
}
