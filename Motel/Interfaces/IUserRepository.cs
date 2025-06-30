using MongoDB.Bson;
using Motel.DTO;
using Motel.Models;

namespace Motel.Interfaces
{
    public interface IUserRepository
    {
          Task<object> GetUsers(int page, int pageSize);
        public ApplicationUser GetUser(Guid id);
        public void UpdateUser(Guid id, ApplicationUser user);
        public void DeleteUser(Guid id);
        public void BlockUser(Guid id, bool is_block);
        long CountPost(Guid id);
        public long GetCount();
        bool ConfirmPhone(Guid id);
        Task<bool> AddFavoritePost(Guid id, string postId);
        bool CheckFavorite(Guid userId, string postId);
        Task<object> GetUserFavorite(Guid userId);
        Task<object> GetUserPosts(Guid userId);
        Task<List<PostCountByMonthDTO>> GetPostCountsByMonth(int year);
        bool VerifyUser(Guid id, bool isVerified);
    }
}
