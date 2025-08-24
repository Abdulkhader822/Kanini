using ProSportsStore.Models;

namespace ProSportsStore.Interface
{
    public interface IToken
    {
        string GenerateToken(User user);
    }
}
