using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillAssessmentPortal.DTOs.Test;
using SkillAssessmentPortal.Services.Interfaces;

namespace SkillAssessmentPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TestController : ControllerBase
    {
        private readonly ITestService _testService;
        private readonly ILogger<TestController> _logger;

        public TestController(ITestService testService, ILogger<TestController> logger)
        {
            _testService = testService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Fetching all tests from database");
            try
            {
                var tests = await _testService.GetAllAsync();
                _logger.LogInformation("Successfully retrieved {Count} tests", tests.Count());
                Response.Headers.Add("X-Status-Message", "Tests Retrieved Successfully");
                return Ok(tests);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all tests");
                return StatusCode(500, new { error = "Failed to retrieve tests from database. Please try again later." });
            }
        }

        [HttpGet("available")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAvailableTests()
        {
            _logger.LogInformation("GET /api/Test/available - Fetching available tests for users");
            try
            {
                var tests = await _testService.GetAvailableTestsForUserAsync();
                _logger.LogInformation("Retrieved {Count} available tests for users", tests.Count());
                return Ok(tests);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching available tests");
                return StatusCode(500, new { error = "Failed to retrieve available tests. Please try again later." });
            }
        }

        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetTestsByCategory(int categoryId)
        {
            _logger.LogInformation("GET /api/Test/category/{CategoryId} - Fetching tests by category", categoryId);
            try
            {
                var tests = await _testService.GetTestsByCategoryAsync(categoryId);
                _logger.LogInformation("Successfully retrieved {Count} tests for CategoryId: {CategoryId}", tests.Count(), categoryId);
                return Ok(tests);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching tests for CategoryId: {CategoryId}", categoryId);
                return StatusCode(500, new { error = $"Failed to retrieve tests for category ID {categoryId}. Please try again later." });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation("Fetching test by ID: {Id}", id);
            try
            {
                var test = await _testService.GetByIdAsync(id);
                if (test == null)
                {
                    _logger.LogWarning("Test not found for ID: {Id}", id);
                    return BadRequest(new { error = $"Test with ID {id} not found." });
                }

                _logger.LogInformation("Test found for ID: {Id}", id);
                return Ok(test);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching test by ID: {Id}", id);
                return StatusCode(500, new { error = $"Failed to retrieve test with ID {id}. Please try again later." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TestCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new { error = $"Invalid test data: {string.Join(", ", errors)}" });
            }

            try
            {
                var (isSuccess, message) = await _testService.CreateAsync(dto);
                if (!isSuccess)
                {
                    Response.StatusCode = 400;
                    Response.Headers.Add("X-Status-Message", "Test Creation Failed");
                    return BadRequest(new { error = message });
                }

                Response.Headers.Add("X-Status-Message", "Test Created Successfully");
                return Ok(new { message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating test");
                return StatusCode(500, new { error = "Failed to create test. Please check your data and try again." });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TestUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new { error = $"Invalid test update data: {string.Join(", ", errors)}" });
            }

            try
            {
                var message = await _testService.UpdateAsync(id, dto);
                if (message.Contains("not found"))
                    return BadRequest(new { error = $"Test with ID {id} not found for update." });

                return Ok(new { message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating test ID: {Id}", id);
                return StatusCode(500, new { error = $"Failed to update test with ID {id}. Please try again later." });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("DELETE /api/Test/{Id} - Deleting test", id);
            try
            {
                var message = await _testService.DeleteAsync(id);
                if (message.Contains("not found"))
                {
                    _logger.LogWarning("Test deletion failed - Test not found for ID: {Id}", id);
                    return BadRequest(new { error = $"Test with ID {id} not found for deletion." });
                }

                _logger.LogInformation("Test deleted successfully for ID: {Id}", id);
                return Ok(new { message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting test ID: {Id}", id);
                return StatusCode(500, new { error = $"Failed to delete test with ID {id}. It may be in use by existing test levels or results." });
            }
        }
    }
}
