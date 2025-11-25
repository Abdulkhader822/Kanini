using System.ComponentModel.DataAnnotations;

namespace SkillAssessmentPortal.DTOs.Category
{
    public class CategoryUpdateDto
    {
        [Required(ErrorMessage = "Category Name is required")]
        [MaxLength(100)]
        public string CategoryName { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Description { get; set; }
    }
}
