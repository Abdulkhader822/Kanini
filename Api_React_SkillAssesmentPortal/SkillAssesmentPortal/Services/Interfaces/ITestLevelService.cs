using SkillAssessmentPortal.DTOs.TestLevel;
using SkillAssessmentPortal.Models;

namespace SkillAssessmentPortal.Services.Interfaces
{
    public interface ITestLevelService
    {
        // ✅ Get all test levels (Admin)
        Task<IEnumerable<TestLevelResponseDto>> GetAllAsync();

        // ✅ Create new test level
        Task<string> CreateAsync(TestLevelCreateDto dto);

        // ✅ Get tests that are ready (for User)
        Task<IEnumerable<Test>> GetAvailableTestsForUserAsync();

        // ✅ Get test levels with test names for dropdown
        Task<IEnumerable<TestLevelWithTestDto>> GetTestLevelsWithTestNameAsync();

        // ✅ Get test levels by test ID for cascading dropdown
        Task<IEnumerable<TestLevelResponseDto>> GetTestLevelsByTestAsync(int testId);

        // ✅ Get single test level details by ID
        Task<TestLevelResponseDto?> GetByIdAsync(int testLevelId);

        // ✅ Validate if user can start test (cooldown + completion check)
        Task<(bool CanStart, string Message, int WaitMinutes)> ValidateTestStartAsync(int testLevelId);

        // ✅ Delete test level
        Task<string> DeleteAsync(int testLevelId);
    }
}
