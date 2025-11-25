using System.ComponentModel.DataAnnotations;

namespace SkillAssessmentPortal.DTOs.Test
{
    public class TestCreateDto
    {
        [Required]
        public int CategoryId { get; set; }

        [Required, MaxLength(150)]
        public string TestName { get; set; } = string.Empty;

        [Range(15, 30, ErrorMessage = "Test must contain between 15 to 30 questions.")]
        public int TotalQuestions { get; set; }

        [Range(10, 180, ErrorMessage = "Duration should be between 10 and 180 minutes.")]
        public int DurationMins { get; set; }

        [Required]
        public int CreatedBy { get; set; } // Admin user
    }
}
