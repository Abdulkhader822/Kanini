using SkillAssessmentPortal.DTOs.TestLevel;
using SkillAssessmentPortal.Models;
using SkillAssessmentPortal.Repositories.Interfaces;
using SkillAssessmentPortal.Services.Interfaces;

namespace SkillAssessmentPortal.Services.Implementations
{
    public class TestLevelService : ITestLevelService
    {
        private readonly ITestLevelRepository _levelRepo;
        private readonly ITestRepository _testRepo;
        private readonly ILogger<TestLevelService> _logger;

        public TestLevelService(ITestLevelRepository levelRepo, ITestRepository testRepo, ILogger<TestLevelService> logger)
        {
            _levelRepo = levelRepo;
            _testRepo = testRepo;
            _logger = logger;
        }

        // ✅ Get all test levels
        public async Task<IEnumerable<TestLevelResponseDto>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all test levels from repository");
            try
            {
                var levels = await _levelRepo.GetAllAsync();
                _logger.LogInformation("Successfully retrieved {Count} test levels", levels.Count());
                return levels.Select(l => new TestLevelResponseDto
                {
                    TestLevelId = l.TestLevelId,
                    TestId = l.TestId,
                    LevelName = l.LevelName,
                    PassingScore = l.PassingScore,
                    VideoLink = l.VideoLink,
                    DurationMins = l.DurationMins
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving test levels from repository");
                throw;
            }
        }

        // ✅ Add test level
        public async Task<string> CreateAsync(TestLevelCreateDto dto)
        {
            _logger.LogInformation("Creating test level for TestId: {TestId}, Level: {LevelName}, Duration: {Duration} mins", 
                dto.TestId, dto.LevelName, dto.DurationMins);
            
            try
            {
                var test = await _testRepo.GetByIdAsync(dto.TestId);
                if (test == null)
                {
                    _logger.LogWarning("Test level creation failed - Test not found for TestId: {TestId}", dto.TestId);
                    return "Test not found.";
                }

                if (!IsValidYouTubeLink(dto.VideoLink))
                {
                    _logger.LogWarning("Test level creation failed - Invalid YouTube link: {VideoLink}", dto.VideoLink);
                    return "Invalid video link. Must be a valid YouTube URL.";
                }

                // Validate time distribution
                var timeValidation = await ValidateTimeLimitsAsync(dto.TestId, dto.DurationMins);
                if (!timeValidation.IsValid)
                {
                    _logger.LogError("Test level creation failed - {Message}", timeValidation.Message);
                    return timeValidation.Message;
                }

                var level = new TestLevel
                {
                    TestId = dto.TestId,
                    LevelName = dto.LevelName,
                    PassingScore = dto.PassingScore,
                    VideoLink = dto.VideoLink,
                    DurationMins = dto.DurationMins
                };

                await _levelRepo.AddAsync(level);
                _logger.LogInformation("Test level created successfully for TestId: {TestId}, Level: {LevelName}", dto.TestId, dto.LevelName);
                return "Test level created successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating test level for TestId: {TestId}", dto.TestId);
                throw;
            }
        }

        // ✅ Only show tests to user that have levels and questions
        public async Task<IEnumerable<Test>> GetAvailableTestsForUserAsync()
        {
            _logger.LogInformation("Filtering available tests for user based on levels and questions");
            try
            {
                var allTests = await _testRepo.GetAllAsync();
                _logger.LogDebug("Retrieved {TotalCount} total tests for filtering", allTests.Count());
                
                var readyTests = allTests.Where(t =>
                    t.Levels != null &&
                    t.Levels.Any(l => l.Questions != null && l.Questions.Count > 0)
                );
                
                var readyTestsList = readyTests.ToList();
                _logger.LogInformation("Found {AvailableCount} tests available for user out of {TotalCount} total tests", readyTestsList.Count, allTests.Count());
                return readyTestsList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error filtering available tests for user");
                throw;
            }
        }

        // ✅ Helper for YouTube validation
        private bool IsValidYouTubeLink(string url)
        {
            var isValid = !string.IsNullOrEmpty(url) &&
                   (url.Contains("youtube.com/watch") || url.Contains("youtu.be/"));
            _logger.LogDebug("YouTube link validation result: {IsValid} for URL: {Url}", isValid, url);
            return isValid;
        }

        private async Task<(bool IsValid, string Message)> ValidateTimeLimitsAsync(int testId, int newLevelDuration)
        {
            var test = await _testRepo.GetByIdAsync(testId);
            if (test == null)
                return (false, "Test not found.");

            var maxDurationPerLevel = test.DurationMins / 3;
            var currentTotalDuration = await _levelRepo.GetTotalDurationByTestIdAsync(testId);

            // Check if adding this level exceeds test total duration
            if (currentTotalDuration + newLevelDuration > test.DurationMins)
            {
                _logger.LogError("Time allocation exceeded for TestId: {TestId} - Total {Total} mins > Allowed {Allowed} mins", 
                    testId, currentTotalDuration + newLevelDuration, test.DurationMins);
                return (false, $"Invalid time distribution — total level duration exceeds test duration ({test.DurationMins} mins).");
            }

            // Check if this level exceeds individual level limit
            if (newLevelDuration > maxDurationPerLevel)
            {
                _logger.LogError("Level duration exceeded for TestId: {TestId} - Level: {Duration} mins > Allowed: {Max} mins", 
                    testId, newLevelDuration, maxDurationPerLevel);
                return (false, $"Invalid time distribution — level exceeds allowed duration limit ({maxDurationPerLevel} mins per level).");
            }

            // Warning at 90% capacity
            var newTotal = currentTotalDuration + newLevelDuration;
            if (newTotal >= test.DurationMins * 0.9)
            {
                _logger.LogWarning("Test duration approaching capacity for TestId: {TestId} - {Current}/{Max} minutes", 
                    testId, newTotal, test.DurationMins);
            }

            return (true, "Time validation passed");
        }

