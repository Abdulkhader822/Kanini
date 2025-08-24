namespace ProSportsStore.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;   // <- not "Name"
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty; // <- keep this name
        public string Role { get; set; } = "Customer";
       
    }
}
