using SkillAssessmentPortal.Models;

namespace SkillAssessmentPortal.Repositories.Interfaces
{
    public interface ITestRepository
    {
        Task<IEnumerable<Test>> GetAllAsync();
        Task<IEnumerable<Test>> GetAllWithIncludesAsync();
        Task<Test?> GetByIdAsync(int id);
        Task<Test?> GetByNameAsync(string testName);
        Task<Test> AddAsync(Test test);
        Task UpdateAsync(Test test);
        Task DeleteAsync(int id);

        Task<TestLevel?> GetLevelByIdAsync(int testLevelId);
        Task<bool> HasResultsAsync(int testId);
        Task<bool> HasCertificatesAsync(int testId);

    }
}
