using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using OrderFeedbackManagementSystemAPI.Application.Interfaces;
using OrderFeedbackManagementSystemAPI.Domain.Entities;
using OrderFeedbackManagementSystemAPI.Models.Requests;

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

        /// <summary>
        /// Створення нового відгуку
        /// </summary>
        /// <param name="request">Дані відгуку</param>
        /// <returns>Створений відгук</returns>
        /// <response code="201">Відгук успішно створено</response>
        /// <response code="400">Помилка валідації</response>
        /// <response code="401">Користувач не авторизований</response>
        /// <response code="403">Доступ заборонено</response>
        [HttpPost]
        [ProducesResponseType(typeof(Review), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
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

        /// <summary>
        /// Оновлення існуючого відгуку
        /// </summary>
        /// <param name="id">Ідентифікатор відгуку</param>
        /// <param name="request">Оновлені дані відгуку</param>
        /// <returns>Оновлений відгук</returns>
        /// <response code="200">Відгук успішно оновлено</response>
        /// <response code="400">Помилка валідації</response>
        /// <response code="401">Користувач не авторизований</response>
        /// <response code="404">Відгук не знайдено</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Review), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Отримання всіх відгуків для замовлення
        /// </summary>
        /// <param name="orderId">Ідентифікатор замовлення</param>
        /// <returns>Список відгуків</returns>
        /// <response code="200">Список відгуків успішно отримано</response>
        /// <response code="401">Користувач не авторизований</response>
        [HttpGet("order/{orderId}")]
        [ProducesResponseType(typeof(IEnumerable<Review>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<Review>>> GetOrderReviews(int orderId)
        {
            var reviews = await _reviewService.GetOrderReviewsAsync(orderId);
            return Ok(reviews);
        }

        /// <summary>
        /// Отримання відгуків за рейтингом
        /// </summary>
        /// <param name="rating">Рейтинг для фільтрації</param>
        /// <returns>Відфільтрований список відгуків</returns>
        /// <response code="200">Список відгуків успішно отримано</response>
        /// <response code="400">Невірний рейтинг</response>
        /// <response code="401">Користувач не авторизований</response>
        [HttpGet("filter")]
        [ProducesResponseType(typeof(IEnumerable<Review>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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

        /// <summary>
        /// Отримання середньої оцінки для конкретного товару
        /// </summary>
        /// <param name="productId">ID товару</param>
        /// <response code="200">Середню оцінку товару отримано</response>
        [HttpGet("product/{productId}/rating")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<double>> GetProductAverageRating(int productId)
        {
            var rating = await _reviewService.GetProductAverageRatingAsync(productId);
            return Ok(rating);
        }
    }
}
