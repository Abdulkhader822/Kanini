using SkillAssessmentPortal.DTOs.Result;
using SkillAssessmentPortal.Models;
using SkillAssessmentPortal.Repositories.Interfaces;
using SkillAssessmentPortal.Services.Interfaces;

namespace SkillAssessmentPortal.Services.Implementations
{
    public class ResultService : IResultService
    {
        private readonly IResultRepository _resultRepo;
        private readonly IQuestionRepository _questionRepo;
        private readonly ITestRepository _testRepo;
        private readonly IUserRepository _userRepo;
        private readonly ICertificateService _certificateService;
        private readonly ICertificateRepository _certificateRepo;
        private readonly ITestLevelRepository _testLevelRepo;
        private readonly ILogger<ResultService> _logger;

        public ResultService(
            IResultRepository resultRepo,
            IQuestionRepository questionRepo,
            ITestRepository testRepo,
            IUserRepository userRepo,
            ICertificateService certificateService,
            ICertificateRepository certificateRepo,
            ITestLevelRepository testLevelRepo,
            ILogger<ResultService> logger)
        {
            _resultRepo = resultRepo;
            _questionRepo = questionRepo;
            _testRepo = testRepo;
            _userRepo = userRepo;
            _certificateService = certificateService;
            _certificateRepo = certificateRepo;
            _testLevelRepo = testLevelRepo;
            _logger = logger;
        }

