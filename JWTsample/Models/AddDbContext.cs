using Microsoft.EntityFrameworkCore;

namespace JWTsample.Models
{
    public class AddDbContext : DbContext
    {
        public AddDbContext(DbContextOptions<AddDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Passengers> Passengers { get; set; }
    }
}
