using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderFeedbackManagementSystemAPI.Application.Interfaces;
using OrderFeedbackManagementSystemAPI.Domain.Entities;
using OrderFeedbackManagementSystemAPI.Models.Requests;
using Swashbuckle.AspNetCore.Annotations;

namespace OrderFeedbackManagementSystemAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ILogger<CartController> _logger;

        public CartController(ICartService cartService, ILogger<CartController> logger)
        {
            _cartService = cartService;
            _logger = logger;
        }

        /// <summary>
        /// Додавання товар в кошик
        /// </summary>
        /// <param name="request">Запит на додавання товару</param>
        /// <returns>Повідомлення про успішне додавання</returns>
        [HttpPost("items")]
        [SwaggerOperation(Summary = "Додає товар в кошик", Description = "Цей метод дозволяє користувачу додавати товар в кошик.")]
        [SwaggerResponse(200, "Товар успішно додано в кошик")]
        [SwaggerResponse(400, "Помилка при додаванні товару в кошик")]
        [SwaggerResponse(500, "Неочікувана помилка")]
        public async Task<ActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            try
            {
                _logger.LogInformation($"Отримано запит на додавання в кошик: ProductId={request.ProductId}, Quantity={request.Quantity}");

                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                _logger.LogInformation($"UserId: {userId}");

                await _cartService.AddToCartAsync(userId, request.ProductId, request.Quantity);
                return Ok(new { message = "Товар успішно додано в кошик" });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError($"Помилка при додаванні в кошик: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Неочікувана помилка: {ex.Message}");
                return StatusCode(500, new { message = "Сталася помилка при додаванні товару в кошик" });
            }
        }

        /// <summary>
        /// Отримання кошик користувача
        /// </summary>
        /// <returns>Кошик користувача</returns>
        [HttpGet]
        [SwaggerOperation(Summary = "Отримує кошик користувача", Description = "Цей метод повертає всі товари, які є в кошику користувача.")]
        [SwaggerResponse(200, "Кошик успішно отримано")]
        public async Task<ActionResult<Cart>> GetCart()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var cart = await _cartService.GetCartAsync(userId);
            return Ok(cart);
        }

        /// <summary>
        /// Оновлення кількості товару в кошику
        /// </summary>
        /// <param name="productId">Ідентифікатор товару</param>
        /// <param name="request">Запит з новою кількістю товару</param>
        /// <returns>Оновлений кошик</returns>
        [HttpPut("items/{productId}")]
        [SwaggerOperation(Summary = "Оновлює кількість товару в кошику", Description = "Цей метод дозволяє змінити кількість товару в кошику.")]
        [SwaggerResponse(200, "Кількість товару в кошику оновлено")]
        [SwaggerResponse(400, "Невірне значення кількості товару")]
        public async Task<ActionResult<Cart>> UpdateCartItemQuantity(int productId, [FromBody] UpdateCartItemQuantityRequest request)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var updatedCart = await _cartService.UpdateCartItemQuantityAsync(userId, productId, request.Quantity);
                return Ok(updatedCart);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Видалення товару з кошика
        /// </summary>
        /// <param name="productId">Ідентифікатор товару</param>
        /// <returns>Статус видалення товару</returns>
        [HttpDelete("items/{productId}")]
        [SwaggerOperation(Summary = "Видаляє товар з кошика", Description = "Цей метод дозволяє користувачу видалити товар з кошика.")]
        [SwaggerResponse(200, "Товар успішно видалено з кошика")]
        [SwaggerResponse(400, "Помилка при видаленні товару з кошика")]
        public async Task<ActionResult> RemoveFromCart(int productId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            await _cartService.RemoveFromCartAsync(userId, productId);
            return Ok();
        }
        
        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            await _cartService.ClearCartAsync(userId);
            return Ok(new { message = "Кошик успішно очищено." });
        }
    }
}