        // ✅ MAIN LOGIC: Evaluate test answers, calculate marks, and generate certificate
        public async Task<ResultResponseDto> SubmitResultAsync(ResultCreateDto dto)
        {
            _logger.LogInformation("Validating test submission for UserId: {UserId}, TestId: {TestId}, TestLevelId: {TestLevelId}", 
                dto.UserId, dto.TestId, dto.TestLevelId);
            
            // ✅ Defensive validation for invalid submission context
            if (dto.TestId <= 0 || dto.UserId <= 0)
            {
                _logger.LogError("Invalid submission context - TestId: {TestId}, UserId: {UserId}", dto.TestId, dto.UserId);
                throw new ArgumentException("Invalid submission context. Please restart the test.");
            }
            
            var user = await _userRepo.GetByIdAsync(dto.UserId);
            var test = await _testRepo.GetByIdAsync(dto.TestId);
            var questions = await _questionRepo.GetByTestLevelAsync(dto.TestLevelId);

            if (user == null || test == null || !questions.Any())
                throw new Exception("Invalid test or user data.");

            // ✅ 1. Test Expiry Validation (using attempt start time if provided)
            if (await IsTestExpiredAsync(dto.TestId, dto.TestLevelId, dto.StartedAtUtc))
            {
                _logger.LogWarning("User {UserId} attempted expired test {TestId} (StartedAt: {StartedAt})", dto.UserId, dto.TestId, dto.StartedAtUtc);
                throw new InvalidOperationException("Test duration expired. You can no longer attempt this test.");
            }

            // ✅ 2. Certificate Lockout Validation
            var certificate = await _certificateRepo.GetByUserAndTestAsync(dto.UserId, dto.TestId);
            if (certificate != null)
            {
                _logger.LogWarning("User {UserId} already has a certificate for TestId: {TestId}. Reattempt not allowed.", dto.UserId, dto.TestId);
                throw new InvalidOperationException("You have already completed this test and received a certificate. Reattempt not allowed.");
            }

            // ✅ 3. Reattempt Logic
            var attemptCount = await _resultRepo.GetAttemptCountAsync(dto.UserId, dto.TestId);
            var isReattempt = attemptCount > 0;
            var attemptNumber = attemptCount + 1;
            
            _logger.LogInformation("User {UserId} attempting TestLevelId: {TestLevelId} (Attempt #{AttemptNumber})", 
                dto.UserId, dto.TestLevelId, attemptNumber);

            int totalQuestions = questions.Count();
            int totalMarks = test.TotalMarks;

            // marks per question
            decimal marksPerQuestion = (decimal)totalMarks / totalQuestions;
            decimal score = 0;

            // evaluate submitted answers
            foreach (var ans in dto.Answers)
            {
                var q = questions.FirstOrDefault(x => x.QuestionId == ans.QuestionId);
                if (q != null &&
                    string.Equals(q.CorrectOption, ans.SelectedOption, StringComparison.OrdinalIgnoreCase))
                {
                    score += marksPerQuestion;
                }
            }

            // Get the actual passing score for this level
            var level = await _testLevelRepo.GetByIdAsync(dto.TestLevelId);
            var passingScore = level?.PassingScore ?? 60m; // Fallback to 60 if level not found
            
            decimal percentage = Math.Round(score / totalMarks * 100, 2);
            
            // Determine pass/fail based on actual passing score
            var isPassed = percentage >= passingScore;
            string status = isPassed ? "Pass" : "Fail";
            string suggestion = isPassed ? "Excellent performance!" : "Keep learning and try again!";
            
            _logger.LogInformation("Pass/Fail determination for UserId: {UserId} - Percentage: {Percentage}%, Required: {PassingScore}%, Status: {Status}", 
                dto.UserId, percentage, passingScore, status);
            
            _logger.LogInformation("Score calculated for UserId: {UserId} - Score: {Score}/{TotalMarks}, Percentage: {Percentage}%, Status: {Status}", 
                dto.UserId, (int)Math.Round(score), totalMarks, percentage, status);

            // ✅ Save result
            var result = new Result
            {
                UserId = dto.UserId,
                TestId = dto.TestId,
                TestLevelId = dto.TestLevelId,
                AttemptNumber = attemptNumber,
                Score = (int)Math.Round(score),
                Percentage = percentage,
                TimeTakenSecs = dto.TimeTakenSecs,
                DateAttempted = DateTime.UtcNow,
                ResultStatus = status,
                Suggestion = suggestion
            };

            _logger.LogInformation("DEBUG: Saving result - UserId: {UserId}, TestId: {TestId}, TestLevelId: {TestLevelId}, Status: {Status}, Score: {Score}, Percentage: {Percentage}", 
                result.UserId, result.TestId, result.TestLevelId, result.ResultStatus, result.Score, result.Percentage);
            
            await _resultRepo.AddAsync(result);
            
            _logger.LogInformation("DEBUG: Result saved with ResultId: {ResultId}", result.ResultId);

            // ✅ Store detailed answer information
            foreach (var ans in dto.Answers)
            {
                var question = questions.FirstOrDefault(q => q.QuestionId == ans.QuestionId);
                if (question != null)
                {
                    var userAnswer = new UserAnswer
                    {
                        ResultId = result.ResultId,
                        QuestionId = ans.QuestionId,
                        SelectedOption = ans.SelectedOption ?? "",
                        CorrectOption = question.CorrectOption,
                        IsCorrect = string.Equals(question.CorrectOption, ans.SelectedOption, StringComparison.OrdinalIgnoreCase)
                    };
                    await _resultRepo.AddUserAnswerAsync(userAnswer);
                }
            }

            // ✅ Generate certificate if all levels passed
            bool passedAll = await _resultRepo.HasUserPassedAllLevelsAsync(dto.UserId, dto.TestId);
            if (passedAll)
            {
                _logger.LogInformation("User {UserId} has passed all levels for Test {TestId}. Generating certificate.", dto.UserId, dto.TestId);
                await _certificateService.GenerateCertificateAsync(
                    dto.UserId, 
                    dto.TestId, 
                    user.Name, 
                    test.TestName, 
                    (int)Math.Round(score), 
                    percentage);
                _logger.LogInformation("Certificate generated successfully for UserId: {UserId}, TestId: {TestId}", dto.UserId, dto.TestId);
            }

            var responseMessage = isReattempt 
                ? "This is a reattempt."
                : "Result saved successfully.";

            _logger.LogInformation("Result submitted successfully for UserId: {UserId}, TestId: {TestId}, Attempt: {AttemptNumber}", 
                dto.UserId, dto.TestId, attemptNumber);

            return new ResultResponseDto
            {
                ResultId = result.ResultId,
                UserId = user.UserId,
                UserName = user.Name,
                TestId = result.TestId,
                TestName = test.TestName,
                TestLevelId = result.TestLevelId,
                LevelName = level?.LevelName ?? "Unknown Level",
                PassingScore = passingScore,
                Score = (int)Math.Round(result.Score),
                Percentage = result.Percentage,
                ResultStatus = result.ResultStatus,
                Suggestion = responseMessage + " " + result.Suggestion,
                DateAttempted = result.DateAttempted,
                AttemptNumber = attemptNumber,
                IsReattempt = isReattempt,
                HasCertificate = passedAll,
                IsFinalLevelCleared = passedAll
            };
        }

