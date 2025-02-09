using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderFeedbackManagementSystemAPI.Application.Interfaces;
using OrderFeedbackManagementSystemAPI.Domain.Entities;
using OrderFeedbackManagementSystemAPI.Models.Requests;

namespace OrderFeedbackManagementSystemAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Отримання списку всіх товарів
        /// </summary>
        /// <returns>Список товарів</returns>
        /// <response code="200">Список товарів успішно отримано</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        /// <summary>
        /// Отримання інформації про конкретний товар
        /// </summary>
        /// <param name="id">Ідентифікатор товару</param>
        /// <returns>Інформація про товар</returns>
        /// <response code="200">Товар знайдено</response>
        /// <response code="404">Товар не знайдено</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                return Ok(product);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Створення нового товару
        /// </summary>
        /// <param name="product">Дані нового товару</param>
        /// <returns>Створений товар</returns>
        /// <response code="201">Товар успішно створено</response>
        /// <response code="400">Помилка валідації</response>
        /// <response code="401">Користувач не авторизований</response>
        /// <response code="403">Доступ заборонено</response>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(Product), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
        {
            try
            {
                var createdProduct = await _productService.CreateProductAsync(product);
                return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.Id }, createdProduct);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Оновлення інформації про товар
        /// </summary>
        /// <param name="id">Ідентифікатор товару</param>
        /// <param name="product">Оновлені дані товару</param>
        /// <returns>Оновлений товар</returns>
        /// <response code="200">Товар успішно оновлено</response>
        /// <response code="401">Користувач не авторизований</response>
        /// <response code="403">Доступ заборонено</response>
        /// <response code="404">Товар не знайдено</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Product>> UpdateProduct(int id, [FromBody] Product product)
        {
            try
            {
                var updatedProduct = await _productService.UpdateProductAsync(id, product);
                return Ok(updatedProduct);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Видалення товару
        /// </summary>
        /// <param name="id">Ідентифікатор товару</param>
        /// <response code="204">Товар успішно видалено</response>
        /// <response code="401">Користувач не авторизований</response>
        /// <response code="403">Доступ заборонено</response>
        /// <response code="404">Товар не знайдено</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            try
            {
                await _productService.DeleteProductAsync(id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Оновлення кількості товару на складі
        /// </summary>
        /// <param name="id">Ідентифікатор товару</param>
        /// <param name="request">Дані про кількість</param>
        /// <returns>Оновлений товар</returns>
        /// <response code="200">Кількість успішно оновлено</response>
        /// <response code="400">Помилка валідації</response>
        /// <response code="401">Користувач не авторизований</response>
        /// <response code="403">Доступ заборонено</response>
        /// <response code="404">Товар не знайдено</response>
        [HttpPost("{id}/stock")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Product>> UpdateStock(int id, [FromBody] StockUpdateRequest request)
        {
            try
            {
                var product = await _productService.UpdateProductStockAsync(id, request.Quantity);
                return Ok(product);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
