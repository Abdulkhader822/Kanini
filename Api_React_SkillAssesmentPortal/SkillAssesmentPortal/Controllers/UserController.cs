using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillAssessmentPortal.DTOs.User;
using SkillAssessmentPortal.Services.Interfaces;

namespace SkillAssessmentPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("GET /api/User - Fetching all users");
            try
            {
                var users = await _userService.GetAllAsync();
                _logger.LogInformation("Successfully retrieved {Count} users", users.Count());
                Response.Headers.Add("X-Status-Message", "Users Retrieved Successfully");
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all users");
                return StatusCode(500, new { error = "Internal server error. Please try again later." });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation("GET /api/User/{Id} - Fetching user by ID", id);
            try
            {
                var user = await _userService.GetByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("User not found for ID: {Id}", id);
                    return BadRequest(new { error = "User not found" });
                }

                _logger.LogInformation("Successfully retrieved user for ID: {Id}", id);
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user by ID: {Id}", id);
                return StatusCode(500, new { error = "Internal server error. Please try again later." });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromBody] UserCreateDto dto)
        {
            _logger.LogInformation("POST /api/User - Creating new user with email: {Email}", dto.Email?.Substring(0, Math.Min(3, dto.Email.Length)) + "***");
            
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("User creation failed - Invalid model state");
                return BadRequest(new { error = "Please provide valid user information." });
            }

            try
            {
                var (isSuccess, message) = await _userService.CreateAsync(dto);

                if (!isSuccess)
                {
                    _logger.LogWarning("User creation failed: {Message}", message);
                    return BadRequest(new { error = message });
                }

                _logger.LogInformation("User created successfully: {Message}", message);
                Response.Headers.Add("X-Status-Message", "User Created Successfully");
                return Ok(new { message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                return StatusCode(500, new { error = "Internal server error. Please try again later." });
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UserUpdateDto dto)
        {
            _logger.LogInformation("PUT /api/User/{Id} - Updating user", id);
            
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("User update failed - Invalid model state for ID: {Id}", id);
                return BadRequest(new { error = "Please provide valid user information." });
            }

            try
            {
                var message = await _userService.UpdateAsync(id, dto);
                if (message.Contains("not found"))
                {
                    _logger.LogWarning("User update failed - User not found for ID: {Id}", id);
                    return BadRequest(new { error = message });
                }

                _logger.LogInformation("User updated successfully for ID: {Id}", id);
                return Ok(new { message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user ID: {Id}", id);
                return StatusCode(500, new { error = "Internal server error. Please try again later." });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("DELETE /api/User/{Id} - Deleting user", id);
            try
            {
                var message = await _userService.DeleteAsync(id);
                if (message.Contains("not found"))
                {
                    _logger.LogWarning("User deletion failed - User not found for ID: {Id}", id);
                    return BadRequest(new { error = message });
                }

                _logger.LogInformation("User deleted successfully for ID: {Id}", id);
                return Ok(new { message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user ID: {Id}", id);
                return StatusCode(500, new { error = "Internal server error. Please try again later." });
            }
        }

        [HttpPut("{id}/change-password")]
        public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordDto dto)
        {
            _logger.LogInformation("PUT /api/User/{Id}/change-password - Changing password", id);
            
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Password change failed - Invalid model state for ID: {Id}", id);
                return BadRequest(new { error = "Please provide valid password information." });
            }

            try
            {
                var (isSuccess, message) = await _userService.ChangePasswordAsync(id, dto.CurrentPassword, dto.NewPassword);
                
                if (!isSuccess)
                {
                    _logger.LogWarning("Password change failed for ID: {Id} - {Message}", id, message);
                    return BadRequest(new { error = message });
                }

                _logger.LogInformation("Password changed successfully for ID: {Id}", id);
                return Ok(new { message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for user ID: {Id}", id);
                return StatusCode(500, new { error = "Internal server error. Please try again later." });
            }
        }
    }
}
