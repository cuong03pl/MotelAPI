using Microsoft.AspNetCore.Mvc;
using Motel.Interfaces;
using Motel.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Motel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        public INewsRepository _newsRepository { get; set; }
        public NewsController(INewsRepository newsRepository)
        {
            _newsRepository = newsRepository;
        }
        [HttpGet]
        public IEnumerable<News> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 1)
        {
            return _newsRepository.GetNews(page, pageSize) ;
        }

        [HttpGet("{id}")]
        public News Get(string id)
        {
            return _newsRepository.GetNews(id);
        }

        [HttpPost]
        public void Post([FromBody] News news)
        {
            _newsRepository.CreateNews(news);
        }

        [HttpPut("{id}")]
        public void Put(string id, [FromBody] News news)
        {
            _newsRepository.UpdateNews(id, news);
        }

        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            _newsRepository.DeleteNews(id);
        }
    }
}
