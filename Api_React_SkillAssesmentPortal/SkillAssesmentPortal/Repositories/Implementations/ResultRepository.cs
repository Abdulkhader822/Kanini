using Microsoft.EntityFrameworkCore;
using SkillAssessmentPortal.Models;
using SkillAssessmentPortal.Repositories.Interfaces;

namespace SkillAssessmentPortal.Repositories.Implementations
{
    public class ResultRepository : GenericRepository<Result>, IResultRepository
    {
        private new readonly ILogger<ResultRepository> _logger;

        public ResultRepository(SkillAssessmentDbContext context, ILogger<ResultRepository> logger) : base(context, logger) 
        {
            _logger = logger;
        }

        public async Task<int> GetNextAttemptNumberAsync(int userId, int testId)
        {
            _logger.LogInformation("Getting next attempt number for UserId: {UserId}, TestId: {TestId}", userId, testId);
            try
            {
                var last = await _context.Results
                    .Where(r => r.UserId == userId && r.TestId == testId)
                    .OrderByDescending(r => r.AttemptNumber)
                    .FirstOrDefaultAsync();
                var nextAttempt = last == null ? 1 : last.AttemptNumber + 1;
                _logger.LogInformation("Next attempt number for UserId: {UserId}, TestId: {TestId} is {AttemptNumber}", userId, testId, nextAttempt);
                return nextAttempt;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting next attempt number for UserId: {UserId}, TestId: {TestId}", userId, testId);
                throw;
            }
        }

        public async Task<bool> HasUserPassedLevelAsync(int userId, int testId, string levelName)
        {
            _logger.LogInformation("Checking if user passed level - UserId: {UserId}, TestId: {TestId}, Level: {LevelName}", userId, testId, levelName);
            try
            {
                var hasPassed = await _context.Results
                    .AnyAsync(r => r.UserId == userId &&
                                   r.TestId == testId &&
                                   r.TestLevel.LevelName == levelName &&
                                   r.ResultStatus == "Pass");
                _logger.LogInformation("User {UserId} has {Status} level {LevelName} for TestId: {TestId}", userId, hasPassed ? "passed" : "not passed", levelName, testId);
                return hasPassed;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if user passed level - UserId: {UserId}, TestId: {TestId}, Level: {LevelName}", userId, testId, levelName);
                throw;
            }
        }

