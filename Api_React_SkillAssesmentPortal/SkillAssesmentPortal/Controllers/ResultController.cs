using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillAssessmentPortal.DTOs.Result;
using SkillAssessmentPortal.Services.Interfaces;

namespace SkillAssessmentPortal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ResultController : ControllerBase
    {
        private readonly IResultService _resultService;
        private readonly ICertificateService _certificateService;
        private readonly ILogger<ResultController> _logger;

        public ResultController(IResultService resultService, ICertificateService certificateService, ILogger<ResultController> logger)
        {
            _resultService = resultService;
            _certificateService = certificateService;
            _logger = logger;
        }

        // ✅ Submit test results
        [HttpPost("submit")]
        public async Task<IActionResult> SubmitResult([FromBody] ResultCreateDto dto)
        {
            _logger.LogInformation("Validating test submission for UserId: {UserId}, TestId: {TestId}, TestLevelId: {TestLevelId}", 
                dto.UserId, dto.TestId, dto.TestLevelId);
            
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Result submission failed - Invalid model state");
                return BadRequest(new { error = "Invalid test submission data. Please check your answers and try again." });
            }

            try
            {
                // ✅ Pre-submission validation
                var validationResult = await _resultService.ValidateTestAttemptAsync(dto.UserId, dto.TestId);
                if (!validationResult.Success)
                {
                    _logger.LogWarning("Test submission validation failed for UserId: {UserId}, TestId: {TestId} - {Message}", 
                        dto.UserId, dto.TestId, validationResult.Message);
                    return BadRequest(new { error = validationResult.Message });
                }

                var resultResponse = await _resultService.SubmitResultAsync(dto);

                if (resultResponse == null)
                    return BadRequest(new { error = "Result submission failed. Please try again." });

                // ✅ Certificate is already generated in ResultService if all levels passed
                var message = resultResponse.IsFinalLevelCleared 
                    ? "All levels completed successfully! Certificate generated."
                    : "Test completed successfully!";
                    
                var nextStep = resultResponse.ResultStatus == "Pass"
                    ? (resultResponse.IsFinalLevelCleared ? "Download your certificate." : "Proceed to next level.")
                    : "Please review the study video and retry.";

                Response.Headers.Add("X-Status-Message", "Test Result Submitted Successfully");
                return Ok(new
                {
                    message,
                    resultId = resultResponse.ResultId,
                    result = resultResponse,
                    nextStep
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Test submission blocked for UserId: {UserId}, TestId: {TestId} - {Message}", dto.UserId, dto.TestId, ex.Message);
                Response.StatusCode = 400;
                Response.Headers.Add("X-Status-Message", "Test Submission Blocked");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting result for UserId: {UserId}, TestId: {TestId}", dto.UserId, dto.TestId);
                return StatusCode(500, new { error = "Internal server error. Please try again later." });
            }
        }

        // ✅ Get result by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetResultById(int id)
        {
            var result = await _resultService.GetByIdAsync(id);
            if (result == null)
                return NotFound(new { message = $"Result not found for ID {id}" });

            return Ok(result);
        }

        // ✅ Get detailed result with question-wise answers
        [HttpGet("{id}/detailed")]
        public async Task<IActionResult> GetDetailedResult(int id)
        {
            try
            {
                var detailedResult = await _resultService.GetDetailedResultAsync(id);
                if (detailedResult == null)
                    return NotFound(new { message = $"Detailed result not found for ID {id}" });

                return Ok(detailedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting detailed result for ID: {ResultId}", id);
                return StatusCode(500, new { error = "Internal server error. Please try again later." });
            }
        }

        // ✅ View all results for a user
        [HttpGet("user/{userId}/test/{testId}")]
        public async Task<IActionResult> GetUserResults(int userId, int testId)
        {
            var results = await _resultService.GetResultsByUserAndTestAsync(userId, testId);
            return Ok(results);
        }

        // ✅ Get completed levels for authenticated user
        [HttpGet("user/completed/{testId}")]
        public async Task<IActionResult> GetCompletedLevels(int testId)
        {
            _logger.LogInformation("GET /api/Result/user/completed/{TestId} - Fetching completed levels", testId);
            
            // Try multiple claim types for UserId
            var userIdClaim = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value
                           ?? User.FindFirst("sub")?.Value
                           ?? User.FindFirst("userId")?.Value;
            
            if (!int.TryParse(userIdClaim, out int userId))
            {
                _logger.LogWarning("Invalid user authentication - no valid UserId in JWT token. Available claims: {Claims}", 
                    string.Join(", ", User.Claims.Select(c => $"{c.Type}={c.Value}")));
                return BadRequest(new { error = "Invalid user authentication. Please login again." });
            }

            _logger.LogInformation("Extracted UserId: {UserId} from JWT token for TestId: {TestId}", userId, testId);

            try
            {
                var completedLevels = await _resultService.GetCompletedLevelsAsync(userId, testId);
                _logger.LogInformation("Found {Count} completed levels for UserId: {UserId}, TestId: {TestId}: [{Levels}]", 
                    completedLevels.Count(), userId, testId, string.Join(", ", completedLevels));
                
                // Always return 200 OK, even if empty array
                return Ok(completedLevels.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching completed levels for UserId: {UserId}, TestId: {TestId}", userId, testId);
                return StatusCode(500, new { error = "Internal server error. Please try again later." });
            }
        }

        // ✅ Debug endpoint to check all results
        [HttpGet("debug/user/{userId}/test/{testId}")]
        public async Task<IActionResult> DebugUserResults(int userId, int testId)
        {
            _logger.LogInformation("DEBUG: Fetching all results for UserId: {UserId}, TestId: {TestId}", userId, testId);
            var results = await _resultService.GetResultsByUserAndTestAsync(userId, testId);
            return Ok(results);
        }



        // ✅ Test endpoint to verify route is working
        [HttpGet("test/route/{testId}")]
        public IActionResult TestRoute(int testId)
        {
            _logger.LogInformation("TEST: Route working for TestId: {TestId}", testId);
            return Ok(new { message = $"Route working for TestId: {testId}", timestamp = DateTime.UtcNow });
        }

        // ✅ Download certificate (if exists)
        [HttpGet("certificate/download/{userId}/{testId}")]
        public async Task<IActionResult> DownloadCertificate(int userId, int testId)
        {
            _logger.LogInformation("Certificate download requested for UserId: {UserId}, TestId: {TestId}", userId, testId);
            try
            {
                var fileBytes = await _certificateService.DownloadCertificateAsync(userId, testId);
                if (fileBytes == null)
                {
                    _logger.LogWarning("Certificate not found for UserId: {UserId}, TestId: {TestId}", userId, testId);
                    return BadRequest(new { error = "Certificate not found. Please complete all test levels first." });
                }

                _logger.LogInformation("Certificate download successful for UserId: {UserId}, TestId: {TestId}", userId, testId);
                var fileName = $"Certificate_{userId}_{testId}.pdf";
                return File(fileBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading certificate for UserId: {UserId}, TestId: {TestId}", userId, testId);
                return StatusCode(500, new { error = "Internal server error. Please try again later." });
            }
        }
    }
}
