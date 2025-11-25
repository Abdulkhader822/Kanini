using Microsoft.EntityFrameworkCore;
using SkillAssessmentPortal.Models;
using SkillAssessmentPortal.Repositories.Interfaces;

namespace SkillAssessmentPortal.Repositories.Implementations
{
    public class CertificateRepository : GenericRepository<Certificate>, ICertificateRepository
    {
        private new readonly ILogger<CertificateRepository> _logger;

        public CertificateRepository(SkillAssessmentDbContext context, ILogger<CertificateRepository> logger) : base(context, logger) 
        {
            _logger = logger;
        }

        public async Task<Certificate?> GetByUserAndTestAsync(int userId, int testId)
        {
            _logger.LogInformation("Fetching certificate for UserId: {UserId}, TestId: {TestId}", userId, testId);
            try
            {
                var certificate = await _context.Certificates
                    .FirstOrDefaultAsync(c => c.UserId == userId && c.TestId == testId);
                if (certificate != null)
                    _logger.LogInformation("Certificate found for UserId: {UserId}, TestId: {TestId}", userId, testId);
                else
                    _logger.LogInformation("No certificate found for UserId: {UserId}, TestId: {TestId}", userId, testId);
                return certificate;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching certificate for UserId: {UserId}, TestId: {TestId}", userId, testId);
                throw;
            }
        }

        public async Task<IEnumerable<Certificate>> GetByUserIdAsync(int userId)
        {
            _logger.LogInformation("Fetching all certificates for UserId: {UserId}", userId);
            try
            {
                var certificates = await _context.Certificates
                    .Include(c => c.Test)
                    .Where(c => c.UserId == userId)
                    .ToListAsync();
                _logger.LogInformation("Found {Count} certificates for UserId: {UserId}", certificates.Count, userId);
                return certificates;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching certificates for UserId: {UserId}", userId);
                throw;
            }
        }
    }
}
