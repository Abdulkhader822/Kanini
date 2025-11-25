using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillAssessmentPortal.Services.Interfaces;

namespace SkillAssessmentPortal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CertificateController : ControllerBase
    {
        private readonly ICertificateService _certificateService;
        private readonly ILogger<CertificateController> _logger;

        public CertificateController(ICertificateService certificateService, ILogger<CertificateController> logger)
        {
            _certificateService = certificateService;
            _logger = logger;
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserCertificates(int userId)
        {
            _logger.LogInformation("Fetching certificates for UserId: {UserId}", userId);
            try
            {
                var certificates = await _certificateService.GetUserCertificatesAsync(userId);
                return Ok(certificates);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching certificates for UserId: {UserId}", userId);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }
}