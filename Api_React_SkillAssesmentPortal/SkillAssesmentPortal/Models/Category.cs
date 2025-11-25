using System.ComponentModel.DataAnnotations;

namespace SkillAssessmentPortal.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required, MaxLength(100)]
        public string CategoryName { get; set; } = string.Empty;

        public string? Description { get; set; }

        //  Navigation
        public ICollection<Test>? Tests { get; set; }
    }
}
