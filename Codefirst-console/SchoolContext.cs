using Codefirst_console;
using Microsoft.EntityFrameworkCore;

namespace Codefirst_console
{
    public class SchoolContext : DbContext
    {
        public DbSet<Student> Students { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Different database name
            optionsBuilder.UseSqlServer(
                @"Server=ABDULKHADER67;Database=SchoolDB_CodeFirst;Trusted_Connection=True;TrustServerCertificate=True;");
        }
    }
}
