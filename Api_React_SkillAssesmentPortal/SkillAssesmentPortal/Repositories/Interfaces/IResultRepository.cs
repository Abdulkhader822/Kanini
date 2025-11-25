using SkillAssessmentPortal.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SkillAssessmentPortal.Repositories.Interfaces
{
    public interface IResultRepository : IGenericRepository<Result>
    {
        Task<int> GetNextAttemptNumberAsync(int userId, int testId);
        Task<bool> HasUserPassedAllLevelsAsync(int userId, int testId);
        Task<bool> HasUserPassedLevelAsync(int userId, int testId, string levelName);
        Task<IEnumerable<Result>> GetResultsByUserAndTestAsync(int userId, int testId);
        Task<int> GetAttemptCountAsync(int userId, int testId);
        Task<DateTime?> GetLastAttemptDateAsync(int userId, int testId);
        Task<DateTime?> GetLastAttemptDateForLevelAsync(int userId, int testLevelId);
        Task<IEnumerable<string>> GetCompletedLevelsAsync(int userId, int testId);
        Task AddUserAnswerAsync(UserAnswer userAnswer);
        Task<IEnumerable<UserAnswer>> GetUserAnswersByResultIdAsync(int resultId);
    }
}
