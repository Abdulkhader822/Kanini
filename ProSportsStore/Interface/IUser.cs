// Interface/IUser.cs
using ProSportsStore.DTOs;
using ProSportsStore.Models;

namespace ProSportsStore.Interface
{
    public interface IUser
    {
        Task<User> Register(User user);
        Task<User?> Login(LoginDTO loginDTO);

        Task<IEnumerable<User>> GetAllUsers();
        Task<User?> GetUserById(int id);
        Task<User> AddUser(User user);
        Task<User?> UpdateUser(User user);
        Task<bool> DeleteUser(int id);
    }
}
