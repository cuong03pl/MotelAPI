using Microsoft.AspNetCore.Mvc;
using Motel.Interfaces;
using Motel.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Motel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        public ICategoryRepository _categoryRepository { get; set; }
        public CategoriesController(ICategoryRepository categoryRepository) {
            _categoryRepository = categoryRepository;
        }
        // GET: api/<CategoriesController>
        [HttpGet]
        public IEnumerable<Categories> Get()
        {
           return _categoryRepository.GetCategories();
        }

        // GET api/<CategoriesController>/5
        [HttpGet("{id}")]
        public Categories Get(string id)
        {
            return _categoryRepository.GetCategory(id);
        }

        // POST api/<CategoriesController>
        [HttpPost]
        public void Post([FromBody] Categories category)
        {
            _categoryRepository.CreateCategory(category);
        }

        // PUT api/<CategoriesController>/5
        [HttpPut("{id}")]
        public void Put(string id, [FromBody] Categories category)
        {
            _categoryRepository.UpdateCategory(id, category);
        }

        // DELETE api/<CategoriesController>/5
        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            _categoryRepository.DeleteCategory(id);
        }
    }
}
