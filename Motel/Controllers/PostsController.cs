using Microsoft.AspNetCore.Mvc;
using Motel.Interfaces;
using Microsoft.AspNetCore.Http;
using Motel.DTO;
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
        public async Task<object> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 1, [FromQuery] decimal? minPrice = 0, [FromQuery] decimal? maxPrice = null,
            [FromQuery] double? minArea = 0, [FromQuery] double? maxArea = null, [FromQuery] string? categoryId = null,[FromQuery] string? provinceSlug = null, [FromQuery] string? districtSlug = null)
        {
           return await _postRepository.GetPosts(page, pageSize, minPrice, maxPrice, minArea, maxArea, categoryId, provinceSlug, districtSlug);
        }

        [HttpGet("GetApprovedPosts")]
        public async Task<object> GetApprovedPosts([FromQuery] int page = 1, [FromQuery] int pageSize = 1, [FromQuery] decimal? minPrice = 0, [FromQuery] decimal? maxPrice = null,
            [FromQuery] double? minArea = 0, [FromQuery] double? maxArea = null, [FromQuery] string? categoryId = null, [FromQuery] string? provinceSlug = null, [FromQuery] string? districtSlug = null)
        {
            return await _postRepository.GetApprovedPosts(page, pageSize, minPrice, maxPrice, minArea, maxArea, categoryId, provinceSlug, districtSlug);
        }

        [HttpPost("SearchByLocation")]
        public IEnumerable<Posts> SearchByLocation([FromForm] Location location)
        {
            return _postRepository.SearchByLocation(location);
        }

        [HttpGet("{slug}")]
        public PostsDTO Get(string slug)
        {
            return _postRepository.GetPost(slug);
        }

        [HttpGet("GetPostsByCategory/{slug}")]
        public async Task<object> GetPostsByCategory([FromQuery] int page , [FromQuery] int pageSize, string slug, [FromQuery] decimal? minPrice ,[FromQuery] decimal? maxPrice, [FromQuery] double? minArea, [FromQuery]  double? maxArea)
        {
            return await _postRepository.GetPostsByCategory(slug, minPrice, maxPrice, minArea, maxArea, page, pageSize);
        }

        [HttpGet("GetPostsByProvinceSlug/{id}")]
        public async Task<object> GetPostsByProvinceSlug(string id, [FromQuery] int page = 1, [FromQuery] int pageSize = 1)
        {
            return await _postRepository.GetPostsByProvinceSlug(id, page, pageSize);
        }

       
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Post([FromForm] Posts post, [FromForm] List<IFormFile> imageFiles)
        {
            if (imageFiles == null || imageFiles.Count == 0)
            {
                return BadRequest(new { message = "No images uploaded" });
            }

            try
            {
                await _postRepository.CreatePost(post, imageFiles);
                return Ok(new { message = "Post created successfully" }); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal Server Error", error = ex.Message });
            }
        }


        [HttpPut("{id}")]
        public void Put(string id, [FromForm] Posts posts)
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

        [HttpGet("GetLatestPosts")]
        public IEnumerable<Posts> GetLatestPosts()
        {
            return _postRepository.GetLatestPosts(5);
        }
        [HttpPut("Browse")]
        public void Browse(string id)
        {
            _postRepository.Browse(id);
        }

        [HttpGet("GetLocations")]
        public async Task<ActionResult<IEnumerable<Location>>> GetLocations()
        {
            var locations = await _postRepository.GetLocations();

            return Ok(locations);
        }

       

    }
}
