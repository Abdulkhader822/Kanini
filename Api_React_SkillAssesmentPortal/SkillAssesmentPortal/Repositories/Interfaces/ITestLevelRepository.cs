using SkillAssessmentPortal.Models;

namespace SkillAssessmentPortal.Repositories.Interfaces
{
    public interface ITestLevelRepository
    {
        Task<IEnumerable<TestLevel>> GetAllAsync();
        Task<TestLevel?> GetByIdAsync(int id);
        Task<IEnumerable<TestLevel>> GetByTestIdAsync(int testId);
        Task<IEnumerable<TestLevel>> GetByTestIdsAsync(IEnumerable<int> testIds);
        Task<int> GetTotalDurationByTestIdAsync(int testId);
        Task AddAsync(TestLevel level);
        Task UpdateAsync(TestLevel level);
        Task DeleteAsync(int id);
        Task<bool> HasUserCompletedTestAsync(int testLevelId);
        Task<Result?> GetLastFailedAttemptAsync(int testLevelId);
    }
}
