using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using OrderFeedbackManagementSystemAPI.Application.Interfaces;
using OrderFeedbackManagementSystemAPI.Domain.Entities;

namespace OrderFeedbackManagementSystemAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly IOrderService _orderService;

        public ReviewsController(IReviewService reviewService, IOrderService orderService)
        {
            _reviewService = reviewService;
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<ActionResult<Review>> CreateReview([FromBody] CreateReviewRequest request)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var review = await _reviewService.CreateReviewAsync(userId, request.OrderId, request.Rating, request.Comment);
                return CreatedAtAction(nameof(GetOrderReviews), new { orderId = request.OrderId }, review);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Review>> UpdateReview(int id, [FromBody] UpdateReviewRequest request)
        {
            try
            {
                var review = await _reviewService.UpdateReviewAsync(id, request.Rating, request.Comment);
                return Ok(review);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("order/{orderId}")]
        public async Task<ActionResult<IEnumerable<Review>>> GetOrderReviews(int orderId)
        {
            var reviews = await _reviewService.GetOrderReviewsAsync(orderId);
            return Ok(reviews);
        }

        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<Review>>> GetFilteredReviews([FromQuery] int rating)
        {
            try
            {
                var reviews = await _reviewService.GetFilteredReviewsAsync(rating);
                return Ok(reviews);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("product/{productId}/rating")]
        public async Task<ActionResult<double>> GetProductAverageRating(int productId)
        {
            var rating = await _reviewService.GetProductAverageRatingAsync(productId);
            return Ok(rating);
        }
    }
}
