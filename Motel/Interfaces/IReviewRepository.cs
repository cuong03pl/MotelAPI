using Motel.DTO;
using Motel.Models;

namespace Motel.Interfaces
{
    public interface IReviewRepository
    {
         Task<object> GetReviews(int page, int pageSize);
       List<ReviewDTO> GetReviewsByPost(string postId);

        public ReviewDTO GetReview(string id);
        public void CreateReview(Reviews report);
        public bool UpdateReview(string id, Reviews report);
        public bool DeleteReview(string id);


    }
}