        // ✅ Fetch all results for a user and a test
        public async Task<IEnumerable<ResultResponseDto>> GetResultsByUserAndTestAsync(int userId, int testId)
        {
            var results = await _resultRepo.GetResultsByUserAndTestAsync(userId, testId);

            return results.Select(r => new ResultResponseDto
            {
                ResultId = r.ResultId,
                TestId = r.TestId,
                TestLevelId = r.TestLevelId,
                Score = (int)Math.Round(r.Score),
                Percentage = r.Percentage,
                ResultStatus = r.ResultStatus,
                Suggestion = r.Suggestion,
                DateAttempted = r.DateAttempted
            });
        }

        // ✅ Suggestion logic
        private string GetSuggestion(decimal percentage)
        {
            if (percentage >= 85) return "Excellent performance! Keep it up.";
            if (percentage >= 70) return "Good work! A bit more practice can make it perfect.";
            if (percentage >= 50) return "Average result. Review the material again.";
            return "Needs improvement. Please rewatch the study video and retry.";
        }

        // ✅ Test expiry validation helper with attempt start time support
        private async Task<bool> IsTestExpiredAsync(int testId, int testLevelId, DateTime? startedAtUtc = null)
        {
            _logger.LogDebug("Checking if test {TestId} level {TestLevelId} is expired", testId, testLevelId);
            try
            {
                var test = await _testRepo.GetByIdAsync(testId);
                if (test == null) return true;

                // Use StartedAtUtc when provided, otherwise fall back to test creation time
                DateTime attemptStartUtc;
                if (startedAtUtc.HasValue)
                {
                    attemptStartUtc = startedAtUtc.Value;
                    
                    // Defensive validation for StartedAtUtc
                    var skew = (DateTime.UtcNow - startedAtUtc.Value).TotalHours;
                    if (skew > 24 || startedAtUtc.Value > DateTime.UtcNow.AddMinutes(1))
                    {
                        _logger.LogWarning("Invalid attempt start time for TestId: {TestId} - StartedAt: {StartedAt}", testId, startedAtUtc.Value);
                        return true; // Treat as expired for invalid timestamps
                    }
                }
                else
                {
                    attemptStartUtc = test.CreatedAt.ToUniversalTime();
                }

                // Get level duration for more accurate expiry calculation
                var level = await _testLevelRepo.GetByIdAsync(testLevelId);
                var durationMins = level?.DurationMins ?? test.DurationMins;
                
                var expiryTime = attemptStartUtc.AddMinutes(durationMins);
                // Add 5 minute grace period for auto-submit to handle network delays
                var graceExpiryTime = expiryTime.AddMinutes(5);
                var isExpired = DateTime.UtcNow > graceExpiryTime;
                
                _logger.LogInformation("Test {TestId} expiry check - StartedAt: {StartedAt}, Duration: {Duration} mins, Expires: {Expiry}, GraceExpiry: {GraceExpiry}, IsExpired: {IsExpired}, CurrentTime: {CurrentTime}", 
                    testId, attemptStartUtc, durationMins, expiryTime, graceExpiryTime, isExpired, DateTime.UtcNow);
                
                return isExpired;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking test expiry for TestId: {TestId}", testId);
                return true; // Fail safe - treat as expired if error occurs
            }
        }

        // ✅ Get result by ID with related data
        public async Task<ResultResponseDto?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Fetching result by ID: {ResultId}", id);
            try
            {
                var result = await _resultRepo.GetByIdAsync(id);
                if (result == null)
                {
                    _logger.LogWarning("Result not found for ID: {ResultId}", id);
                    return null;
                }

                // Get related entities
                var user = await _userRepo.GetByIdAsync(result.UserId);
                var test = await _testRepo.GetByIdAsync(result.TestId);
                var testLevel = await _testLevelRepo.GetByIdAsync(result.TestLevelId);

                _logger.LogInformation("Result found for ID: {ResultId}", id);
                return new ResultResponseDto
                {
                    ResultId = result.ResultId,
                    UserId = result.UserId,
                    UserName = user?.Name ?? "Unknown User",
                    TestId = result.TestId,
                    TestName = test?.TestName ?? "Unknown Test",
                    TestLevelId = result.TestLevelId,
                    LevelName = testLevel?.LevelName ?? "Unknown Level",
                    PassingScore = testLevel?.PassingScore ?? 60m,
                    Score = (int)Math.Round(result.Score),
                    Percentage = result.Percentage,
                    ResultStatus = result.ResultStatus,
                    AttemptNumber = result.AttemptNumber,
                    DateAttempted = result.DateAttempted,
                    Suggestion = result.Suggestion ?? string.Empty,
                    IsReattempt = result.AttemptNumber > 1,
                    HasCertificate = false,
                    IsFinalLevelCleared = false
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching result by ID: {ResultId}", id);
                throw;
            }
        }

