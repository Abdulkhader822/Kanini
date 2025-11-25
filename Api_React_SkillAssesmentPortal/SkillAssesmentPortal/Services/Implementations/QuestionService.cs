using SkillAssessmentPortal.DTOs.Question;
using SkillAssessmentPortal.Models;
using SkillAssessmentPortal.Repositories.Interfaces;
using SkillAssessmentPortal.Services.Interfaces;

namespace SkillAssessmentPortal.Services.Implementations
{
    public class QuestionService : IQuestionService
    {
        private readonly IQuestionRepository _questionRepo;
        private readonly ITestRepository _testRepo;
        private readonly IResultRepository _resultRepo;
        private readonly ITestLevelRepository _testLevelRepo;
        private readonly ILogger<QuestionService> _logger;

        public QuestionService(
            IQuestionRepository questionRepo,
            ITestRepository testRepo,
            IResultRepository resultRepo,
            ITestLevelRepository testLevelRepo,
            ILogger<QuestionService> logger)
        {
            _questionRepo = questionRepo;
            _testRepo = testRepo;
            _resultRepo = resultRepo;
            _testLevelRepo = testLevelRepo;
            _logger = logger;
        }
        public async Task<IEnumerable<QuestionResponseDto>> GetQuestionsByLevelAsync(int testLevelId)
        {
            var items = await _questionRepo.GetByTestLevelAsync(testLevelId);
            return items.Select(q => new QuestionResponseDto
            {
                QuestionId = q.QuestionId,
                TestLevelId = q.TestLevelId,
                QuestionText = q.QuestionText,
                OptionA = q.OptionA,
                OptionB = q.OptionB,
                OptionC = q.OptionC,
                OptionD = q.OptionD
            });
        }

        public async Task<(bool Allowed, string Message, IEnumerable<QuestionResponseDto>? Questions)>
            GetQuestionsForUserAsync(int testId, int testLevelId, int userId)
        {
            _logger.LogInformation("Validating question access for UserId: {UserId}, TestId: {TestId}, TestLevelId: {TestLevelId}", 
                userId, testId, testLevelId);
            
            var test = await _testRepo.GetByIdAsync(testId);
            if (test == null) 
            {
                _logger.LogWarning("Question access denied - Invalid test: {TestId}", testId);
                return (false, "Invalid test selected.", null);
            }

            var level = await _testLevelRepo.GetByIdAsync(testLevelId);
            if (level == null) return (false, "Invalid test level selected.", null);

            var levelsOfTest = await _testLevelRepo.GetByTestIdAsync(testId);
            var qns = await _questionRepo.GetByTestLevelAsync(testLevelId);
            if (!levelsOfTest.Any() || !qns.Any())
                return (false, "This test is incomplete (levels or questions missing). Please try later.", null);

            if (level.LevelName.Equals("Medium", StringComparison.OrdinalIgnoreCase))
            {
                bool easyPassed = await _resultRepo.HasUserPassedLevelAsync(userId, testId, "Easy");
                if (!easyPassed) 
                {
                    _logger.LogWarning("Medium level access denied - Easy level not passed for UserId: {UserId}, TestId: {TestId}", userId, testId);
                    return (false, "You must pass the Easy level before attempting Medium.", null);
                }
            }
            else if (level.LevelName.Equals("Hard", StringComparison.OrdinalIgnoreCase))
            {
                bool mediumPassed = await _resultRepo.HasUserPassedLevelAsync(userId, testId, "Medium");
                if (!mediumPassed) 
                {
                    _logger.LogWarning("Hard level access denied - Medium level not passed for UserId: {UserId}, TestId: {TestId}", userId, testId);
                    return (false, "You must pass the Medium level before attempting Hard.", null);
                }
            }

            // Generate consistent question order based on user, test level, and attempt number
            var questionsList = qns.ToList();
            var attemptCount = await _resultRepo.GetAttemptCountAsync(userId, testId);
            var seed = $"{userId}_{testLevelId}_{attemptCount + 1}".GetHashCode();
            var random = new Random(seed);
            
            // Shuffle questions for each new attempt
            for (int i = questionsList.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (questionsList[i], questionsList[j]) = (questionsList[j], questionsList[i]);
            }

            var response = questionsList.Select(q => new QuestionResponseDto
            {
                QuestionId = q.QuestionId,
                TestLevelId = q.TestLevelId,
                QuestionText = q.QuestionText,
                OptionA = q.OptionA,
                OptionB = q.OptionB,
                OptionC = q.OptionC,
                OptionD = q.OptionD
            });

            return (true, "You can start this level.", response);
        }

