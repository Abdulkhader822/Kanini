namespace SkillAssessmentPortal.DTOs.Test
{
    public class TestResponseDto
    {
        public int TestId { get; set; }
        public string TestName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public int DurationMins { get; set; }
        public int TotalQuestions { get; set; }
        public int TotalMarks { get; set; }
        public decimal MarksPerQuestion { get; set; }
        public string CreatedByName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
