using Microsoft.EntityFrameworkCore;
using SkillAssessmentPortal.Models;
using SkillAssessmentPortal.Repositories.Interfaces;

namespace SkillAssessmentPortal.Repositories.Implementations
{
    public class TestRepository : ITestRepository
    {
        private readonly SkillAssessmentDbContext _context;
        private readonly ILogger<TestRepository> _logger;

        public TestRepository(SkillAssessmentDbContext context, ILogger<TestRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Test>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all tests from database");
            try
            {
                var tests = await _context.Tests
                    .Include(t => t.Category)
                    .Include(t => t.AdminUser)
                    .AsNoTracking()
                    .ToListAsync();
                _logger.LogInformation("Successfully retrieved {Count} tests", tests.Count);
                return tests;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all tests");
                throw;
            }
        }

        public async Task<IEnumerable<Test>> GetAllWithIncludesAsync()
        {
            return await _context.Tests
                .Include(t => t.Category)
                .Include(t => t.AdminUser)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Test?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Fetching test by ID: {TestId}", id);
            try
            {
                var test = await _context.Tests
                    .Include(t => t.Category)
                    .Include(t => t.AdminUser)
                    .FirstOrDefaultAsync(t => t.TestId == id);
                if (test != null)
                    _logger.LogInformation("Test found for ID: {TestId}", id);
                else
                    _logger.LogWarning("Test not found for ID: {TestId}", id);
                return test;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching test by ID: {TestId}", id);
                throw;
            }
        }

        public async Task<Test?> GetByNameAsync(string testName)
        {
            _logger.LogInformation("Fetching test by name: {TestName}", testName);
            try
            {
                var test = await _context.Tests
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.TestName == testName);
                if (test != null)
                    _logger.LogInformation("Test found for name: {TestName}", testName);
                else
                    _logger.LogInformation("Test not found for name: {TestName}", testName);
                return test;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching test by name: {TestName}", testName);
                throw;
            }
        }

        public async Task<Test> AddAsync(Test test)
        {
            _logger.LogInformation("Adding new test: {TestName}", test.TestName);
            try
            {
                _context.Tests.Add(test);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Test added successfully with ID: {TestId}", test.TestId);
                return test;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding test: {TestName}", test.TestName);
                throw;
            }
        }

        public async Task UpdateAsync(Test test)
        {
            _logger.LogInformation("Updating test ID: {TestId}", test.TestId);
            try
            {
                _context.Tests.Update(test);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Test updated successfully for ID: {TestId}", test.TestId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating test ID: {TestId}", test.TestId);
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            _logger.LogInformation("Deleting test ID: {TestId}", id);
            try
            {
                var test = await _context.Tests.FindAsync(id);
                if (test != null)
                {
                    _context.Tests.Remove(test);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Test deleted successfully for ID: {TestId}", id);
                }
                else
                {
                    _logger.LogWarning("Test not found for deletion, ID: {TestId}", id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting test ID: {TestId}", id);
                throw;
            }
        }

        public async Task<TestLevel?> GetLevelByIdAsync(int testLevelId)
        {
            _logger.LogInformation("Fetching test level by ID: {TestLevelId}", testLevelId);
            try
            {
                var level = await _context.TestLevels.FindAsync(testLevelId);
                if (level != null)
                    _logger.LogInformation("Test level found for ID: {TestLevelId}", testLevelId);
                else
                    _logger.LogWarning("Test level not found for ID: {TestLevelId}", testLevelId);
                return level;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching test level by ID: {TestLevelId}", testLevelId);
                throw;
            }
        }

        public async Task<bool> HasResultsAsync(int testId)
        {
            return await _context.Results.AnyAsync(r => r.TestId == testId);
        }

        public async Task<bool> HasCertificatesAsync(int testId)
        {
            return await _context.Certificates.AnyAsync(c => c.TestId == testId);
        }

    }
}
