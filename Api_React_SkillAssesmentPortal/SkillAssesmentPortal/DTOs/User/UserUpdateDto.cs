using System.ComponentModel.DataAnnotations;

namespace SkillAssessmentPortal.DTOs.User
{
    public class UserUpdateDto
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role is required")]
        [RegularExpression("^(Admin|User)$", ErrorMessage = "Role must be 'Admin' or 'User'")]
        public string Role { get; set; } = "User";
    }
}
