using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkillAssessmentPortal.Models
{
    public class UserAnswer
    {
        [Key]
        public int UserAnswerId { get; set; }

        [Required]
        public int ResultId { get; set; }

        [Required]
        public int QuestionId { get; set; }

        [Required]
        [StringLength(1)]
        public string SelectedOption { get; set; } = string.Empty; // A, B, C, D

        [Required]
        [StringLength(1)]
        public string CorrectOption { get; set; } = string.Empty; // A, B, C, D

        public bool IsCorrect { get; set; }

        // Navigation properties
        [ForeignKey("ResultId")]
        public Result? Result { get; set; }

        [ForeignKey("QuestionId")]
        public Question? Question { get; set; }
    }
}