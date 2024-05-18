using AutoMapper;
using Lab2.Dto;
using Lab2.Interfaces;
using Lab2.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lab2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : Controller
    {
        private readonly IReviewInterface _reviewInterface;
        private readonly IReviewerInterface _reviewerInterface;
        private readonly IWareInterface _wareInterface;
        private readonly IMapper _mapper;
        public ReviewController(IReviewerInterface reviewerInterface, IWareInterface wareInterface, IReviewInterface reviewInterface, IMapper mapper)
        {
            _reviewInterface = reviewInterface;
            _wareInterface = wareInterface;
            _reviewerInterface = reviewerInterface;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Category>))]

        public IActionResult GetReviews()
        {
            var reviews = _mapper.Map<List<ReviewDto>>(_reviewInterface.GetReviews());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(reviews);
        }

        [HttpGet("{reviewId}")]
        [ProducesResponseType(200, Type = typeof(Review))]
        [ProducesResponseType(400)]
        public IActionResult GetReview(int reviewId)
        {
            if (!_reviewInterface.ReviewExists(reviewId))
                return NotFound();

            var review = _mapper.Map<ReviewDto>(_reviewInterface.GetReview(reviewId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(review);
        }

        [HttpGet("ware/{wareId}")]
        [ProducesResponseType(200, Type = typeof(Review))]
        [ProducesResponseType(400)]
        public IActionResult GetReviewsForAWare(int wareId)
        {
            if (!_wareInterface.WareExists(wareId))
                return NotFound();

            var review = _mapper.Map<List<ReviewDto>>(_reviewInterface.GetReviewsOfAWare(wareId));

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(review);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateReview([FromQuery] int reviewerId, [FromQuery] int wareId, [FromBody] ReviewDto reviewCreate)
        {
            if (reviewCreate == null || !_reviewerInterface.ReviewerExists(reviewerId) || !_wareInterface.WareExists(wareId))
                return BadRequest(ModelState);

            var review = _reviewInterface.GetReviews()
                .Where(r => r.Title == reviewCreate.Title.TrimEnd().ToUpper() 
                && r.Text == reviewCreate.Text.TrimEnd().ToUpper()).FirstOrDefault();
            if (review != null)
            {
                ModelState.AddModelError("", "Review already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewMap = _mapper.Map<Review>(reviewCreate);

            reviewMap.Ware = _wareInterface.GetWare(wareId);
            reviewMap.Reviewer = _reviewerInterface.GetReviewer(reviewerId);

            if (!_reviewInterface.CreateReview(reviewerId, wareId, reviewMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }

        [HttpPut("{reviewId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateReview(int reviewId, [FromBody] ReviewDto updatedReview)
        {
            if (updatedReview == null)
                return BadRequest(ModelState);

            if (reviewId != updatedReview.Id)
                return BadRequest(ModelState);

            if (!_reviewInterface.ReviewExists(reviewId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewMap = _mapper.Map<Review>(updatedReview);

            if (!_reviewInterface.UpdateReview(reviewMap))
            {
                ModelState.AddModelError("", "Something went wrong while updating");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully updated");
        }

        [HttpDelete("{reviewId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteReview(int reviewId)
        {
            if (!_reviewInterface.ReviewExists(reviewId))
                return NotFound();

            var reviewToDelete = _reviewInterface.GetReview(reviewId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_reviewInterface.DeleteReview(reviewToDelete))
                ModelState.AddModelError("", "Something went wrong deleting review");

            return Ok("Successfully deleted");
        }
    }
}
