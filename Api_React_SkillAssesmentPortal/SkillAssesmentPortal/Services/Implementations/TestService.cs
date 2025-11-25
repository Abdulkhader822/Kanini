using SkillAssessmentPortal.DTOs.Test;
using SkillAssessmentPortal.Models;
using SkillAssessmentPortal.Repositories.Interfaces;
using SkillAssessmentPortal.Services.Interfaces;
using SkillAssessmentPortal.Models.Enums;

namespace SkillAssessmentPortal.Services.Implementations
{
    public class TestService : ITestService
    {
        private readonly ITestRepository _testRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITestLevelRepository _testLevelRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly ILogger<TestService> _logger;

        public TestService(ITestRepository testRepository, ICategoryRepository categoryRepository, IUserRepository userRepository, ITestLevelRepository testLevelRepository, IQuestionRepository questionRepository, ILogger<TestService> logger)
        {
            _testRepository = testRepository;
            _categoryRepository = categoryRepository;
            _userRepository = userRepository;
            _testLevelRepository = testLevelRepository;
            _questionRepository = questionRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<TestResponseDto>> GetAllAsync()
        {
            var tests = await _testRepository.GetAllAsync();
            return tests.Select(t => new TestResponseDto
            {
                TestId = t.TestId,
                TestName = t.TestName,
                CategoryName = t.Category?.CategoryName ?? "Unknown",
                DurationMins = t.DurationMins,
                TotalQuestions = t.TotalQuestions,
                TotalMarks = t.TotalMarks,
                MarksPerQuestion = Math.Round(100m / t.TotalQuestions, 2),
                CreatedByName = t.AdminUser?.Name ?? "Admin",
                CreatedAt = t.CreatedAt
            });
        }

        public async Task<TestResponseDto?> GetByIdAsync(int id)
        {
            var t = await _testRepository.GetByIdAsync(id);
            if (t == null) return null;

            return new TestResponseDto
            {
                TestId = t.TestId,
                TestName = t.TestName,
                CategoryName = t.Category?.CategoryName ?? "Unknown",
                DurationMins = t.DurationMins,
                TotalQuestions = t.TotalQuestions,
                TotalMarks = t.TotalMarks,
                MarksPerQuestion = Math.Round(100m / t.TotalQuestions, 2),
                CreatedByName = t.AdminUser?.Name ?? "Admin",
                CreatedAt = t.CreatedAt
            };
        }

        public async Task<(bool IsSuccess, string Message)> CreateAsync(TestCreateDto dto)
        {
            var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);
            if (category == null)
                return (false, "Invalid Category ID.");

            var admin = await _userRepository.GetByIdAsync(dto.CreatedBy);
            if (admin == null || admin.Role != RoleType.Admin) return (false, "Invalid Admin user.");

            var existingTest = await _testRepository.GetByNameAsync(dto.TestName);
            if (existingTest != null)
                return (false, "Test name already exists.");

            var test = new Test
            {
                CategoryId = dto.CategoryId,
                TestName = dto.TestName,
                DurationMins = dto.DurationMins,
                TotalQuestions = dto.TotalQuestions,
                TotalMarks = 100,
                CreatedBy = dto.CreatedBy,
                CreatedAt = DateTime.Now
            };

            await _testRepository.AddAsync(test);
            var marksPerQ = Math.Round(100m / dto.TotalQuestions, 2);
            return (true, $"Test '{dto.TestName}' created successfully ({dto.TotalQuestions} questions, {marksPerQ} marks/question).");
        }

        public async Task<string> UpdateAsync(int id, TestUpdateDto dto)
        {
            var test = await _testRepository.GetByIdAsync(id);
            if (test == null)
                return "Test not found.";

            test.TestName = dto.TestName;
            test.TotalQuestions = dto.TotalQuestions;
            test.DurationMins = dto.DurationMins;

            await _testRepository.UpdateAsync(test);
            return "Test updated successfully.";
        }

