using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkillAssessmentPortal.Models
{
    public class TestLevel
    {
        [Key]
        public int TestLevelId { get; set; }

        [Required]
        public int TestId { get; set; }

        [Required, MaxLength(20)]
        public string LevelName { get; set; } = string.Empty; // Easy, Medium, Hard

        [Range(0, 100)]
        [Column(TypeName = "decimal(5,2)")]
        public decimal PassingScore { get; set; } = 60.00m;

        [MaxLength(255)]
        public string? VideoLink { get; set; } // YouTube link

        public int DurationMins { get; set; } = 0; // Duration allocated for this level

        // Navigation
        [ForeignKey("TestId")]
        public Test? Test { get; set; }

        public ICollection<Question>? Questions { get; set; }
        public ICollection<Result>? Results { get; set; }
    }
}
