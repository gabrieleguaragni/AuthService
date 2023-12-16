using AuthService.Business.Abstractions.Services;
using AuthService.Shared.DTO.Request;
using AuthService.Shared.DTO.Response;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ITokenService _tokenService;
        private IValidator<LoginRequest> _loginRequestValidator;
        private IValidator<RegisterRequest> _registerRequestValidator;
        private IValidator<IFormFile> _imageValidator;
        public AuthController(
            IAuthService authService,
            ITokenService tokenService,
            IValidator<LoginRequest> loginRequestValidator,
            IValidator<RegisterRequest> registerRequestValidator,
            IValidator<IFormFile> imageValidator
            )
        {
            _authService = authService;
            _tokenService = tokenService;
            _loginRequestValidator = loginRequestValidator;
            _registerRequestValidator = registerRequestValidator;
            _imageValidator = imageValidator;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var validationResult = await _loginRequestValidator.ValidateAsync(loginRequest);
            if (!validationResult.IsValid)
            {
                return StatusCode(400, new { message = validationResult.Errors.First().ErrorMessage });
            }

            LoginResponse loggedInUser = await _authService.Login(loginRequest.Email, loginRequest.Password);
            return Ok(loggedInUser);
        }

        [Authorize]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            RefreshTokenResponse refreshTokenResponse = await _authService.RefreshToken(User.Claims);
            return Ok(refreshTokenResponse);
        }

        [Authorize]
        [HttpGet("validate-token")]
        public async Task<IActionResult> ValidateToken()
        {
            ValidateTokenResponse validateTokenResponse = await _tokenService.ValidateToken(User.Claims);
            return Ok(validateTokenResponse);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            var validationResult = await _registerRequestValidator.ValidateAsync(registerRequest);
            if (!validationResult.IsValid)
            {
                return StatusCode(400, new { message = validationResult.Errors.First().ErrorMessage });
            }

            await _authService.Register(
                    username: registerRequest.Username,
                    email: registerRequest.Email,
                    password: registerRequest.Password
                );

            LoginResponse loggedInUser = await _authService.Login(registerRequest.Email, registerRequest.Password);
            return Ok(loggedInUser);
        }

        [Authorize]
        [HttpPost("update/avatar")]
        public async Task<IActionResult> UpdateAvatar(IFormFile avatar)
        {
            var validationResult = await _imageValidator.ValidateAsync(avatar);
            if (!validationResult.IsValid)
            {
                return StatusCode(400, new { message = validationResult.Errors.First().ErrorMessage });
            }

            ValidateTokenResponse validateTokenResponse = await _tokenService.ValidateToken(User.Claims);
            await _authService.UploadAvatar(validateTokenResponse.IDUser, avatar);

            return Ok(new { message = "Take charge request" });
        }
    }
}