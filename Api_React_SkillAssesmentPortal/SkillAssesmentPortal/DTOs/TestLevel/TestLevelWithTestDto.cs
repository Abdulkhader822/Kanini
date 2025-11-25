namespace SkillAssessmentPortal.DTOs.TestLevel
{
    public class TestLevelWithTestDto
    {
        public int TestLevelId { get; set; }
        public int TestId { get; set; }
        public string TestName { get; set; } = string.Empty;
        public string LevelName { get; set; } = string.Empty;
        public decimal PassingScore { get; set; }
        public string VideoLink { get; set; } = string.Empty;
        public int DurationMins { get; set; }
    }
}