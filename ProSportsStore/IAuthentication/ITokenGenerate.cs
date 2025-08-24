// IAuthentication/ITokenGenerate.cs
namespace ProSportsStore.IAuthentication
{
    public interface ITokenGenerate
    {
        string GenerateToken(string email, string role);
    }
}
