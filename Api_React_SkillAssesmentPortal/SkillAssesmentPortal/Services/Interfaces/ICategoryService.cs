using SkillAssessmentPortal.DTOs.Category;

namespace SkillAssessmentPortal.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponseDto>> GetAllAsync();
        Task<CategoryResponseDto?> GetByIdAsync(int id);
        Task<(bool IsSuccess, string Message)> CreateAsync(CategoryCreateDto dto);
        Task<string> UpdateAsync(int id, CategoryUpdateDto dto);
        Task<string> DeleteAsync(int id);
    }
}
