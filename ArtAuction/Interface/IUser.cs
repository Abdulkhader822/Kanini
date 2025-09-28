using ArtAuction.Dto;
using static ArtAuction.Dto.UserDtos;

namespace ArtAuction.Interface
{
    public interface IUser
    {
        Task<IEnumerable<UserDtos>> GetAllAsync();
        Task<UserDtos?> GetByIdAsync(int id);
        Task<UserDtos> CreateAsync(UserCreateDto dto);
        Task<UserDtos?> UpdateAsync(int id, UserUpdateDto dto);
        Task<bool> DeleteAsync(int id);
        Task<UserDtos?> LoginAsync(UserLoginDto dto);
    }
}
