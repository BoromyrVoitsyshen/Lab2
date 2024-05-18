using Lab2.Models;

namespace Lab2.Interfaces
{
    public interface IReviewInterface
    {
        ICollection<Review> GetReviews();
        Review GetReview(int reviewId);
        ICollection<Review> GetReviewsOfAWare(int wareid);
        bool ReviewExists(int reviewId);
        bool CreateReview(int reviewerId, int wareId, Review review);
        bool UpdateReview(Review review);
        bool DeleteReview(Review review);
        bool DeleteReviews(List<Review> reviews);
        bool Save();
    }
}
