using Motel.Models;

namespace Motel.Interfaces
{
    public interface IPostRepository
    {
        public List<Posts> GetPosts();
        public void CreatePost(Posts post);
        public void UpdatePost(string id, Posts post);
        public void DeletePost(string id );
        public Posts GetPost(string id);
        public long GetCount();
        public bool Browse(string id);
        public List<Posts> SearchByLocation(Location location);
        public List<Posts> GetFiltered(decimal? minPrice, decimal? maxPrice, double? minArea, double? maxArea);
    }
}
