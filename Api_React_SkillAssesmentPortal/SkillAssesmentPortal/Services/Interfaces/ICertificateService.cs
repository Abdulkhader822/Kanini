namespace SkillAssessmentPortal.Services.Interfaces
{
    public interface ICertificateService
    {
        Task<string> GenerateCertificateAsync(int userId, int testId, string userName, string testName, int score, decimal percentage);
        Task<byte[]?> DownloadCertificateAsync(int userId, int testId);
        Task<IEnumerable<object>> GetUserCertificatesAsync(int userId);
    }
}
