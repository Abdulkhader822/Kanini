namespace ProSportsStore.DTOs
{
    public class LoginDTO
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;  // client sends "password"
        public string? Role { get; set; }
    }
}
