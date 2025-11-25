using SkillAssessmentPortal.DTOs.Question;

namespace SkillAssessmentPortal.Services.Interfaces
{
    public interface IQuestionService
    {
        Task<IEnumerable<QuestionResponseDto>> GetQuestionsByLevelAsync(int testLevelId);
        Task<string> AddQuestionAsync(QuestionCreateDto dto);
        Task<string> AddBulkQuestionsAsync(BulkQuestionCreateDto dto);
        Task<(bool Success, string Message, QuestionResponseDto? Question)> UpdateQuestionAsync(QuestionUpdateDto dto);
        Task<string> DeleteQuestionAsync(int id);
        Task<(int Score, int TotalMarks)> EvaluateAnswersAsync(int testId, int testLevelId, List<QuestionAnswerDto> answers);

        Task<(bool Allowed, string Message, IEnumerable<QuestionResponseDto>? Questions)>
            GetQuestionsForUserAsync(int testId, int testLevelId, int userId);

    }
}
