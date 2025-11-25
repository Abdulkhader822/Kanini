using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SkillAssesmentPortal.Migrations
{
    /// <inheritdoc />
    public partial class InitialSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Tests",
                columns: table => new
                {
                    TestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    TestName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DurationMins = table.Column<int>(type: "int", nullable: false),
                    TotalQuestions = table.Column<int>(type: "int", nullable: false),
                    TotalMarks = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tests", x => x.TestId);
                    table.ForeignKey(
                        name: "FK_Tests_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tests_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Certificates",
                columns: table => new
                {
                    CertificateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    TestId = table.Column<int>(type: "int", nullable: false),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CertificateURL = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certificates", x => x.CertificateId);
                    table.ForeignKey(
                        name: "FK_Certificates_Tests_TestId",
                        column: x => x.TestId,
                        principalTable: "Tests",
                        principalColumn: "TestId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Certificates_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TestLevels",
                columns: table => new
                {
                    TestLevelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestId = table.Column<int>(type: "int", nullable: false),
                    LevelName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PassingScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    VideoLink = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestLevels", x => x.TestLevelId);
                    table.ForeignKey(
                        name: "FK_TestLevels_Tests_TestId",
                        column: x => x.TestId,
                        principalTable: "Tests",
                        principalColumn: "TestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    QuestionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestLevelId = table.Column<int>(type: "int", nullable: false),
                    QuestionText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OptionA = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OptionB = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OptionC = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OptionD = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CorrectOption = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.QuestionId);
                    table.ForeignKey(
                        name: "FK_Questions_TestLevels_TestLevelId",
                        column: x => x.TestLevelId,
                        principalTable: "TestLevels",
                        principalColumn: "TestLevelId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Results",
                columns: table => new
                {
                    ResultId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    TestId = table.Column<int>(type: "int", nullable: false),
                    TestLevelId = table.Column<int>(type: "int", nullable: false),
                    AttemptNumber = table.Column<int>(type: "int", nullable: false),
                    Score = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    Percentage = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    TimeTakenSecs = table.Column<int>(type: "int", nullable: false),
                    DateAttempted = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResultStatus = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Suggestion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Results", x => x.ResultId);
                    table.ForeignKey(
                        name: "FK_Results_TestLevels_TestLevelId",
                        column: x => x.TestLevelId,
                        principalTable: "TestLevels",
                        principalColumn: "TestLevelId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Results_Tests_TestId",
                        column: x => x.TestId,
                        principalTable: "Tests",
                        principalColumn: "TestId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Results_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "CategoryName", "Description" },
                values: new object[,]
                {
                    { 1, "Technical", "Programming and Software" },
                    { 2, "Aptitude", "Logical and Quantitative Skills" },
                    { 3, "Soft Skills", "Communication and Behavioral Skills" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "CreatedAt", "Email", "Name", "PasswordHash", "Role" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@portal.com", "Admin User", "admin123", "Admin" },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "student1@portal.com", "Student One", "student123", "User" },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "student2@portal.com", "Student Two", "student123", "User" }
                });

            migrationBuilder.InsertData(
                table: "Tests",
                columns: new[] { "TestId", "CategoryId", "CreatedAt", "CreatedBy", "DurationMins", "TestName", "TotalMarks", "TotalQuestions" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 30, "C# Fundamentals", 100, 15 },
                    { 2, 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 30, "Logical Reasoning", 100, 20 },
                    { 3, 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 20, "Communication Skills", 50, 10 }
                });

            migrationBuilder.InsertData(
                table: "Certificates",
                columns: new[] { "CertificateId", "CertificateURL", "IssueDate", "TestId", "UserId" },
                values: new object[] { 1, "/certificates/student1_csharpfundamentals.pdf", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2 });

            migrationBuilder.InsertData(
                table: "TestLevels",
                columns: new[] { "TestLevelId", "LevelName", "PassingScore", "TestId", "VideoLink" },
                values: new object[,]
                {
                    { 1, "Easy", 60.00m, 1, "https://www.youtube.com/watch?v=gfkTfcpWqAY" },
                    { 2, "Medium", 65.00m, 1, "https://www.youtube.com/watch?v=GhQdlIFylQ8" },
                    { 3, "Hard", 70.00m, 1, "https://www.youtube.com/watch?v=IYvD9oBCuJI" },
                    { 4, "Easy", 60.00m, 2, "https://www.youtube.com/watch?v=lX2U3R1HfN0" },
                    { 5, "Medium", 65.00m, 2, "https://www.youtube.com/watch?v=QTY6lFQ0V7c" },
                    { 6, "Hard", 70.00m, 2, "https://www.youtube.com/watch?v=xEqlgNaJ1WA" },
                    { 7, "Easy", 55.00m, 3, "https://www.youtube.com/watch?v=HAnw168huqA" }
                });

            migrationBuilder.InsertData(
                table: "Questions",
                columns: new[] { "QuestionId", "CorrectOption", "OptionA", "OptionB", "OptionC", "OptionD", "QuestionText", "TestLevelId" },
                values: new object[,]
                {
                    { 1, "A", "Common Language Runtime", "Compile Language Runtime", "Code Logic Reader", "None of the above", "What does CLR stand for in C#?", 1 },
                    { 2, "C", "function", "define", "class", "structure", "Which keyword is used to define a class in C#?", 1 },
                    { 3, "D", "18", "20", "24", "32", "Find the missing number: 2, 4, 8, 16, ?", 4 },
                    { 4, "A", "Listening carefully", "Using complex words", "Talking loudly", "Interrupting others", "Effective communication includes:", 7 }
                });

            migrationBuilder.InsertData(
                table: "Results",
                columns: new[] { "ResultId", "AttemptNumber", "DateAttempted", "Percentage", "ResultStatus", "Score", "Suggestion", "TestId", "TestLevelId", "TimeTakenSecs", "UserId" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 85.50m, "Pass", 85.50m, "Good job! Proceed to Medium level.", 1, 1, 1200, 2 },
                    { 2, 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 45.00m, "Fail", 45.00m, "Need to improve logical reasoning.", 2, 4, 1300, 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_CategoryName",
                table: "Categories",
                column: "CategoryName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_TestId",
                table: "Certificates",
                column: "TestId");

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_UserId_TestId",
                table: "Certificates",
                columns: new[] { "UserId", "TestId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Questions_TestLevelId",
                table: "Questions",
                column: "TestLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_Results_TestId",
                table: "Results",
                column: "TestId");

            migrationBuilder.CreateIndex(
                name: "IX_Results_TestLevelId",
                table: "Results",
                column: "TestLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_Results_UserId",
                table: "Results",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TestLevels_TestId_LevelName",
                table: "TestLevels",
                columns: new[] { "TestId", "LevelName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tests_CategoryId",
                table: "Tests",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Tests_CreatedBy",
                table: "Tests",
                column: "CreatedBy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Certificates");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "Results");

            migrationBuilder.DropTable(
                name: "TestLevels");

            migrationBuilder.DropTable(
                name: "Tests");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
