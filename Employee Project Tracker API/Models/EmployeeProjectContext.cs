using Employee_Project_Tracker_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Employee_Project_Tracker_API.Models
{
    public class EmployeeProjectContext : DbContext
    {
        public EmployeeProjectContext(DbContextOptions<EmployeeProjectContext> options) : base(options) { }

        public DbSet<Project> Projects { get; set; }
        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>()
                .HasIndex(p => p.ProjectCode)
                .IsUnique();

            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.EmployeeCode)
                .IsUnique();

            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.Email)
                .IsUnique();

            // One-to-Many Relationship
            modelBuilder.Entity<Project>()
                .HasMany(p => p.Employees)
                .WithOne(e => e.Project)
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
