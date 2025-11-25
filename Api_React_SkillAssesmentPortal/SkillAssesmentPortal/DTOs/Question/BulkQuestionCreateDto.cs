using System.ComponentModel.DataAnnotations;

namespace SkillAssessmentPortal.DTOs.Question
{
    public class BulkQuestionCreateDto
    {
        [Required]
        public int TestId { get; set; }

        [Required]
        public int TestLevelId { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one question is required")]
        public List<QuestionCreateDto> Questions { get; set; } = new List<QuestionCreateDto>();
    }
}