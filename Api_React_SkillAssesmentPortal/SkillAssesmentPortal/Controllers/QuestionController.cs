using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillAssessmentPortal.DTOs.Question;
using SkillAssessmentPortal.Services.Interfaces;

namespace SkillAssessmentPortal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionService _service;
        private readonly ILogger<QuestionController> _logger;

        public QuestionController(IQuestionService service, ILogger<QuestionController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet("user/{testId}/{testLevelId}/{userId}")]
        public async Task<IActionResult> GetQuestionsForUser(int testId, int testLevelId, int userId)
        {
            _logger.LogInformation("GET Questions for UserId: {UserId}, TestId: {TestId}, TestLevelId: {TestLevelId}", userId, testId, testLevelId);
            try
            {
                var (allowed, message, questions) = await _service.GetQuestionsForUserAsync(testId, testLevelId, userId);
                if (!allowed)
                {
                    _logger.LogWarning("Question access denied for UserId: {UserId}, TestId: {TestId}, TestLevelId: {TestLevelId} - {Message}", 
                        userId, testId, testLevelId, message);
                    return BadRequest(new { error = message });
                }

                _logger.LogInformation("Questions retrieved successfully for UserId: {UserId}, Count: {Count}", userId, questions?.Count() ?? 0);
                Response.Headers.Add("X-Status-Message", "Questions Retrieved Successfully");
                return Ok(new { message, questions });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving questions for UserId: {UserId}, TestId: {TestId}, TestLevelId: {TestLevelId}", 
                    userId, testId, testLevelId);
                return StatusCode(500, new { error = $"Failed to retrieve questions for user {userId}, test {testId}, level {testLevelId}. Please try again later." });
            }
        }



        [HttpPost]
        public async Task<IActionResult> AddQuestion(QuestionCreateDto dto)
        {
            _logger.LogInformation("POST Question - TestLevelId: {TestLevelId}", dto.TestLevelId);
            
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                _logger.LogWarning("Question creation failed - Invalid model state");
                return BadRequest(new { error = $"Invalid question data: {string.Join(", ", errors)}" });
            }

            try
            {
                var message = await _service.AddQuestionAsync(dto);
                
                if (message.Contains("Cannot add") || message.Contains("Invalid"))
                {
                    _logger.LogWarning("Question creation failed: {Message}", message);
                    return BadRequest(new { error = message });
                }

                _logger.LogInformation("Question created successfully: {Message}", message);
                Response.Headers.Add("X-Status-Message", "Question Created Successfully");
                return Ok(new { message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating question for TestLevelId: {TestLevelId}", dto.TestLevelId);
                return StatusCode(500, new { error = $"Failed to create question for test level {dto.TestLevelId}. Please check your data and try again." });
            }
        }

        [HttpPost("bulk")]
        public async Task<IActionResult> AddBulkQuestions(BulkQuestionCreateDto dto)
        {
            _logger.LogInformation("POST Bulk Questions - TestLevelId: {TestLevelId}, Count: {Count}", dto.TestLevelId, dto.Questions.Count);
            
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                _logger.LogWarning("Bulk question creation failed - Invalid model state");
                return BadRequest(new { error = $"Invalid bulk question data: {string.Join(", ", errors)}" });
            }

            try
            {
                var message = await _service.AddBulkQuestionsAsync(dto);
                
                if (message.Contains("Cannot add") || message.Contains("Invalid"))
                {
                    _logger.LogWarning("Bulk question creation failed: {Message}", message);
                    return BadRequest(new { error = message });
                }

                _logger.LogInformation("Bulk questions created successfully: {Message}", message);
                return Ok(new { message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating bulk questions for TestLevelId: {TestLevelId}", dto.TestLevelId);
                return StatusCode(500, new { error = $"Failed to create bulk questions for test level {dto.TestLevelId}. Please check your data and try again." });
            }
        }

        [HttpGet("testlevel/{testLevelId}")]
        public async Task<IActionResult> GetQuestionsByTestLevel(int testLevelId)
        {
            _logger.LogInformation("GET Questions by TestLevelId: {TestLevelId}", testLevelId);
            
            try
            {
                var questions = await _service.GetQuestionsByLevelAsync(testLevelId);
                _logger.LogInformation("Retrieved {Count} questions for TestLevelId: {TestLevelId}", questions.Count(), testLevelId);
                return Ok(questions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving questions for TestLevelId: {TestLevelId}", testLevelId);
                return StatusCode(500, new { error = $"Failed to retrieve questions for test level {testLevelId}. Please try again later." });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuestion(int id, QuestionUpdateDto dto)
        {
            _logger.LogInformation("PUT Question - ID: {QuestionId}", id);
            
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                _logger.LogWarning("Question update failed - Invalid model state for ID: {QuestionId}", id);
                return BadRequest(new { error = $"Invalid question update data for ID {id}: {string.Join(", ", errors)}" });
            }

            try
            {
                dto.QuestionId = id;
                var (success, message, updatedQuestion) = await _service.UpdateQuestionAsync(dto);
                
                if (!success)
                {
                    _logger.LogWarning("Question update failed - {Message}", message);
                    return BadRequest(new { error = $"Question with ID {id} not found for update." });
                }

                _logger.LogInformation("Question updated successfully: {Message}", message);
                return Ok(updatedQuestion);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating question with ID: {QuestionId}", id);
                return StatusCode(500, new { error = $"Failed to update question with ID {id}. Please try again later." });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            _logger.LogInformation("DELETE Question - ID: {QuestionId}", id);
            
            try
            {
                var message = await _service.DeleteQuestionAsync(id);
                
                if (message.Contains("not found"))
                {
                    _logger.LogWarning("Question deletion failed - {Message}", message);
                    return BadRequest(new { error = $"Question with ID {id} not found for deletion." });
                }

                _logger.LogInformation("Question deleted successfully: {Message}", message);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting question with ID: {QuestionId}", id);
                return StatusCode(500, new { error = $"Failed to delete question with ID {id}. It may be in use by existing results." });
            }
        }

        [HttpPost("evaluate/{testId}/{testLevelId}")]
        public async Task<IActionResult> EvaluateAnswers(int testId, int testLevelId, [FromBody] List<QuestionAnswerDto> answers)
        {
            var (score, totalMarks) = await _service.EvaluateAnswersAsync(testId, testLevelId, answers);
            return Ok(new { Score = score, TotalMarks = totalMarks });
        }
    }
}
