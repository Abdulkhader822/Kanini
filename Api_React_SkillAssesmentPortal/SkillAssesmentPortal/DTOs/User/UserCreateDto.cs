using System.ComponentModel.DataAnnotations;

namespace SkillAssessmentPortal.DTOs.User
{
    public class UserCreateDto
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role is required")]
        [RegularExpression("^(Admin|User)$", ErrorMessage = "Role must be 'Admin' or 'User'")]
        public string Role { get; set; } = "User";
    }
}
