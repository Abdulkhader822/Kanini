using Microsoft.EntityFrameworkCore;
using SkillAssessmentPortal.Models.Enums;
using System;

namespace SkillAssessmentPortal.Models
{
    public class SkillAssessmentDbContext : DbContext
    {
        public SkillAssessmentDbContext(DbContextOptions<SkillAssessmentDbContext> options)
            : base(options)
        {
        }

        // Tables
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<TestLevel> TestLevels { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<UserAnswer> UserAnswers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ---------- Enum Conversion ----------
            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion<string>(); // store "Admin" / "User" instead of 1/2

            // ---------- Decimal Precision ----------
            modelBuilder.Entity<Result>()
                .Property(r => r.Score)
                .HasColumnType("decimal(5,2)");

            modelBuilder.Entity<Result>()
                .Property(r => r.Percentage)
                .HasColumnType("decimal(5,2)");

            modelBuilder.Entity<TestLevel>()
                .Property(tl => tl.PassingScore)
                .HasColumnType("decimal(5,2)");

            // ---------- Unique Constraints ----------
            modelBuilder.Entity<TestLevel>()
                .HasIndex(tl => new { tl.TestId, tl.LevelName })
                .IsUnique();

            modelBuilder.Entity<Category>()
                .HasIndex(c => c.CategoryName)
                .IsUnique();

            modelBuilder.Entity<Certificate>()
                .HasIndex(c => new { c.UserId, c.TestId })
                .IsUnique();

            // ---------- Relationships ----------
            modelBuilder.Entity<Test>()
                .HasOne(t => t.Category)
                .WithMany(c => c.Tests)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Test>()
                .HasOne(t => t.AdminUser)
                .WithMany(u => u.CreatedTests)
                .HasForeignKey(t => t.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TestLevel>()
                .HasOne(tl => tl.Test)
                .WithMany(t => t.Levels)
                .HasForeignKey(tl => tl.TestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Question>()
                .HasOne(q => q.TestLevel)
                .WithMany(tl => tl.Questions)
                .HasForeignKey(q => q.TestLevelId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Result>()
                .HasOne(r => r.User)
                .WithMany(u => u.Results)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Result>()
                .HasOne(r => r.Test)
                .WithMany(t => t.Results)
                .HasForeignKey(r => r.TestId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Result>()
                .HasOne(r => r.TestLevel)
                .WithMany(tl => tl.Results)
                .HasForeignKey(r => r.TestLevelId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Certificate>()
                .HasOne(c => c.User)
                .WithMany(u => u.Certificates)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Certificate>()
                .HasOne(c => c.Test)
                .WithMany(t => t.Certificates)
                .HasForeignKey(c => c.TestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserAnswer>()
                .HasOne(ua => ua.Result)
                .WithMany()
                .HasForeignKey(ua => ua.ResultId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserAnswer>()
                .HasOne(ua => ua.Question)
                .WithMany()
                .HasForeignKey(ua => ua.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------- Seed Data ----------
            var seedDate = new DateTime(2025, 01, 01);

            // USERS (Updated to Enum)
            modelBuilder.Entity<User>().HasData(
                new User { UserId = 1, Name = "Admin User", Email = "admin@portal.com", PasswordHash = "admin123", Role = RoleType.Admin, CreatedAt = seedDate },
                new User { UserId = 2, Name = "Student One", Email = "student1@portal.com", PasswordHash = "student123", Role = RoleType.User, CreatedAt = seedDate },
                new User { UserId = 3, Name = "Student Two", Email = "student2@portal.com", PasswordHash = "student123", Role = RoleType.User, CreatedAt = seedDate }
            );

            // CATEGORIES
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = 1, CategoryName = "Technical", Description = "Programming and Software" },
                new Category { CategoryId = 2, CategoryName = "Aptitude", Description = "Logical and Quantitative Skills" },
                new Category { CategoryId = 3, CategoryName = "Soft Skills", Description = "Communication and Behavioral Skills" }
            );

            // TESTS
            modelBuilder.Entity<Test>().HasData(
                new Test { TestId = 1, CategoryId = 1, TestName = "C# Fundamentals", DurationMins = 30, TotalQuestions = 15, TotalMarks = 100, CreatedBy = 1, CreatedAt = seedDate },
                new Test { TestId = 2, CategoryId = 2, TestName = "Logical Reasoning", DurationMins = 30, TotalQuestions = 20, TotalMarks = 100, CreatedBy = 1, CreatedAt = seedDate },
                new Test { TestId = 3, CategoryId = 3, TestName = "Communication Skills", DurationMins = 20, TotalQuestions = 10, TotalMarks = 50, CreatedBy = 1, CreatedAt = seedDate }
            );

            // TEST LEVELS
            modelBuilder.Entity<TestLevel>().HasData(
                new TestLevel { TestLevelId = 1, TestId = 1, LevelName = "Easy", PassingScore = 60.00m, VideoLink = "https://www.youtube.com/watch?v=gfkTfcpWqAY" },
                new TestLevel { TestLevelId = 2, TestId = 1, LevelName = "Medium", PassingScore = 65.00m, VideoLink = "https://www.youtube.com/watch?v=GhQdlIFylQ8" },
                new TestLevel { TestLevelId = 3, TestId = 1, LevelName = "Hard", PassingScore = 70.00m, VideoLink = "https://www.youtube.com/watch?v=IYvD9oBCuJI" },
                new TestLevel { TestLevelId = 4, TestId = 2, LevelName = "Easy", PassingScore = 60.00m, VideoLink = "https://www.youtube.com/watch?v=lX2U3R1HfN0" },
                new TestLevel { TestLevelId = 5, TestId = 2, LevelName = "Medium", PassingScore = 65.00m, VideoLink = "https://www.youtube.com/watch?v=QTY6lFQ0V7c" },
                new TestLevel { TestLevelId = 6, TestId = 2, LevelName = "Hard", PassingScore = 70.00m, VideoLink = "https://www.youtube.com/watch?v=xEqlgNaJ1WA" },
                new TestLevel { TestLevelId = 7, TestId = 3, LevelName = "Easy", PassingScore = 55.00m, VideoLink = "https://www.youtube.com/watch?v=HAnw168huqA" }
            );

            // QUESTIONS
            modelBuilder.Entity<Question>().HasData(
                new Question { QuestionId = 1, TestLevelId = 1, QuestionText = "What does CLR stand for in C#?", OptionA = "Common Language Runtime", OptionB = "Compile Language Runtime", OptionC = "Code Logic Reader", OptionD = "None of the above", CorrectOption = "A" },
                new Question { QuestionId = 2, TestLevelId = 1, QuestionText = "Which keyword is used to define a class in C#?", OptionA = "function", OptionB = "define", OptionC = "class", OptionD = "structure", CorrectOption = "C" },
                new Question { QuestionId = 3, TestLevelId = 4, QuestionText = "Find the missing number: 2, 4, 8, 16, ?", OptionA = "18", OptionB = "20", OptionC = "24", OptionD = "32", CorrectOption = "D" },
                new Question { QuestionId = 4, TestLevelId = 7, QuestionText = "Effective communication includes:", OptionA = "Listening carefully", OptionB = "Using complex words", OptionC = "Talking loudly", OptionD = "Interrupting others", CorrectOption = "A" }
            );

            // RESULTS
            modelBuilder.Entity<Result>().HasData(
                new Result { ResultId = 1, UserId = 2, TestId = 1, TestLevelId = 1, AttemptNumber = 1, Score = 85.50m, Percentage = 85.50m, TimeTakenSecs = 1200, DateAttempted = seedDate, ResultStatus = "Pass", Suggestion = "Good job! Proceed to Medium level." },
                new Result { ResultId = 2, UserId = 2, TestId = 2, TestLevelId = 4, AttemptNumber = 1, Score = 45.00m, Percentage = 45.00m, TimeTakenSecs = 1300, DateAttempted = seedDate, ResultStatus = "Fail", Suggestion = "Need to improve logical reasoning." }
            );

            // CERTIFICATES
            modelBuilder.Entity<Certificate>().HasData(
                new Certificate { CertificateId = 1, UserId = 2, TestId = 1, IssueDate = seedDate, CertificateURL = "/certificates/student1_csharpfundamentals.pdf" }
            );
        }
    }
}
