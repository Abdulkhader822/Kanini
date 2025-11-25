using System.Threading.Tasks;
using SkillAssessmentPortal.Models;

namespace SkillAssessmentPortal.Repositories.Interfaces
{
    public interface ICertificateRepository : IGenericRepository<Certificate>
    {
        Task<Certificate?> GetByUserAndTestAsync(int userId, int testId);
        Task<IEnumerable<Certificate>> GetByUserIdAsync(int userId);
    }
}
