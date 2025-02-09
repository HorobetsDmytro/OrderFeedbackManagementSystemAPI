using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using OrderFeedbackManagementSystemAPI.Application.Interfaces;
using OrderFeedbackManagementSystemAPI.Domain.Entities;
using OrderFeedbackManagementSystemAPI.Domain.Enums;
using OrderFeedbackManagementSystemAPI.Models.Requests;

namespace OrderFeedbackManagementSystemAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Створення нового замовлення
        /// </summary>
        /// <param name="orderItems">Список товарів для замовлення</param>
        /// <returns>Створене замовлення</returns>
        /// <response code="201">Замовлення успішно створено</response>
        /// <response code="400">Помилка валідації або недостатньо товару на складі</response>
        /// <response code="401">Користувач не авторизований</response>
        [HttpPost]
        [ProducesResponseType(typeof(Order), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Order>> CreateOrder([FromBody] List<CreateOrderItem> orderItems)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var items = orderItems.Select(item => new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                }).ToList();

                var order = await _orderService.CreateOrderAsync(userId, items);
                return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Отримання інформації про конкретне замовлення
        /// </summary>
        /// <param name="id">Ідентифікатор замовлення</param>
        /// <returns>Інформація про замовлення</returns>
        /// <response code="200">Замовлення знайдено</response>
        /// <response code="401">Користувач не авторизований</response>
        /// <response code="403">Доступ заборонено</response>
        /// <response code="404">Замовлення не знайдено</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound();

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (order.UserId != userId && !User.IsInRole("Admin"))
                return Forbid();

            return Ok(order);
        }

        /// <summary>
        /// Отримання всіх замовлень користувача
        /// </summary>
        /// <returns>Список замовлень користувача</returns>
        /// <response code="200">Список замовлень</response>
        /// <response code="401">Користувач не авторизований</response>
        [HttpGet("user")]
        [ProducesResponseType(typeof(IEnumerable<Order>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<Order>>> GetUserOrders()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var orders = await _orderService.GetUserOrdersAsync(userId);
            return Ok(orders);
        }

        /// <summary>
        /// Оновлення статусу замовлення
        /// </summary>
        /// <param name="id">Ідентифікатор замовлення</param>
        /// <param name="newStatus">Новий статус замовлення</param>
        /// <returns>Оновлене замовлення</returns>
        /// <response code="200">Статус успішно оновлено</response>
        /// <response code="401">Користувач не авторизований</response>
        /// <response code="403">Доступ заборонено</response>
        /// <response code="404">Замовлення не знайдено</response>
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Order>> UpdateOrderStatus(int id, [FromBody] OrderStatus newStatus)
        {
            try
            {
                var order = await _orderService.UpdateOrderStatusAsync(id, newStatus);
                return Ok(order);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }

}
