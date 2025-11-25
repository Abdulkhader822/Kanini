using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillAssessmentPortal.DTOs.TestLevel;
using SkillAssessmentPortal.Services.Interfaces;

namespace SkillAssessmentPortal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TestLevelController : ControllerBase
    {
        private readonly ITestLevelService _service;
        private readonly ILogger<TestLevelController> _logger;

        public TestLevelController(ITestLevelService service, ILogger<TestLevelController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("GET /api/TestLevel - Fetching all test levels");
            try
            {
                var levels = await _service.GetAllAsync();
                _logger.LogInformation("Successfully retrieved {Count} test levels", levels.Count());
                Response.Headers.Add("X-Status-Message", "Test Levels Retrieved Successfully");
                return Ok(levels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all test levels");
                return StatusCode(500, new { error = "Failed to retrieve test levels from database. Please try again later." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(TestLevelCreateDto dto)
        {
            _logger.LogInformation("POST /api/TestLevel - Creating test level for TestId: {TestId}, Level: {LevelName}", dto.TestId, dto.LevelName);
            
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                _logger.LogWarning("Test level creation failed - Invalid model state");
                return BadRequest(new { error = $"Invalid test level data: {string.Join(", ", errors)}" });
            }

            try
            {
                var result = await _service.CreateAsync(dto);
                
                if (result.Contains("successfully"))
                {
                    _logger.LogInformation("Test level created successfully for TestId: {TestId}", dto.TestId);
                    Response.Headers.Add("X-Status-Message", "Test Level Created Successfully");
                    return Ok(new { message = result });
                }
                else
                {
                    _logger.LogWarning("Test level creation failed: {Result}", result);
                    return BadRequest(new { error = result });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating test level for TestId: {TestId}", dto.TestId);
                return StatusCode(500, new { error = $"Failed to create test level for test {dto.TestId}. Please check your data and try again." });
            }
        }

        [HttpGet("available-tests")]
        public async Task<IActionResult> GetAvailableTestsForUser()
        {
            _logger.LogInformation("GET /api/TestLevel/available-tests - Fetching available tests for user");
            try
            {
                var availableTests = await _service.GetAvailableTestsForUserAsync();
                _logger.LogInformation("Successfully retrieved {Count} available tests for user", availableTests.Count());
                return Ok(availableTests);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching available tests for user");
                return StatusCode(500, new { error = "Failed to retrieve available tests for user. Please try again later." });
            }
        }

        [HttpGet("with-test-names")]
        public async Task<IActionResult> GetTestLevelsWithTestNames()
        {
            _logger.LogInformation("GET /api/TestLevel/with-test-names - Fetching test levels with test names");
            try
            {
                var levels = await _service.GetTestLevelsWithTestNameAsync();
                _logger.LogInformation("Successfully retrieved {Count} test levels with test names", levels.Count());
                return Ok(levels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching test levels with test names");
                return StatusCode(500, new { error = "Failed to retrieve test levels with test names. Please try again later." });
            }
        }

        [HttpGet("test/{testId}")]
        public async Task<IActionResult> GetTestLevelsByTest(int testId)
        {
            _logger.LogInformation("GET /api/TestLevel/test/{TestId} - Fetching test levels by test", testId);
            try
            {
                var levels = await _service.GetTestLevelsByTestAsync(testId);
                _logger.LogInformation("Successfully retrieved {Count} test levels for TestId: {TestId}", levels.Count(), testId);
                return Ok(levels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching test levels for TestId: {TestId}", testId);
                return StatusCode(500, new { error = $"Failed to retrieve test levels for test {testId}. Please try again later." });
            }
        }

        [HttpGet("details/{id}")]
        public async Task<IActionResult> GetLevelDetails(int id)
        {
            _logger.LogInformation("GET /api/TestLevel/details/{Id} - Fetching test level details", id);
            try
            {
                var level = await _service.GetByIdAsync(id);
                if (level == null)
                {
                    _logger.LogWarning("TestLevel not found for ID: {Id}", id);
                    return BadRequest(new { error = $"Test level with ID {id} not found." });
                }

                _logger.LogInformation("Successfully retrieved test level details for ID: {Id}", id);
                return Ok(level);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching test level details for ID: {Id}", id);
                return StatusCode(500, new { error = $"Failed to retrieve test level details for ID {id}. Please try again later." });
            }
        }

        [HttpPost("validate-start/{testLevelId}")]
        public async Task<IActionResult> ValidateTestStart(int testLevelId)
        {
            _logger.LogInformation("POST /api/TestLevel/validate-start/{TestLevelId} - Validating test start", testLevelId);
            try
            {
                var (canStart, message, waitMinutes) = await _service.ValidateTestStartAsync(testLevelId);
                
                if (!canStart)
                {
                    _logger.LogWarning("Test start blocked for TestLevelId: {TestLevelId} - {Message}", testLevelId, message);
                    return BadRequest(new { error = message, waitMinutes });
                }

                _logger.LogInformation("Test start validated for TestLevelId: {TestLevelId}", testLevelId);
                return Ok(new { message = "Test can be started" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating test start for TestLevelId: {TestLevelId}", testLevelId);
                return StatusCode(500, new { error = $"Failed to validate test start for level {testLevelId}. Please try again later." });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("DELETE /api/TestLevel/{Id} - Deleting test level", id);
            try
            {
                var message = await _service.DeleteAsync(id);
                if (message.Contains("not found"))
                {
                    _logger.LogWarning("Test level deletion failed - Test level not found for ID: {Id}", id);
                    return BadRequest(new { error = $"Test level with ID {id} not found for deletion." });
                }

                _logger.LogInformation("Test level deleted successfully for ID: {Id}", id);
                return Ok(new { message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting test level ID: {Id}", id);
                return StatusCode(500, new { error = $"Failed to delete test level with ID {id}. It may be in use by existing questions or results." });
            }
        }
    }
}
