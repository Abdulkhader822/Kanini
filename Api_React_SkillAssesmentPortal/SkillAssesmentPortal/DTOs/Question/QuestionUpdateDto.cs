using System.ComponentModel.DataAnnotations;

namespace SkillAssessmentPortal.DTOs.Question
{
    public class QuestionUpdateDto
    {
        [Required]
        public int QuestionId { get; set; }

        [Required]
        public string QuestionText { get; set; } = string.Empty;

        [Required]
        public string OptionA { get; set; } = string.Empty;

        [Required]
        public string OptionB { get; set; } = string.Empty;

        [Required]
        public string OptionC { get; set; } = string.Empty;

        [Required]
        public string OptionD { get; set; } = string.Empty;

        [Required]
        [StringLength(1)]
        public string CorrectOption { get; set; } = string.Empty;
    }
}