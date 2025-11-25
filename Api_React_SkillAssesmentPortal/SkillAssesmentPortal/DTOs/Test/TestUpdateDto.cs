using System.ComponentModel.DataAnnotations;

namespace SkillAssessmentPortal.DTOs.Test
{
    public class TestUpdateDto
    {
        [Required, MaxLength(150)]
        public string TestName { get; set; } = string.Empty;

        [Range(15, 30)]
        public int TotalQuestions { get; set; }

        [Range(10, 180)]
        public int DurationMins { get; set; }
    }
}