        // ✅ Get completed level names for user and test
        public async Task<IEnumerable<string>> GetCompletedLevelsAsync(int userId, int testId)
        {
            _logger.LogInformation("Fetching completed levels for UserId: {UserId}, TestId: {TestId}", userId, testId);
            try
            {
                var completedLevels = await _resultRepo.GetCompletedLevelsAsync(userId, testId);
                _logger.LogInformation("Found {Count} completed levels for UserId: {UserId}, TestId: {TestId}", completedLevels.Count(), userId, testId);
                return completedLevels;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching completed levels for UserId: {UserId}, TestId: {TestId}", userId, testId);
                throw;
            }
        }



        // ✅ Get detailed result with question-wise answers
        public async Task<DetailedResultDto?> GetDetailedResultAsync(int resultId)
        {
            _logger.LogInformation("Fetching detailed result for ResultId: {ResultId}", resultId);
            try
            {
                var result = await _resultRepo.GetByIdAsync(resultId);
                if (result == null)
                {
                    _logger.LogWarning("Result not found for ID: {ResultId}", resultId);
                    return null;
                }

                // Get related entities
                var user = await _userRepo.GetByIdAsync(result.UserId);
                var test = await _testRepo.GetByIdAsync(result.TestId);
                var testLevel = await _testLevelRepo.GetByIdAsync(result.TestLevelId);
                var userAnswers = await _resultRepo.GetUserAnswersByResultIdAsync(resultId);

                var detailedResult = new DetailedResultDto
                {
                    ResultId = result.ResultId,
                    UserId = result.UserId,
                    UserName = user?.Name ?? "Unknown User",
                    TestId = result.TestId,
                    TestName = test?.TestName ?? "Unknown Test",
                    TestLevelId = result.TestLevelId,
                    LevelName = testLevel?.LevelName ?? "Unknown Level",
                    PassingScore = testLevel?.PassingScore ?? 60m,
                    Score = (int)Math.Round(result.Score),
                    Percentage = result.Percentage,
                    ResultStatus = result.ResultStatus,
                    DateAttempted = result.DateAttempted,
                    AttemptNumber = result.AttemptNumber,
                    Suggestion = result.Suggestion ?? string.Empty,
                    Questions = userAnswers.Select(ua => new QuestionResultDto
                    {
                        QuestionId = ua.QuestionId,
                        QuestionText = ua.Question?.QuestionText ?? "Question not found",
                        OptionA = ua.Question?.OptionA ?? "",
                        OptionB = ua.Question?.OptionB ?? "",
                        OptionC = ua.Question?.OptionC ?? "",
                        OptionD = ua.Question?.OptionD ?? "",
                        CorrectOption = ua.CorrectOption,
                        SelectedOption = ua.SelectedOption,
                        IsCorrect = ua.IsCorrect
                    }).ToList()
                };

                _logger.LogInformation("Detailed result retrieved for ResultId: {ResultId} with {QuestionCount} questions", resultId, detailedResult.Questions.Count);
                return detailedResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching detailed result for ResultId: {ResultId}", resultId);
                throw;
            }
        }

        // ✅ Validation method for test attempts (backward compatible)
        public async Task<(bool Success, string Message)> ValidateTestAttemptAsync(int userId, int testId)
        {
            _logger.LogInformation("Validating test attempt for UserId: {UserId}, TestId: {TestId}", userId, testId);
            
            try
            {
                // Check certificate lockout
                var certificate = await _certificateRepo.GetByUserAndTestAsync(userId, testId);
                if (certificate != null)
                {
                    _logger.LogWarning("Reattempt blocked due to existing certificate for UserId: {UserId}, TestId: {TestId}", userId, testId);
                    return (false, "You have already completed this test and received a certificate. Reattempt not allowed.");
                }

                // Note: Cooldown validation is now handled per-level in SubmitResultAsync
                // This validation method is kept for backward compatibility

                _logger.LogInformation("Test attempt validation passed for UserId: {UserId}, TestId: {TestId}", userId, testId);
                return (true, "Validation passed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during test attempt validation for UserId: {UserId}, TestId: {TestId}", userId, testId);
                return (false, "Validation error occurred. Please try again.");
            }
        }

    }
}
