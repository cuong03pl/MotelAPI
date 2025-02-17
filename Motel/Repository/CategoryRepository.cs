using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using Motel.Helpers;
using Motel.Interfaces;
using Motel.Models;
using Motel.Services;

namespace Motel.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        public MotelService _motelService { get; set; }
        private readonly IWebHostEnvironment _evn;
        private readonly GenerateSlug _generateSlug;
        public CategoryRepository(MotelService motelService, IWebHostEnvironment evn, GenerateSlug generateSlug)
        {
            _motelService = motelService;
            _evn = evn;
            _generateSlug = generateSlug;
        }
        public void CreateCategory(Categories category)
        {
            try
            {
                var categories = _motelService.GetCategoryCollection();
                category.Slug = _generateSlug.CreateSlug(category.Name);
                categories.InsertOne(category);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating category: {ex.Message}");
            }
        }

        public void DeleteCategory(string id)
        {
            _motelService.GetCategoryCollection().DeleteOne(category => category.Id == id);
        }

        public List<Categories> GetCategories()
        {
            return _motelService.GetCategoryCollection().Find(cate => true).ToList();
        }

        public Categories GetCategory(string slug)
        {
            return _motelService.GetCategoryCollection().Find(cate => cate.Slug == slug).FirstOrDefault();
        }

        public long GetCount()
        {
            return _motelService.GetCategoryCollection().CountDocuments(FilterDefinition<Categories>.Empty);
        }

        public void UpdateCategory(string id, Categories category)
        {
           var current_category = _motelService.GetCategoryCollection().Find(c => c.Id == id).FirstOrDefault();
            if (current_category.Slug != null)
            {
                category.Slug = current_category.Slug;
            }
            else category.Slug = _generateSlug.CreateSlug(category.Name);
            _motelService.GetCategoryCollection().ReplaceOne(category => category.Id == id, category);
        }
    }
}
