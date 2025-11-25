namespace SkillAssessmentPortal.DTOs.Result
{
    public class ResultResponseDto
    {
        public int ResultId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;

        public int TestId { get; set; }
        public string TestName { get; set; } = string.Empty;

        public int TestLevelId { get; set; }
        public string LevelName { get; set; } = string.Empty;
        public decimal PassingScore { get; set; }

        public int Score { get; set; }              // Marks scored
        public decimal Percentage { get; set; }     // Percentage

        public string ResultStatus { get; set; } = string.Empty;  // Pass/Fail
        public string Suggestion { get; set; } = string.Empty;    // Feedback message
        public DateTime DateAttempted { get; set; }

        // ✅ Whether all levels (Easy, Medium, Hard) are cleared
        public bool IsFinalLevelCleared { get; set; } = false;
        
        public int AttemptNumber { get; set; }
        public bool IsReattempt { get; set; }
        public bool HasCertificate { get; set; }
    }
}
