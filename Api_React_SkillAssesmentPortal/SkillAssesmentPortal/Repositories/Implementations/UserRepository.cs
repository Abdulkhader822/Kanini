using Microsoft.EntityFrameworkCore;
using SkillAssessmentPortal.Models;
using SkillAssessmentPortal.Repositories.Interfaces;

namespace SkillAssessmentPortal.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly SkillAssessmentDbContext _context;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(SkillAssessmentDbContext context, ILogger<UserRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserId == id);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            _logger.LogDebug("Fetching user by email: {Email}", email?.Substring(0, Math.Min(3, email.Length)) + "***");
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);
            
            if (user != null)
            {
                _logger.LogDebug("User found - UserId: {UserId}, PasswordHashLength: {HashLength}", 
                    user.UserId, user.PasswordHash?.Length ?? 0);
            }
            
            return user;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<User> AddAsync(User user)
        {
            _logger.LogDebug("Adding new user to database: {Email}", user.Email?.Substring(0, Math.Min(3, user.Email.Length)) + "***");
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            _logger.LogInformation("User added successfully with UserId: {UserId}", user.UserId);
            return user;
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return;

            try
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Cannot delete user because of related data.", ex);
            }
        }
    }
}
