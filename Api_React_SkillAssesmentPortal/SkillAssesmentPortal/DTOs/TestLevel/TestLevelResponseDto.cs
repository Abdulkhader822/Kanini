namespace SkillAssessmentPortal.DTOs.TestLevel
{
    public class TestLevelResponseDto
    {
        public int TestLevelId { get; set; }
        public int TestId { get; set; }
        public string LevelName { get; set; } = string.Empty;
        public decimal PassingScore { get; set; }
        public string VideoLink { get; set; } = string.Empty;
        public int DurationMins { get; set; }
    }
}
