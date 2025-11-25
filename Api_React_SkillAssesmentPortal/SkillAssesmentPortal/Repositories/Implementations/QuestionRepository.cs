using Microsoft.EntityFrameworkCore;
using SkillAssessmentPortal.Models;
using SkillAssessmentPortal.Repositories.Interfaces;

namespace SkillAssessmentPortal.Repositories.Implementations
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly SkillAssessmentDbContext _context;
        private readonly ILogger<QuestionRepository> _logger;

        public QuestionRepository(SkillAssessmentDbContext context, ILogger<QuestionRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Question>> GetByTestLevelAsync(int testLevelId)
        {
            _logger.LogDebug("Fetching questions for TestLevelId: {TestLevelId}", testLevelId);
            var questions = await _context.Questions
                .Where(q => q.TestLevelId == testLevelId)
                .ToListAsync();
            _logger.LogDebug("Retrieved {Count} questions for TestLevelId: {TestLevelId}", questions.Count, testLevelId);
            return questions;
        }

        public async Task<Question?> GetByIdAsync(int id)
        {
            return await _context.Questions.FindAsync(id);
        }

        public async Task AddAsync(Question question)
        {
            _context.Questions.Add(question);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Question question)
        {
            _context.Questions.Update(question);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question != null)
            {
                _context.Questions.Remove(question);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> CountByTestLevelAsync(int testLevelId)
        {
            return await _context.Questions
                .Where(q => q.TestLevelId == testLevelId)
                .CountAsync();
        }

        public async Task<int> CountByTestIdAsync(int testId)
        {
            return await _context.Questions
                .Include(q => q.TestLevel)
                .Where(q => q.TestLevel!.TestId == testId)
                .CountAsync();
        }

        public async Task<Dictionary<int, int>> GetQuestionCountsByTestLevelsAsync(IEnumerable<int> testLevelIds)
        {
            return await _context.Questions
                .Where(q => testLevelIds.Contains(q.TestLevelId))
                .GroupBy(q => q.TestLevelId)
                .ToDictionaryAsync(g => g.Key, g => g.Count());
        }
    }
}
