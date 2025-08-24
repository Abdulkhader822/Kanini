// Repository/UserRepository.cs
using Microsoft.EntityFrameworkCore;
using ProSportsStore.DTOs;
using ProSportsStore.Interface;
using ProSportsStore.Models;

namespace ProSportsStore.Repository
{
    public class UserRepository : IUser
    {
        private readonly ProSportsStoreContext _context;
        public UserRepository(ProSportsStoreContext ctx) => _context = ctx;

        public async Task<User> Register(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> Login(LoginDTO login)
        {
            // Here we compare PasswordHash with plain Password from DTO
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == login.Email && u.PasswordHash == login.Password);
        }

        public Task<IEnumerable<User>> GetAllUsers() =>
            Task.FromResult(_context.Users.AsEnumerable());

        public async Task<User?> GetUserById(int id) =>
            await _context.Users.FindAsync(id);

        public async Task<User> AddUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> UpdateUser(User user)
        {
            var existing = await _context.Users.FindAsync(user.UserId);
            if (existing == null) return null;

            existing.UserName = user.UserName;
            existing.Email = user.Email;
            existing.PasswordHash = user.PasswordHash;
            existing.Role = user.Role;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteUser(int id)
        {
            var u = await _context.Users.FindAsync(id);
            if (u == null) return false;
            _context.Users.Remove(u);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<IEnumerable<User>> SearchUsers(string keyword)
        {
            return await _context.Users
                .Where(u => u.UserName.Contains(keyword) || u.Email.Contains(keyword))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> FilterByRole(string role)
        {
            return await _context.Users
                .Where(u => u.Role == role)
                .AsNoTracking()
                .ToListAsync();
        }

    }
}
