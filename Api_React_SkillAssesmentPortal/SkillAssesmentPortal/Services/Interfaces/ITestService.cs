using SkillAssessmentPortal.DTOs.Test;

namespace SkillAssessmentPortal.Services.Interfaces
{
    public interface ITestService
    {
        Task<IEnumerable<TestResponseDto>> GetAllAsync();
        Task<IEnumerable<TestResponseDto>> GetAvailableTestsForUserAsync();
        Task<TestResponseDto?> GetByIdAsync(int id);
        Task<(bool IsSuccess, string Message)> CreateAsync(TestCreateDto dto);
        Task<string> UpdateAsync(int id, TestUpdateDto dto);
        Task<string> DeleteAsync(int id);
        Task<IEnumerable<TestResponseDto>> GetTestsByCategoryAsync(int categoryId);
    }
}