        // ✅ Get test levels with test names for enhanced dropdown display
        public async Task<IEnumerable<TestLevelWithTestDto>> GetTestLevelsWithTestNameAsync()
        {
            _logger.LogInformation("Fetching test levels with test names for dropdown display");
            try
            {
                var levels = await _levelRepo.GetAllAsync();
                var result = levels.Select(l => new TestLevelWithTestDto
                {
                    TestLevelId = l.TestLevelId,
                    TestId = l.TestId,
                    TestName = l.Test?.TestName ?? "Unknown Test",
                    LevelName = l.LevelName,
                    PassingScore = l.PassingScore,
                    VideoLink = l.VideoLink ?? string.Empty,
                    DurationMins = l.DurationMins
                }).OrderBy(x => x.TestName).ThenBy(x => x.LevelName);
                
                _logger.LogInformation("Successfully retrieved {Count} test levels with test names", result.Count());
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching test levels with test names");
                throw;
            }
        }

        // ✅ Get test levels by test ID for cascading dropdown
        public async Task<IEnumerable<TestLevelResponseDto>> GetTestLevelsByTestAsync(int testId)
        {
            _logger.LogInformation("Fetching test levels for TestId: {TestId}", testId);
            try
            {
                var levels = await _levelRepo.GetByTestIdAsync(testId);
                var result = levels.Select(l => new TestLevelResponseDto
                {
                    TestLevelId = l.TestLevelId,
                    TestId = l.TestId,
                    LevelName = l.LevelName,
                    PassingScore = l.PassingScore,
                    VideoLink = l.VideoLink ?? string.Empty,
                    DurationMins = l.DurationMins
                }).OrderBy(x => x.LevelName);
                
                _logger.LogInformation("Successfully retrieved {Count} test levels for TestId: {TestId}", result.Count(), testId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching test levels for TestId: {TestId}", testId);
                throw;
            }
        }

        // ✅ Get single test level by ID with full details
        public async Task<TestLevelResponseDto?> GetByIdAsync(int testLevelId)
        {
            _logger.LogInformation("Fetching test level details for TestLevelId: {TestLevelId}", testLevelId);
            try
            {
                var level = await _levelRepo.GetByIdAsync(testLevelId);
                if (level == null)
                {
                    _logger.LogWarning("Test level not found for TestLevelId: {TestLevelId}", testLevelId);
                    return null;
                }

                var result = new TestLevelResponseDto
                {
                    TestLevelId = level.TestLevelId,
                    TestId = level.TestId,
                    LevelName = level.LevelName,
                    PassingScore = level.PassingScore,
                    VideoLink = level.VideoLink ?? string.Empty,
                    DurationMins = level.DurationMins
                };
                
                _logger.LogInformation("Successfully retrieved test level details for TestLevelId: {TestLevelId}", testLevelId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching test level details for TestLevelId: {TestLevelId}", testLevelId);
                throw;
            }
        }

        // ✅ Validate if user can start test (cooldown + completion check)
        public async Task<(bool CanStart, string Message, int WaitMinutes)> ValidateTestStartAsync(int testLevelId)
        {
            _logger.LogInformation("Validating test start for TestLevelId: {TestLevelId}", testLevelId);
            
            try
            {
                var level = await _levelRepo.GetByIdAsync(testLevelId);
                if (level == null)
                {
                    return (false, "Test level not found.", 0);
                }

                // Check if user has already completed this test (has certificate)
                var hasCertificate = await _levelRepo.HasUserCompletedTestAsync(testLevelId);
                if (hasCertificate)
                {
                    _logger.LogWarning("Test start blocked - User already has certificate for TestId: {TestId}", level.TestId);
                    return (false, "You have already completed this test and earned a certificate. No reattempts allowed.", 0);
                }

                // Check cooldown period for failed attempts
                var lastAttempt = await _levelRepo.GetLastFailedAttemptAsync(testLevelId);
                if (lastAttempt != null)
                {
                    var timeSinceLastAttempt = DateTime.Now - lastAttempt.DateAttempted;
                    var cooldownMinutes = 5; // 5 minute cooldown
                    var remainingMinutes = cooldownMinutes - (int)timeSinceLastAttempt.TotalMinutes;

                    if (remainingMinutes > 0)
                    {
                        _logger.LogWarning("Test start blocked - Cooldown active for TestLevelId: {TestLevelId}, Wait: {Minutes} minutes", testLevelId, remainingMinutes);
                        return (false, $"Please wait {remainingMinutes} minutes before reattempting this level.", remainingMinutes);
                    }
                }

                _logger.LogInformation("Test start validated successfully for TestLevelId: {TestLevelId}", testLevelId);
                return (true, "Test can be started.", 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating test start for TestLevelId: {TestLevelId}", testLevelId);
                throw;
            }
        }

        // ✅ Delete test level
        public async Task<string> DeleteAsync(int testLevelId)
        {
            _logger.LogInformation("Attempting to delete test level with ID: {TestLevelId}", testLevelId);
            
            try
            {
                var level = await _levelRepo.GetByIdAsync(testLevelId);
                if (level == null)
                {
                    _logger.LogWarning("Test level not found for deletion, ID: {TestLevelId}", testLevelId);
                    return "Test level not found.";
                }

                await _levelRepo.DeleteAsync(testLevelId);
                _logger.LogInformation("Test level deleted successfully with cascade delete, ID: {TestLevelId}", testLevelId);
                return "Test level deleted successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting test level ID: {TestLevelId}", testLevelId);
                throw new InvalidOperationException("Failed to delete test level due to database constraints.", ex);
            }
        }
    }
}
