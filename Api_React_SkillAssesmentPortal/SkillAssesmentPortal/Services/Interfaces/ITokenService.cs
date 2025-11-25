using SkillAssessmentPortal.Models;

namespace SkillAssessmentPortal.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}