        public async Task<string> AddQuestionAsync(QuestionCreateDto dto)
        {
            _logger.LogInformation("Adding single question to TestId: {TestId}, TestLevelId: {TestLevelId}", dto.TestId, dto.TestLevelId);
            
            // Validate question limits with testId consistency check
            var validationResult = await ValidateQuestionLimitsAsync(dto.TestLevelId, 1, dto.TestId);
            if (!validationResult.IsValid)
            {
                _logger.LogError("Question addition failed - {Message}", validationResult.Message);
                return validationResult.Message;
            }

            var question = new Question
            {
                TestLevelId = dto.TestLevelId,
                QuestionText = dto.QuestionText,
                OptionA = dto.OptionA,
                OptionB = dto.OptionB,
                OptionC = dto.OptionC,
                OptionD = dto.OptionD,
                CorrectOption = dto.CorrectOption
            };
            
            await _questionRepo.AddAsync(question);
            _logger.LogInformation("Question added successfully to TestLevelId: {TestLevelId}", dto.TestLevelId);
            return "Question added successfully.";
        }

        public async Task<string> AddBulkQuestionsAsync(BulkQuestionCreateDto dto)
        {
            _logger.LogInformation("Adding {Count} bulk questions to TestId: {TestId}, TestLevelId: {TestLevelId}", dto.Questions.Count, dto.TestId, dto.TestLevelId);
            
            // Validate question limits with testId consistency check
            var validationResult = await ValidateQuestionLimitsAsync(dto.TestLevelId, dto.Questions.Count, dto.TestId);
            if (!validationResult.IsValid)
            {
                _logger.LogError("Bulk question addition failed - {Message}", validationResult.Message);
                return validationResult.Message;
            }

            var testLevel = await _testLevelRepo.GetByIdAsync(dto.TestLevelId);
            if (testLevel == null)
            {
                _logger.LogWarning("Bulk question addition failed - Invalid TestLevelId: {TestLevelId}", dto.TestLevelId);
                return "Invalid test level ID.";
            }

            foreach (var questionDto in dto.Questions)
            {
                var question = new Question
                {
                    TestLevelId = dto.TestLevelId,
                    QuestionText = questionDto.QuestionText,
                    OptionA = questionDto.OptionA,
                    OptionB = questionDto.OptionB,
                    OptionC = questionDto.OptionC,
                    OptionD = questionDto.OptionD,
                    CorrectOption = questionDto.CorrectOption
                };
                await _questionRepo.AddAsync(question);
            }

            _logger.LogInformation("{Count} questions added successfully to {LevelName} level", dto.Questions.Count, testLevel.LevelName);
            return $"{dto.Questions.Count} questions added successfully to {testLevel.LevelName} level.";
        }

        public async Task<(bool Success, string Message, QuestionResponseDto? Question)> UpdateQuestionAsync(QuestionUpdateDto dto)
        {
            _logger.LogInformation("Updating question with ID: {QuestionId}", dto.QuestionId);
            
            var question = await _questionRepo.GetByIdAsync(dto.QuestionId);
            if (question == null)
            {
                _logger.LogWarning("Question update failed - Question not found: {QuestionId}", dto.QuestionId);
                return (false, "Question not found.", null);
            }

            question.QuestionText = dto.QuestionText;
            question.OptionA = dto.OptionA;
            question.OptionB = dto.OptionB;
            question.OptionC = dto.OptionC;
            question.OptionD = dto.OptionD;
            question.CorrectOption = dto.CorrectOption;

            await _questionRepo.UpdateAsync(question);
            _logger.LogInformation("Question updated successfully: {QuestionId}", dto.QuestionId);
            
            var responseDto = new QuestionResponseDto
            {
                QuestionId = question.QuestionId,
                TestLevelId = question.TestLevelId,
                QuestionText = question.QuestionText,
                OptionA = question.OptionA,
                OptionB = question.OptionB,
                OptionC = question.OptionC,
                OptionD = question.OptionD
            };
            
            return (true, "Question updated successfully.", responseDto);
        }

