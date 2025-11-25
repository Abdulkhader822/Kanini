using SkillAssessmentPortal.DTOs.User;
using SkillAssessmentPortal.Models;
using SkillAssessmentPortal.Repositories.Interfaces;
using SkillAssessmentPortal.Services.Interfaces;
using SkillAssessmentPortal.Models.Enums;
using BCrypt.Net;
using System.Text.RegularExpressions;

namespace SkillAssessmentPortal.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        //  Get all users
        public async Task<IEnumerable<UserResponseDto>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();

            return users.Select(u => new UserResponseDto
            {
                UserId = u.UserId,
                Name = u.Name,
                Email = u.Email,
                Role = u.Role.ToString(),
                CreatedAt = u.CreatedAt
            });
        }

        //  Get user by ID
        public async Task<UserResponseDto?> GetByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return null;

            return new UserResponseDto
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role.ToString(),
                CreatedAt = user.CreatedAt
            };
        }

        //  Create (Register) user with validation
        public async Task<(bool IsSuccess, string Message)> CreateAsync(UserCreateDto dto)
        {
            if (string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Password) || string.IsNullOrEmpty(dto.Name))
                return (false, "All fields are required.");
                
            _logger.LogInformation("Creating new user with email: {Email}", dto.Email.Substring(0, Math.Min(3, dto.Email.Length)) + "***");
            
            //  Check if email already exists
            var exists = await _userRepository.GetByEmailAsync(dto.Email);
            if (exists != null)
            {
                _logger.LogWarning("User creation failed - Email already exists: {Email}", dto.Email.Substring(0, Math.Min(3, dto.Email.Length)) + "***");
                return (false, "Email already registered. Please log in instead.");
            }

            //  Validate email format
            if (!IsValidEmail(dto.Email))
                return (false, "Invalid email format. Please enter a valid email (e.g., user@example.com)");

            //  Validate password strength
            if (!IsValidPassword(dto.Password))
                return (false, "Password must be 6–8 characters long and contain both letters and numbers (no special characters).");

            //  Parse Role (Enum conversion)
            RoleType parsedRole;
            if (!Enum.TryParse(dto.Role ?? "User", true, out parsedRole))
                parsedRole = RoleType.User;

            //  Hash password before storing
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            // 6 Create user object
            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = hashedPassword,
                Role = parsedRole,
                CreatedAt = DateTime.Now
            };

            await _userRepository.AddAsync(user);
            _logger.LogInformation("User created successfully with UserId: {UserId}, Role: {Role}", user.UserId, user.Role);
            return (true, "Registration successful.");
        }

        //  Update user info
        public async Task<string> UpdateAsync(int id, UserUpdateDto dto)
        {
            if (string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Name))
                return "Name and email are required.";
                
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return "User not found.";

            if (!IsValidEmail(dto.Email))
                return "Invalid email format.";

            user.Name = dto.Name;
            user.Email = dto.Email;

            RoleType parsedRole;
            if (!Enum.TryParse(dto.Role ?? "User", true, out parsedRole))
                parsedRole = RoleType.User;

            user.Role = parsedRole;

            await _userRepository.UpdateAsync(user);
            return "User updated successfully.";
        }

        //  Delete user
        public async Task<string> DeleteAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return "User not found.";

            await _userRepository.DeleteAsync(id);
            return "User deleted successfully.";
        }

     

        //  Validate email format
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                // Must contain domain part like .com, .in, etc.
                return addr.Address == email && email.Contains(".");
            }
            catch
            {
                return false;
            }
        }

        //  Change password
        public async Task<(bool IsSuccess, string Message)> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            _logger.LogInformation("Changing password for user ID: {UserId}", userId);
            
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Password change failed - User not found: {UserId}", userId);
                return (false, "User not found.");
            }

            // Verify current password
            if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
            {
                _logger.LogWarning("Password change failed - Invalid current password for user: {UserId}", userId);
                return (false, "Current password is incorrect.");
            }

            // Validate new password
            if (!IsValidPassword(newPassword))
            {
                return (false, "New password must be 6–8 characters long and contain both letters and numbers.");
            }

            // Hash and update new password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _userRepository.UpdateAsync(user);
            
            _logger.LogInformation("Password changed successfully for user ID: {UserId}", userId);
            return (true, "Password changed successfully.");
        }

        //  Validate password strength (6–8 chars, letters + digits only)
        private bool IsValidPassword(string password)
        {
            var regex = new Regex(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{6,8}$");
            return regex.IsMatch(password);
        }
    }
}
