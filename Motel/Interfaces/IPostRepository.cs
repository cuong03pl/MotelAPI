using Microsoft.AspNetCore.Mvc;
using Motel.DTO;
using Motel.Models;

namespace Motel.Interfaces
{
    public interface IPostRepository
    {
         Task<object> GetPosts(int page, int pageSize, decimal? minPrice, decimal? maxPrice, double? minArea, double? maxArea, string categoryId, string provinceSlug, string districtSlug);
        Task<object> GetApprovedPosts(int page, int pageSize, decimal? minPrice, decimal? maxPrice, double? minArea, double? maxArea, string categoryId, string provinceSlug, string districtSlug);
        public Task CreatePost(Posts post, List<IFormFile> imageFiles);
        public void UpdatePost(string id, Posts post);
        public void DeletePost(string id );
        public PostsDTO GetPost(string slug);
        public long GetCount();
        public bool Browse(string id);
        public List<Posts> SearchByLocation(Location location);
        List<Posts> GetLatestPosts(int count = 5);
        Task<object> GetPostsByCategory(string slug, decimal? minPrice, decimal? maxPrice, double? minArea, double? maxArea, int page, int pageSize );
        Task<object> GetPostsByProvinceSlug(string id, int page, int pageSize);
        Task<List<Location>> GetLocations();
    }
}
