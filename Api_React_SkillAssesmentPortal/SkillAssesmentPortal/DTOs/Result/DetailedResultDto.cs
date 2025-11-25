namespace SkillAssessmentPortal.DTOs.Result
{
    public class DetailedResultDto
    {
        public int ResultId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int TestId { get; set; }
        public string TestName { get; set; } = string.Empty;
        public int TestLevelId { get; set; }
        public string LevelName { get; set; } = string.Empty;
        public decimal PassingScore { get; set; }
        public int Score { get; set; }
        public decimal Percentage { get; set; }
        public string ResultStatus { get; set; } = string.Empty;
        public DateTime DateAttempted { get; set; }
        public int AttemptNumber { get; set; }
        public string Suggestion { get; set; } = string.Empty;
        public List<QuestionResultDto> Questions { get; set; } = new List<QuestionResultDto>();
    }

    public class QuestionResultDto
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string OptionA { get; set; } = string.Empty;
        public string OptionB { get; set; } = string.Empty;
        public string OptionC { get; set; } = string.Empty;
        public string OptionD { get; set; } = string.Empty;
        public string CorrectOption { get; set; } = string.Empty;
        public string SelectedOption { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
    }
}