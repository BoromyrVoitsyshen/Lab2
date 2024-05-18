using Lab2.Data;
using Lab2.Interfaces;
using Lab2.Models;

namespace Lab2.Repository
{
    public class ReviewRepository : IReviewInterface
    {
        private readonly DataContext _context;
        public ReviewRepository(DataContext context)
        {
            _context = context;
        }

        public bool CreateReview(int reviewerId, int wareId, Review review)
        {
            _context.Add(review);

            return Save();
        }

        public bool DeleteReview(Review review)
        {
            _context.Remove(review);

            return Save();
        }

        public bool DeleteReviews(List<Review> reviews)
        {
            _context.RemoveRange(reviews);

            return Save();
        }

        public Review GetReview(int reviewId)
        {
            return _context.Reviews.Where(r => r.Id == reviewId).FirstOrDefault();
        }

        public ICollection<Review> GetReviews()
        {
            return _context.Reviews.OrderBy(r => r.Id).ToList();
        }

        public ICollection<Review> GetReviewsOfAWare(int wareid)
        {
            return _context.Reviews.Where(r => r.Ware.Id == wareid).ToList();
        }

        public bool ReviewExists(int reviewId)
        {
            return _context.Reviews.Any(r => r.Id == reviewId);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateReview(Review review)
        {
            _context.Update(review);

            return Save();
        }
    }
}
