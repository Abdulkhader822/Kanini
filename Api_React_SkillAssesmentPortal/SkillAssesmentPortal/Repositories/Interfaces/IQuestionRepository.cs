using SkillAssessmentPortal.Models;

namespace SkillAssessmentPortal.Repositories.Interfaces
{
    public interface IQuestionRepository
    {
        Task<IEnumerable<Question>> GetByTestLevelAsync(int testLevelId);
        Task<Question?> GetByIdAsync(int id);
        Task AddAsync(Question question);
        Task UpdateAsync(Question question);
        Task DeleteAsync(int id);
        Task<int> CountByTestLevelAsync(int testLevelId);
        Task<int> CountByTestIdAsync(int testId);
        Task<Dictionary<int, int>> GetQuestionCountsByTestLevelsAsync(IEnumerable<int> testLevelIds);
    }
}
