using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderFeedbackManagementSystemAPI.Application.Interfaces;
using OrderFeedbackManagementSystemAPI.Domain.Entities;
using OrderFeedbackManagementSystemAPI.Domain.Interfaces;
using OrderFeedbackManagementSystemAPI.Infrastructure.Data;
using OrderFeedbackManagementSystemAPI.Models.Requests;

namespace OrderFeedbackManagementSystemAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        protected readonly ApplicationDbContext _context;
        private readonly IProductService _productService;
        private readonly IProductRepository _productRepository;

        private readonly IWebHostEnvironment _environment;

        public ProductsController(IProductService productService, ApplicationDbContext context, IWebHostEnvironment environment, IProductRepository productRepository)
        {
            _productService = productService;
            _context = context;
            _environment = environment;
            _productRepository = productRepository;
        }

        /// <summary>
        /// Отримання списку всіх товарів
        /// </summary>
        /// <returns>Список товарів</returns>
        /// <response code="200">Список товарів успішно отримано</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetProducts(
            [FromQuery] string search = "",
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] string sortBy = "name",
            [FromQuery] string sortOrder = "asc",
            [FromQuery] int page = 1,
            [FromQuery] int limit = 100,
            [FromQuery] bool getAll = false)
        {
            try
            {
                var query = _context.Products.AsQueryable();

                // Фільтрація
                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(p => p.Name.Contains(search) || p.Description.Contains(search));
                }
                if (minPrice.HasValue)
                {
                    query = query.Where(p => p.Price >= minPrice.Value);
                }
                if (maxPrice.HasValue)
                {
                    query = query.Where(p => p.Price <= maxPrice.Value);
                }

                // Сортування
                query = sortBy.ToLower() switch
                {
                    "name" => sortOrder.ToLower() == "asc"
                        ? query.OrderBy(p => p.Name)
                        : query.OrderByDescending(p => p.Name),
                    "price" => sortOrder.ToLower() == "asc"
                        ? query.OrderBy(p => p.Price)
                        : query.OrderByDescending(p => p.Price),
                    _ => query.OrderBy(p => p.Name)
                };

                // Повернення всіх товарів або з пагінацією
                if (getAll)
                {
                    var items = await query.ToListAsync();
                    return Ok(new { items });
                }
                else
                {
                    var totalCount = await query.CountAsync();
                    var items = await query
                        .Skip((page - 1) * limit)
                        .Take(limit)
                        .ToListAsync();
                    return Ok(new { items, totalCount });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
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
        public async Task<IActionResult> CreateProduct([FromForm] ProductRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var product = new Product
                {
                    Name = request.Name,
                    Description = request.Description,
                    Price = request.Price,
                    Stock = request.Stock
                };

                if (request.Image != null)
                {
                    string uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "products");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + request.Image.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    Directory.CreateDirectory(uploadsFolder);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await request.Image.CopyToAsync(fileStream);
                    }

                    product.ImagePath = $"/images/products/{uniqueFileName}";
                }

                var createdProduct = await _productService.CreateProductAsync(product);
                return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.Id }, createdProduct);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
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
        public async Task<ActionResult<Product>> UpdateProduct(int id, [FromForm] ProductUpdateRequest request)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(id);
                if (product == null)
                    return NotFound();

                product.Name = request.Name;
                product.Description = request.Description;
                product.Price = decimal.Parse(request.Price);
                product.Stock = int.Parse(request.Stock);

                if (request.Image != null)
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "products");
                    Directory.CreateDirectory(uploadsFolder);

                    if (!string.IsNullOrEmpty(product.ImagePath))
                    {
                        var oldImagePath = Path.Combine(_environment.WebRootPath, product.ImagePath.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + request.Image.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await request.Image.CopyToAsync(fileStream);
                    }

                    product.ImagePath = "/images/products/" + uniqueFileName;
                }

                await _productRepository.UpdateAsync(product);
                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        
        public class ProductUpdateRequest
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string Price { get; set; }
            public string Stock { get; set; }
            public IFormFile? Image { get; set; }
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
