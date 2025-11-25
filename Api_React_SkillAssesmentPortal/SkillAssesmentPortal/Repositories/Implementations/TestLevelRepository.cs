using Microsoft.EntityFrameworkCore;
using SkillAssessmentPortal.Models;
using SkillAssessmentPortal.Repositories.Interfaces;

namespace SkillAssessmentPortal.Repositories.Implementations
{
    public class TestLevelRepository : ITestLevelRepository
    {
        private readonly SkillAssessmentDbContext _context;
        private readonly ILogger<TestLevelRepository> _logger;

        public TestLevelRepository(SkillAssessmentDbContext context, ILogger<TestLevelRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<TestLevel>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all test levels from database");
            try
            {
                var levels = await _context.TestLevels.Include(tl => tl.Test).ToListAsync();
                _logger.LogInformation("Successfully retrieved {Count} test levels from database", levels.Count);
                return levels;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching test levels from database");
                throw;
            }
        }

        public async Task<TestLevel?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Fetching test level by ID: {TestLevelId}", id);
            try
            {
                var level = await _context.TestLevels.Include(tl => tl.Test)
                                                .FirstOrDefaultAsync(tl => tl.TestLevelId == id);
                if (level != null)
                    _logger.LogInformation("Test level found for ID: {TestLevelId}", id);
                else
                    _logger.LogWarning("Test level not found for ID: {TestLevelId}", id);
                return level;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching test level by ID: {TestLevelId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<TestLevel>> GetByTestIdAsync(int testId)
        {
            _logger.LogInformation("Fetching test levels for TestId: {TestId}", testId);
            try
            {
                var levels = await _context.TestLevels
                    .Where(tl => tl.TestId == testId)
                    .Include(tl => tl.Questions)
                    .ToListAsync();
                _logger.LogInformation("Retrieved {Count} test levels for TestId: {TestId}", levels.Count, testId);
                return levels;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching test levels for TestId: {TestId}", testId);
                throw;
            }
        }

        public async Task<IEnumerable<TestLevel>> GetByTestIdsAsync(IEnumerable<int> testIds)
        {
            return await _context.TestLevels
                .Where(tl => testIds.Contains(tl.TestId))
                .ToListAsync();
        }

        public async Task<int> GetTotalDurationByTestIdAsync(int testId)
        {
            _logger.LogInformation("Calculating total duration for TestId: {TestId}", testId);
            try
            {
                var totalDuration = await _context.TestLevels
                    .Where(tl => tl.TestId == testId)
                    .SumAsync(tl => tl.DurationMins);
                _logger.LogInformation("Total duration for TestId: {TestId} is {Duration} minutes", testId, totalDuration);
                return totalDuration;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating total duration for TestId: {TestId}", testId);
                throw;
            }
        }

        public async Task AddAsync(TestLevel level)
        {
            _logger.LogInformation("Adding new test level for TestId: {TestId}, Level: {LevelName}", level.TestId, level.LevelName);
            try
            {
                _context.TestLevels.Add(level);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Test level added successfully with ID: {TestLevelId}", level.TestLevelId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding test level for TestId: {TestId}", level.TestId);
                throw;
            }
        }

        public async Task UpdateAsync(TestLevel level)
        {
            _logger.LogInformation("Updating test level ID: {TestLevelId}", level.TestLevelId);
            try
            {
                _context.TestLevels.Update(level);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Test level updated successfully for ID: {TestLevelId}", level.TestLevelId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating test level ID: {TestLevelId}", level.TestLevelId);
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            _logger.LogInformation("Deleting test level ID: {TestLevelId}", id);
            try
            {
                var level = await _context.TestLevels.FindAsync(id);
                if (level != null)
                {
                    _context.TestLevels.Remove(level);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Test level deleted successfully for ID: {TestLevelId}", id);
                }
                else
                {
                    _logger.LogWarning("Test level not found for deletion, ID: {TestLevelId}", id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting test level ID: {TestLevelId}", id);
                throw;
            }
        }

        public async Task<bool> HasUserCompletedTestAsync(int testLevelId)
        {
            var level = await _context.TestLevels.FindAsync(testLevelId);
            if (level == null) return false;
            
            return await _context.Certificates.AnyAsync(c => c.TestId == level.TestId);
        }

        public async Task<Result?> GetLastFailedAttemptAsync(int testLevelId)
        {
            return await _context.Results
                .Where(r => r.TestLevelId == testLevelId && r.ResultStatus == "Fail")
                .OrderByDescending(r => r.DateAttempted)
                .FirstOrDefaultAsync();
        }
    }
}
