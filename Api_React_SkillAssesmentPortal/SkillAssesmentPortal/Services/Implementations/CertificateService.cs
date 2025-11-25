using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using SkillAssessmentPortal.Models;
using SkillAssessmentPortal.Repositories.Interfaces;
using SkillAssessmentPortal.Services.Interfaces;

namespace SkillAssessmentPortal.Services.Implementations
{
    public class CertificateService : ICertificateService
    {
        private readonly ICertificateRepository _certificateRepo;
        private readonly ILogger<CertificateService> _logger;

        public CertificateService(ICertificateRepository certificateRepo, ILogger<CertificateService> logger)
        {
            _certificateRepo = certificateRepo;
            _logger = logger;
        }

        public async Task<string> GenerateCertificateAsync(int userId, int testId, string userName, string testName, int score, decimal percentage)
        {
            _logger.LogInformation("Generating certificate for UserId: {UserId}, TestId: {TestId}, Score: {Score}, Percentage: {Percentage}%", 
                userId, testId, score, percentage);
            
            // Check if certificate already exists
            var existingCert = await _certificateRepo.GetByUserAndTestAsync(userId, testId);
            if (existingCert != null)
            {
                _logger.LogWarning("Certificate already exists for UserId: {UserId}, TestId: {TestId}. Returning existing path.", userId, testId);
                return existingCert.CertificateURL;
            }
            
            string certDir = Path.Combine(Directory.GetCurrentDirectory(), "Certificates");
            if (!Directory.Exists(certDir))
            {
                Directory.CreateDirectory(certDir);
                _logger.LogDebug("Created Certificates directory: {Directory}", certDir);
            }

            string fileName = $"{userName.Replace(" ", "_")}_{testName.Replace(" ", "_")}_Certificate.pdf";
            string filePath = Path.Combine(certDir, fileName);

            using (var writer = new PdfWriter(filePath))
            using (var pdf = new PdfDocument(writer))
            using (var doc = new iText.Layout.Document(pdf))
            {
                doc.SetMargins(80, 60, 80, 60);

                // Professional Header
                var title = new Paragraph("Certificate of Achievement")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(32)
                    .SetBold()
                    .SetFontColor(new DeviceRgb(21, 101, 192))
                    .SetMarginBottom(30);
                doc.Add(title);

                // Recipient Section
                doc.Add(new Paragraph("This certifies that")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(16)
                    .SetMarginBottom(10));

                doc.Add(new Paragraph(userName)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(24)
                    .SetBold()
                    .SetFontColor(new DeviceRgb(25, 118, 210))
                    .SetMarginBottom(20));

                // Test Information
                doc.Add(new Paragraph($"has successfully completed the")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(16)
                    .SetMarginBottom(10));

                doc.Add(new Paragraph($"'{testName}'")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(20)
                    .SetBold()
                    .SetMarginBottom(25));

                // Score Section
                doc.Add(new Paragraph($"Score: {score} / 100 ({percentage}%)")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(16)
                    .SetFontColor(new DeviceRgb(46, 125, 50))
                    .SetBold()
                    .SetMarginBottom(30));

                // Issue Date
                doc.Add(new Paragraph($"Issued Date: {DateTime.Now:MMMM dd, yyyy}")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(14)
                    .SetMarginBottom(60));

                // Digital Signature Section
                doc.Add(new Paragraph("Digital Signature")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(12)
                    .SetFontColor(ColorConstants.GRAY)
                    .SetMarginBottom(10));

                doc.Add(new Paragraph("Skill Assessment Portal")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(18)
                    .SetBold()
                    .SetFontColor(new DeviceRgb(21, 101, 192)));
            }

            await _certificateRepo.AddAsync(new Certificate
            {
                UserId = userId,
                TestId = testId,
                IssueDate = DateTime.Now,
                CertificateURL = filePath
            });

            _logger.LogInformation("Certificate successfully generated and saved: {FilePath}", filePath);
            return filePath;
        }

        public async Task<byte[]?> DownloadCertificateAsync(int userId, int testId)
        {
            var cert = await _certificateRepo.GetByUserAndTestAsync(userId, testId);
            if (cert == null || !File.Exists(cert.CertificateURL))
                return null;

            return await File.ReadAllBytesAsync(cert.CertificateURL);
        }

        public async Task<IEnumerable<object>> GetUserCertificatesAsync(int userId)
        {
            _logger.LogInformation("Fetching certificates for UserId: {UserId}", userId);
            try
            {
                var certificates = await _certificateRepo.GetByUserIdAsync(userId);
                return certificates.Select(c => new
                {
                    certificateId = c.CertificateId,
                    testId = c.TestId,
                    testName = c.Test?.TestName ?? "Unknown Test",
                    issueDate = c.IssueDate,
                    certificateUrl = c.CertificateURL
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching certificates for UserId: {UserId}", userId);
                throw;
            }
        }
    }
}