        public async Task<string> DeleteAsync(int id)
        {
            _logger.LogInformation("Attempting to delete test with ID: {TestId}", id);
            
            var test = await _testRepository.GetByIdAsync(id);
            if (test == null)
            {
                _logger.LogWarning("Test not found for deletion, ID: {TestId}", id);
                return "Test not found.";
            }

            try
            {
                await _testRepository.DeleteAsync(id);
                _logger.LogInformation("Test deleted successfully with cascade delete, ID: {TestId}", id);
                return "Test deleted successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting test ID: {TestId}", id);
                throw new InvalidOperationException("Failed to delete test due to database constraints.", ex);
            }
        }

        public async Task<IEnumerable<TestResponseDto>> GetAvailableTestsForUserAsync()
        {
            _logger.LogInformation("Filtering available tests for users with optimized queries");
            
            // Get all tests with related data in single query
            var allTests = await _testRepository.GetAllWithIncludesAsync();
            
            // Get all test levels and question counts in batch
            var allTestIds = allTests.Select(t => t.TestId).ToList();
            var allLevels = await _testLevelRepository.GetByTestIdsAsync(allTestIds);
            var levelQuestionCounts = await _questionRepository.GetQuestionCountsByTestLevelsAsync(
                allLevels.Select(l => l.TestLevelId).ToList());
            
            var availableTests = new List<TestResponseDto>();
            var requiredLevels = new[] { "Easy", "Medium", "Hard" };

            foreach (var test in allTests)
            {
                var testLevels = allLevels.Where(l => l.TestId == test.TestId).ToList();
                
                bool hasAllLevels = requiredLevels.All(levelName => 
                    testLevels.Any(l => l.LevelName.Equals(levelName, StringComparison.OrdinalIgnoreCase)));

                if (hasAllLevels)
                {
                    bool allLevelsHaveEnoughQuestions = testLevels.All(level => 
                        levelQuestionCounts.GetValueOrDefault(level.TestLevelId, 0) >= 10);

                    if (allLevelsHaveEnoughQuestions)
                    {
                        availableTests.Add(new TestResponseDto
                        {
                            TestId = test.TestId,
                            TestName = test.TestName,
                            CategoryName = test.Category?.CategoryName ?? "Unknown",
                            DurationMins = test.DurationMins,
                            TotalQuestions = test.TotalQuestions,
                            TotalMarks = test.TotalMarks,
                            MarksPerQuestion = Math.Round(100m / test.TotalQuestions, 2),
                            CreatedByName = test.AdminUser?.Name ?? "Admin",
                            CreatedAt = test.CreatedAt
                        });
                    }
                }
            }

            _logger.LogInformation("Found {Count} available tests for users out of {Total} total tests", availableTests.Count, allTests.Count());
            return availableTests;
        }

        public async Task<IEnumerable<TestResponseDto>> GetTestsByCategoryAsync(int categoryId)
        {
            _logger.LogInformation("Fetching tests for CategoryId: {CategoryId}", categoryId);
            try
            {
                var allTests = await _testRepository.GetAllAsync();
                var categoryTests = allTests.Where(t => t.CategoryId == categoryId);
                
                var result = categoryTests.Select(t => new TestResponseDto
                {
                    TestId = t.TestId,
                    TestName = t.TestName,
                    CategoryName = t.Category?.CategoryName ?? "Unknown",
                    DurationMins = t.DurationMins,
                    TotalQuestions = t.TotalQuestions,
                    TotalMarks = t.TotalMarks,
                    MarksPerQuestion = Math.Round(100m / t.TotalQuestions, 2),
                    CreatedByName = t.AdminUser?.Name ?? "Admin",
                    CreatedAt = t.CreatedAt
                }).ToList();
                
                _logger.LogInformation("Found {Count} tests for CategoryId: {CategoryId}", result.Count, categoryId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching tests for CategoryId: {CategoryId}", categoryId);
                throw;
            }
        }
    }
}
