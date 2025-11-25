using SkillAssessmentPortal.DTOs.Result;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SkillAssessmentPortal.Services.Interfaces
{
    public interface IResultService
    {
        Task<ResultResponseDto> SubmitResultAsync(ResultCreateDto dto);
        Task<IEnumerable<ResultResponseDto>> GetResultsByUserAndTestAsync(int userId, int testId);
        Task<ResultResponseDto?> GetByIdAsync(int id);
        Task<DetailedResultDto?> GetDetailedResultAsync(int resultId);
        Task<IEnumerable<string>> GetCompletedLevelsAsync(int userId, int testId);
        Task<(bool Success, string Message)> ValidateTestAttemptAsync(int userId, int testId);
    }
}
