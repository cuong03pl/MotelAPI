using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using Motel.Helpers;
using Motel.Interfaces;
using Motel.Models;
using Motel.Services;

namespace Motel.Repository
{
    public class NewsRepository : INewsRepository
    {
        public MotelService _motelService { get; set; }
        private readonly GenerateSlug _generateSlug;
        public NewsRepository(MotelService motelService, GenerateSlug generateSlug)
        {
            _motelService = motelService;
            _generateSlug = generateSlug;
        }
        public void CreateNews(News news)
        {
            try
            {
                var newsAll = _motelService.GetNewsCollection();
                news.Slug = _generateSlug.CreateSlug(news.Title);
                news.CreateAt = DateTime.Now;
                newsAll.InsertOne(news);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating news: {ex.Message}");
            }
        }

        public void DeleteNews(string id)
        {
            _motelService.GetNewsCollection().DeleteOne(news => news.Id == id);
        }

        public long GetCount()
        {
            throw new NotImplementedException();
        }

        public List<News> GetNews(int page, int pageSize)
        {
           return _motelService.GetNewsCollection().Find(_ => true).Skip((page - 1) * pageSize).Limit(pageSize).ToList();
        }

        public News GetNews(string slug)
        {
            return _motelService.GetNewsCollection().Find(news => news.Slug == slug).FirstOrDefault();
        }

        public void UpdateNews(string id, News news)
        {
            var updateDefinition = Builders<News>.Update
                .Set(n => n.Title, news.Title)
                .Set(n => n.Description, news.Description)
                .Set(n => n.UpdateAt, DateTime.Now);


            _motelService.GetNewsCollection().UpdateOne(n => n.Id == id, updateDefinition);
        }

    }
}
