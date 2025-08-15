using Microsoft.EntityFrameworkCore;

namespace CodeFirst_Assignment.Models
{
    public class BookReviewContext : DbContext
    {
       public DbSet<BookReview> BookReviews { get; set; }
        public BookReviewContext(DbContextOptions<BookReviewContext> opt) : base(opt)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlServer("Data Source=ABDULKHADER67;database=BookReviewDB;Integrated Security=True;TrustServerCertificate=True;");
    }
}
