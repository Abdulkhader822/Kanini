using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillAssessmentPortal.Services.Interfaces;

namespace SkillAssessmentPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserDashboardController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ITestService _testService;
        private readonly ICertificateService _certificateService;
        private readonly ILogger<UserDashboardController> _logger;

        public UserDashboardController(
            ICategoryService categoryService,
            ITestService testService,
            ICertificateService certificateService,
            ILogger<UserDashboardController> logger)
        {
            _categoryService = categoryService;
            _testService = testService;
            _certificateService = certificateService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserDashboardData()
        {
            _logger.LogInformation("Fetching user dashboard data");
            
            try
            {
                var userIdClaim = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value
                               ?? User.FindFirst("sub")?.Value
                               ?? User.FindFirst("userId")?.Value;
                
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    _logger.LogWarning("Invalid user authentication - no valid UserId in JWT token");
                    return BadRequest(new { error = "Invalid user authentication" });
                }

                _logger.LogInformation("Loading data for UserId: {UserId}", userId);

                _logger.LogInformation("Loading categories...");
                var categories = await _categoryService.GetAllAsync();
                _logger.LogInformation("Categories loaded: {Count}", categories?.Count() ?? 0);

                _logger.LogInformation("Loading tests...");
                var tests = await _testService.GetAllAsync();
                _logger.LogInformation("Tests loaded: {Count}", tests?.Count() ?? 0);

                _logger.LogInformation("Loading certificates for UserId: {UserId}...", userId);
                var certificates = await _certificateService.GetUserCertificatesAsync(userId);
                _logger.LogInformation("Certificates loaded: {Count}", certificates?.Count() ?? 0);

                _logger.LogInformation("Successfully loaded dashboard data for UserId: {UserId}", userId);

                return Ok(new
                {
                    categories = categories,
                    tests = tests,
                    certificates = certificates
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user dashboard data: {Message}", ex.Message);
                return StatusCode(500, new { error = "Failed to retrieve dashboard data" });
            }
        }
    }
}