        public async Task<string> DeleteQuestionAsync(int id)
        {
            _logger.LogInformation("Deleting question with ID: {QuestionId}", id);
            
            var question = await _questionRepo.GetByIdAsync(id);
            if (question == null)
            {
                _logger.LogWarning("Question deletion failed - Question not found: {QuestionId}", id);
                return "Question not found.";
            }

            await _questionRepo.DeleteAsync(id);
            _logger.LogInformation("Question deleted successfully: {QuestionId}", id);
            return "Question deleted successfully.";
        }

        public async Task<(int Score, int TotalMarks)> EvaluateAnswersAsync(int testId, int testLevelId, List<QuestionAnswerDto> answers)
        {
            var questions = await _questionRepo.GetByTestLevelAsync(testLevelId);
            if (!questions.Any()) return (0, 0);

            var test = await _testRepo.GetByIdAsync(testId);
            var totalMarks = test?.TotalMarks ?? 100;

            int totalQuestions = questions.Count();
            int marksPerQuestion = totalMarks / totalQuestions;
            int score = 0;

            foreach (var ans in answers)
            {
                var q = questions.FirstOrDefault(x => x.QuestionId == ans.QuestionId);
                if (q != null && string.Equals(q.CorrectOption, ans.SelectedOption, StringComparison.OrdinalIgnoreCase))
                    score += marksPerQuestion;
            }

            return (score, totalMarks);
        }

        private async Task<(bool IsValid, string Message)> ValidateQuestionLimitsAsync(int testLevelId, int questionsToAdd, int? expectedTestId = null)
        {
            var testLevel = await _testLevelRepo.GetByIdAsync(testLevelId);
            if (testLevel == null)
                return (false, "Invalid test level ID.");

            // Validate testId and testLevelId consistency
            if (expectedTestId.HasValue && testLevel.TestId != expectedTestId.Value)
            {
                _logger.LogError("TestId mismatch - Expected: {ExpectedTestId}, Actual: {ActualTestId} for TestLevelId: {TestLevelId}", 
                    expectedTestId.Value, testLevel.TestId, testLevelId);
                return (false, $"TestLevel {testLevelId} does not belong to Test {expectedTestId.Value}. Please verify your selection.");
            }

            var test = await _testRepo.GetByIdAsync(testLevel.TestId);
            if (test == null)
                return (false, "Test not found.");

            // Calculate limits
            var maxQuestionsPerLevel = test.TotalQuestions / 3;
            var currentLevelQuestions = await _questionRepo.CountByTestLevelAsync(testLevelId);
            var currentTestQuestions = await _questionRepo.CountByTestIdAsync(test.TestId);

            // Check test total limit
            if (currentTestQuestions + questionsToAdd > test.TotalQuestions)
            {
                _logger.LogError("Question limit exceeded for TestId: {TestId} - Current: {Current}, Adding: {Adding}, Limit: {Limit}", 
                    test.TestId, currentTestQuestions, questionsToAdd, test.TotalQuestions);
                return (false, $"Cannot add {questionsToAdd} questions. Test limit: {test.TotalQuestions}, Current total: {currentTestQuestions}");
            }

            // Check level limit
            if (currentLevelQuestions + questionsToAdd > maxQuestionsPerLevel)
            {
                _logger.LogError("Level question limit exceeded for TestLevelId: {TestLevelId} - Current: {Current}, Adding: {Adding}, Limit: {Limit}", 
                    testLevelId, currentLevelQuestions, questionsToAdd, maxQuestionsPerLevel);
                return (false, $"Cannot add more than {maxQuestionsPerLevel} questions for {testLevel.LevelName} level (Total test limit: {test.TotalQuestions})");
            }

            // Warning at 90% capacity
            var newLevelTotal = currentLevelQuestions + questionsToAdd;
            if (newLevelTotal >= maxQuestionsPerLevel * 0.9)
            {
                _logger.LogWarning("{LevelName} level approaching capacity for TestId: {TestId} - {Current}/{Max} questions", 
                    testLevel.LevelName, test.TestId, newLevelTotal, maxQuestionsPerLevel);
            }

            return (true, "Validation passed");
        }
    }
}
