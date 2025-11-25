using System.ComponentModel.DataAnnotations;
using SkillAssessmentPortal.Models.Enums;

namespace SkillAssessmentPortal.Models

{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public RoleType Role { get; set; } = RoleType.User;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        //  Navigation Properties
        public ICollection<Result>? Results { get; set; }
        public ICollection<Certificate>? Certificates { get; set; }
        public ICollection<Test>? CreatedTests { get; set; } // Admin created tests
    }
}
