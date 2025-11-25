using SkillAssessmentPortal.DTOs.Question;

namespace SkillAssessmentPortal.DTOs.Result
{
    public class ResultCreateDto
    {
        public int UserId { get; set; }
        public int TestId { get; set; }
        public int TestLevelId { get; set; }

        // ✅ Total time taken by the user (in seconds)
        public int TimeTakenSecs { get; set; }

        // ✅ List of answers submitted by the user
        public List<QuestionAnswerDto> Answers { get; set; } = new List<QuestionAnswerDto>();

        // ✅ Optional: When user started this level attempt (UTC)
        public DateTime? StartedAtUtc { get; set; }
    }
}
