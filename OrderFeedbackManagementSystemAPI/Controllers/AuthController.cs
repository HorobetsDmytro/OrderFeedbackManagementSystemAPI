using Microsoft.AspNetCore.Mvc;
using OrderFeedbackManagementSystemAPI.Application.Interfaces;

namespace OrderFeedbackManagementSystemAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Реєстрація нового користувача
        /// </summary>
        /// <param name="request">Дані для реєстрації користувача</param>
        /// <returns>Токен авторизації та інформація про користувача</returns>
        /// <response code="200">Успішна реєстрація</response>
        /// <response code="400">Помилка валідації або користувач вже існує</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var response = await _authService.RegisterAsync(request);
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Авторизація користувача
        /// </summary>
        /// <param name="request">Дані для входу</param>
        /// <returns>Токен авторизації та інформація про користувача</returns>
        /// <response code="200">Успішна авторизація</response>
        /// <response code="401">Невірні облікові дані</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            try
            {
                var response = await _authService.LoginAsync(request);
                return Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Invalid credentials");
            }
        }
    }
}
