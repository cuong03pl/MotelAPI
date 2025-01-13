using Microsoft.AspNetCore.Mvc;
using Motel.Interfaces;
using Motel.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Motel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        public IPostRepository _postRepository { get; set; }
        public PostsController(IPostRepository postRepository) {
               _postRepository = postRepository;
        }

        [HttpGet]
        public IEnumerable<Posts> Get()
        {
           return _postRepository.GetPosts();
        }

        [HttpPost("SearchByLocation")]
        public IEnumerable<Posts> SearchByLocation([FromForm] Location location)
        {
            return _postRepository.SearchByLocation(location);
        }

        [HttpGet("{id}")]
        public Posts Get(string id)
        {
            return _postRepository.GetPost(id);
        }

        [HttpPost]
        public void Post(Posts post)
        {
            _postRepository.CreatePost(post);
        }

        [HttpPut("{id}")]
        public void Put(string id, [FromBody] Posts posts)
        {
            _postRepository.UpdatePost(id, posts);
        }

        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            _postRepository.DeletePost(id);
        }


        [HttpGet("GetCount")]
        public long GetCount()
        {
            return _postRepository.GetCount();
        }

        [HttpPut("Browse")]
        public void Browse(string id)
        {
            _postRepository.Browse(id);
        }

        [HttpGet("filter")]
        public IEnumerable<Posts> GetFiltered([FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice,
            [FromQuery] double? minArea,[FromQuery] double? maxArea)
        {
            return _postRepository.GetFiltered(minPrice, maxPrice, minArea, maxArea);
        }
    }
}
