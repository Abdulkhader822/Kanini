using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillAssessmentPortal.DTOs.Auth;
using SkillAssessmentPortal.Repositories.Interfaces;
using SkillAssessmentPortal.Services.Interfaces;
using BCrypt.Net;
using System.Security.Claims;

namespace SkillAssessmentPortal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserRepository userRepository, ITokenService tokenService, ILogger<AuthController> logger)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (string.IsNullOrEmpty(dto.Email))
            {
                return BadRequest(new { error = "Email is required." });
            }
            
            _logger.LogInformation("Login attempt for email: {Email}", dto.Email.Substring(0, Math.Min(3, dto.Email.Length)) + "***");
            
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Login failed - Invalid model state for email: {Email}", dto.Email.Substring(0, Math.Min(3, dto.Email.Length)) + "***");
                return BadRequest(new { error = "Please provide valid email and password." });
            }

            try
            {
                var user = await _userRepository.GetByEmailAsync(dto.Email);
                if (user == null)
                {
                    _logger.LogWarning("Login failed - User not found for email: {Email}", dto.Email.Substring(0, Math.Min(3, dto.Email.Length)) + "***");
                    return BadRequest(new { error = "Invalid email or password" });
                }

                // Enhanced debugging for intermittent login failures
                if (string.IsNullOrEmpty(dto.Password))
                {
                    _logger.LogWarning("Login failed - Password is null or empty for UserId: {UserId}", user.UserId);
                    return BadRequest(new { error = "Invalid email or password" });
                }

                if (string.IsNullOrEmpty(user.PasswordHash))
                {
                    _logger.LogError("Login failed - PasswordHash is null or empty for UserId: {UserId}", user.UserId);
                    return BadRequest(new { error = "Invalid email or password" });
                }

                // Log hash details for debugging (first 10 chars only for security)
                _logger.LogDebug("Password verification attempt for UserId: {UserId}, HashPrefix: {HashPrefix}, PasswordLength: {PasswordLength}", 
                    user.UserId, user.PasswordHash.Substring(0, Math.Min(10, user.PasswordHash.Length)), dto.Password.Length);

                bool isPasswordValid;
                try
                {
                    isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
                }
                catch (Exception bcryptEx)
                {
                    _logger.LogError(bcryptEx, "BCrypt verification failed for UserId: {UserId}", user.UserId);
                    return BadRequest(new { error = "Invalid email or password" });
                }

                if (!isPasswordValid)
                {
                    _logger.LogWarning("Login failed - BCrypt verification failed for UserId: {UserId}", user.UserId);
                    Response.StatusCode = 400;
                    Response.Headers.Add("X-Status-Message", "Login Failed - Invalid Credentials");
                    return BadRequest(new { error = "Invalid email or password" });
                }

                var token = _tokenService.GenerateToken(user);
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogError("Token generation failed for UserId: {UserId}", user.UserId);
                    return StatusCode(500, new { error = "Authentication failed. Please try again." });
                }
                
                _logger.LogInformation("Login successful for UserId: {UserId}, Role: {Role}", user.UserId, user.Role);

                Response.StatusCode = 200;
                Response.Headers.Add("X-Status-Message", "Login Successful");
                return Ok(new 
                {
                    Token = token,
                    Username = user.Name ?? "User",
                    Role = user.Role.ToString(),
                    message = "Login successful"
                });
            }
            catch (Exception ex)
            {
                var emailMask = !string.IsNullOrEmpty(dto.Email) ? dto.Email.Substring(0, Math.Min(3, dto.Email.Length)) + "***" : "unknown";
                _logger.LogError(ex, "Error during login process for email: {Email}", emailMask);
                return StatusCode(500, new { error = "Internal server error. Please try again later." });
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            _logger.LogInformation("User logout requested");
            return Ok(new { message = "Logged out successfully" });
        }

        //[HttpGet("test")]
        //[Authorize]
        //public IActionResult TestAuth()
        //{
        //    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    var userName = User.FindFirst(ClaimTypes.Name)?.Value;
        //    var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
        //    return Ok(new { 
        //        message = "Authentication successful", 
        //        userId, 
        //        userName, 
        //        userRole 
        //    });
        //}
    }
}