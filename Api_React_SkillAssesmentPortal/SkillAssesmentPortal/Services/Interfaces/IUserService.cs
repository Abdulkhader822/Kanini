using SkillAssessmentPortal.DTOs.User;

namespace SkillAssessmentPortal.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseDto>> GetAllAsync();
        Task<UserResponseDto?> GetByIdAsync(int id);
        Task<(bool IsSuccess, string Message)> CreateAsync(UserCreateDto dto);
        Task<string> UpdateAsync(int id, UserUpdateDto dto);
        Task<string> DeleteAsync(int id);
        Task<(bool IsSuccess, string Message)> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    }
}
