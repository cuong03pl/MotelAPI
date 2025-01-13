using MongoDB.Bson;
using MongoDB.Driver;
using Motel.Interfaces;
using Motel.Models;
using Motel.Services;

namespace Motel.Repository
{
    public class PostRepository : IPostRepository
    {
        public MotelService _motelService { get; set; }
        public PostRepository(MotelService motelService) { 
            _motelService = motelService;
        }
        public List<Posts> GetPosts()
        {
            return _motelService.GetPostCollection().Find(post => true).ToList();
        }

        public void CreatePost(Posts post)
        {
           var posts = _motelService.GetPostCollection();
            posts.InsertOne(post);
        }

        public void UpdatePost(string id , Posts post)
        {
            _motelService.GetPostCollection().ReplaceOne(post => post.Id == id, post);
        }

        public void DeletePost(string id)
        {
           _motelService.GetPostCollection().DeleteOne(post => post.Id == id);
        }

        public Posts GetPost(string id)
        {
            var posts = _motelService.GetPostCollection();
            return posts.Find<Posts>(p => p.Id == id).FirstOrDefault();
        }

        public long GetCount()
        {
            return _motelService.GetPostCollection().CountDocuments(FilterDefinition<Posts>.Empty);
        }

        public bool Browse(string id)
        {
            var filter = Builders<Posts>.Filter.Eq(p => p.Id, id);
            var current_status = _motelService.GetPostCollection().Find(filter).Project(p => p.Is_Browse).FirstOrDefault();
            Console.WriteLine("status: " + current_status);
           var new_sattus = (current_status == 1 ? 0 : 1);
            var update = Builders<Posts>.Update.Set("Is_Browse", new_sattus);
            var result = _motelService.GetPostCollection().UpdateOne(filter, update);
            return result.ModifiedCount > 0;
        }

        public List<Posts> SearchByLocation(Location location)
        {
            return _motelService.GetPostCollection().
                Find(p => p.Location.Province == location.Province && p.Location.District == location.District).ToList();

        }

        public List<Posts> GetFiltered(decimal? minPrice, decimal? maxPrice, double? minArea, double? maxArea)
        {
            var filterBuilder = Builders<Posts>.Filter;
            var filters = new List<FilterDefinition<Posts>>();
            if (minArea.HasValue) filters.Add(filterBuilder.Gte(p => p.Area, minArea.Value));
            if (maxArea.HasValue) filters.Add(filterBuilder.Lte(p => p.Area, maxArea.Value));
            if (minPrice.HasValue) filters.Add(filterBuilder.Gte(p => p.Price, minPrice.Value));
            if (maxPrice.HasValue) filters.Add(filterBuilder.Lte(p => p.Price, maxPrice.Value));
            var filter = filters.Any() ? filterBuilder.And(filters) : filterBuilder.Empty;
            

            return _motelService.GetPostCollection().Find(filter).ToList();
        }
    }
}
