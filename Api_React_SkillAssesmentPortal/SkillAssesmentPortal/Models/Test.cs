using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkillAssessmentPortal.Models
{
    public class Test
    {
        [Key]
        public int TestId { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required, MaxLength(150)]
        public string TestName { get; set; } = string.Empty;

        public int DurationMins { get; set; } = 30;

        [Range(15, 30)]
        public int TotalQuestions { get; set; } = 15;

        public int TotalMarks { get; set; } = 100;

        [Required]
        public int CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        //  Navigation
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        [ForeignKey("CreatedBy")]
        public User? AdminUser { get; set; }

        public ICollection<TestLevel>? Levels { get; set; }
        public ICollection<Result>? Results { get; set; }
        public ICollection<Certificate>? Certificates { get; set; }
    }
}