        public async Task<bool> HasUserPassedAllLevelsAsync(int userId, int testId)
        {
            _logger.LogInformation("Checking if user passed all levels - UserId: {UserId}, TestId: {TestId}", userId, testId);
            try
            {
                var levels = await _context.TestLevels
                    .Where(tl => tl.TestId == testId)
                    .Select(tl => tl.LevelName)
                    .ToListAsync();
                
                _logger.LogDebug("Found {LevelCount} levels for TestId: {TestId}", levels.Count, testId);

                foreach (var level in levels)
                {
                    bool passed = await _context.Results.AnyAsync(r =>
                        r.UserId == userId &&
                        r.TestId == testId &&
                        r.TestLevel.LevelName == level &&
                        r.ResultStatus == "Pass");
                    if (!passed) 
                    {
                        _logger.LogInformation("User {UserId} has not passed level {LevelName} for TestId: {TestId}", userId, level, testId);
                        return false;
                    }
                }
                _logger.LogInformation("User {UserId} has passed all levels for TestId: {TestId}", userId, testId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if user passed all levels - UserId: {UserId}, TestId: {TestId}", userId, testId);
                throw;
            }
        }

        public async Task<IEnumerable<Result>> GetResultsByUserAndTestAsync(int userId, int testId)
        {
            _logger.LogInformation("Fetching results for UserId: {UserId}, TestId: {TestId}", userId, testId);
            try
            {
                var results = await _context.Results
                    .Include(r => r.TestLevel)
                    .Where(r => r.UserId == userId && r.TestId == testId)
                    .ToListAsync();
                _logger.LogInformation("Retrieved {Count} results for UserId: {UserId}, TestId: {TestId}", results.Count, userId, testId);
                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching results for UserId: {UserId}, TestId: {TestId}", userId, testId);
                throw;
            }
        }

        public async Task<int> GetAttemptCountAsync(int userId, int testId)
        {
            _logger.LogInformation("Getting attempt count for UserId: {UserId}, TestId: {TestId}", userId, testId);
            try
            {
                var count = await _context.Results
                    .Where(r => r.UserId == userId && r.TestId == testId)
                    .CountAsync();
                _logger.LogInformation("Found {Count} attempts for UserId: {UserId}, TestId: {TestId}", count, userId, testId);
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting attempt count for UserId: {UserId}, TestId: {TestId}", userId, testId);
                throw;
            }
        }

        public async Task<DateTime?> GetLastAttemptDateAsync(int userId, int testId)
        {
            _logger.LogInformation("Getting last attempt date for UserId: {UserId}, TestId: {TestId}", userId, testId);
            try
            {
                var lastAttempt = await _context.Results
                    .Where(r => r.UserId == userId && r.TestId == testId)
                    .OrderByDescending(r => r.DateAttempted)
                    .FirstOrDefaultAsync();
                var lastDate = lastAttempt?.DateAttempted;
                _logger.LogInformation("Last attempt date for UserId: {UserId}, TestId: {TestId}: {LastDate}", userId, testId, lastDate);
                return lastDate;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting last attempt date for UserId: {UserId}, TestId: {TestId}", userId, testId);
                throw;
            }
        }

        public async Task<DateTime?> GetLastAttemptDateForLevelAsync(int userId, int testLevelId)
        {
            _logger.LogInformation("Getting last attempt date for UserId: {UserId}, TestLevelId: {TestLevelId}", userId, testLevelId);
            try
            {
                var lastAttempt = await _context.Results
                    .Where(r => r.UserId == userId && r.TestLevelId == testLevelId)
                    .OrderByDescending(r => r.DateAttempted)
                    .FirstOrDefaultAsync();
                var lastDate = lastAttempt?.DateAttempted;
                _logger.LogInformation("Last attempt date for UserId: {UserId}, TestLevelId: {TestLevelId}: {LastDate}", userId, testLevelId, lastDate);
                return lastDate;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting last attempt date for UserId: {UserId}, TestLevelId: {TestLevelId}", userId, testLevelId);
                throw;
            }
        }



        public async Task<IEnumerable<string>> GetCompletedLevelsAsync(int userId, int testId)
        {
            _logger.LogInformation("Getting completed levels for UserId: {UserId}, TestId: {TestId}", userId, testId);
            try
            {
                // Debug: Check all results for this user and test
                var allResults = await _context.Results
                    .Include(r => r.TestLevel)
                    .Where(r => r.UserId == userId && r.TestId == testId)
                    .ToListAsync();
                
                _logger.LogInformation("DEBUG: Found {Count} total results for UserId: {UserId}, TestId: {TestId}", allResults.Count, userId, testId);
                foreach (var result in allResults)
                {
                    _logger.LogInformation("DEBUG: Result - Level: {Level}, Status: {Status}, Score: {Score}, Percentage: {Percentage}", 
                        result.TestLevel?.LevelName ?? "NULL", result.ResultStatus, result.Score, result.Percentage);
                }
                
                var completedLevels = await _context.Results
                    .Include(r => r.TestLevel)
                    .Where(r => r.UserId == userId && r.TestId == testId && r.ResultStatus == "Pass")
                    .Select(r => r.TestLevel.LevelName)
                    .Distinct()
                    .ToListAsync();
                
                _logger.LogInformation("Found {Count} completed levels for UserId: {UserId}, TestId: {TestId}: {Levels}", 
                    completedLevels.Count, userId, testId, string.Join(", ", completedLevels));
                return completedLevels;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting completed levels for UserId: {UserId}, TestId: {TestId}", userId, testId);
                throw;
            }
        }

        public async Task AddUserAnswerAsync(UserAnswer userAnswer)
        {
            _logger.LogInformation("Adding user answer for ResultId: {ResultId}, QuestionId: {QuestionId}", userAnswer.ResultId, userAnswer.QuestionId);
            try
            {
                await _context.UserAnswers.AddAsync(userAnswer);
                await _context.SaveChangesAsync();
                _logger.LogInformation("User answer added successfully for ResultId: {ResultId}, QuestionId: {QuestionId}", userAnswer.ResultId, userAnswer.QuestionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding user answer for ResultId: {ResultId}, QuestionId: {QuestionId}", userAnswer.ResultId, userAnswer.QuestionId);
                throw;
            }
        }

        public async Task<IEnumerable<UserAnswer>> GetUserAnswersByResultIdAsync(int resultId)
        {
            _logger.LogInformation("Getting user answers for ResultId: {ResultId}", resultId);
            try
            {
                var userAnswers = await _context.UserAnswers
                    .Include(ua => ua.Question)
                    .Where(ua => ua.ResultId == resultId)
                    .ToListAsync();
                _logger.LogInformation("Retrieved {Count} user answers for ResultId: {ResultId}", userAnswers.Count, resultId);
                return userAnswers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user answers for ResultId: {ResultId}", resultId);
                throw;
            }
        }
    }
}
