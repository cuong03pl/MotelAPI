using Microsoft.AspNetCore.Mvc;
using Motel.DTO;
using Motel.Interfaces;
using Motel.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Motel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        public IReviewRepository _reviewRepository {  get; set; }
        public ReviewsController(IReviewRepository reviewRepository) { 
            _reviewRepository = reviewRepository;
        }
        // GET: api/<ReviewsController>
        [HttpGet]
        public async Task<object> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 1)
        {
            return await _reviewRepository.GetReviews(page, pageSize);
        }

        // GET api/<ReviewsController>/5
        [HttpGet("{id}")]
        public ReviewDTO Get(string id)
        {
            return _reviewRepository.GetReview(id);
        }
        [HttpGet("GetReviewsByPost/{id}")]
        public List<ReviewDTO> GetReviewsByPost(string id)
        {
            return _reviewRepository.GetReviewsByPost(id);
        }

        // POST api/<ReviewsController>
        [HttpPost]
        public void Post([FromBody] Reviews review)
        {
            _reviewRepository.CreateReview(review);
        }

        // PUT api/<ReviewsController>/5
        [HttpPut("{id}")]
        public void Put(string id, [FromForm] Reviews review)
        {
            _reviewRepository.UpdateReview(id, review);
        }

        // DELETE api/<ReviewsController>/5
        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            _reviewRepository.DeleteReview(id);
        }
    }
}
