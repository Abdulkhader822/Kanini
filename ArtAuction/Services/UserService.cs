using ArtAuction.Dto;
using ArtAuction.Interface;

namespace ArtAuction.Services
{
    public class UserService
    {
        private readonly IUser _userRepo;

        public UserService(IUser userRepo)
        {
            _userRepo = userRepo;
        }

        public Task<IEnumerable<UserDtos>> GetAllUsersAsync() => _userRepo.GetAllAsync();
        public Task<UserDtos?> GetUserByIdAsync(int id) => _userRepo.GetByIdAsync(id);
        public Task<UserDtos> CreateUserAsync(UserCreateDto dto) => _userRepo.CreateAsync(dto);
        public Task<UserDtos?> UpdateUserAsync(int id, UserUpdateDto dto) => _userRepo.UpdateAsync(id, dto);
        public Task<bool> DeleteUserAsync(int id) => _userRepo.DeleteAsync(id);
        public Task<UserDtos?> LoginAsync(UserLoginDto dto) => _userRepo.LoginAsync(dto);
    }
}

