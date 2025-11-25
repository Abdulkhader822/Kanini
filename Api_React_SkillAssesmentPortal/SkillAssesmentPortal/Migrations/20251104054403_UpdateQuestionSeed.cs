using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillAssesmentPortal.Migrations
{
    /// <inheritdoc />
    public partial class UpdateQuestionSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Results_TestLevels_TestLevelId",
                table: "Results");

            migrationBuilder.DropForeignKey(
                name: "FK_Results_Users_UserId",
                table: "Results");

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "QuestionId",
                keyValue: 2,
                column: "CorrectOption",
                value: "D");

            migrationBuilder.AddForeignKey(
                name: "FK_Results_TestLevels_TestLevelId",
                table: "Results",
                column: "TestLevelId",
                principalTable: "TestLevels",
                principalColumn: "TestLevelId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Results_Users_UserId",
                table: "Results",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Results_TestLevels_TestLevelId",
                table: "Results");

            migrationBuilder.DropForeignKey(
                name: "FK_Results_Users_UserId",
                table: "Results");

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "QuestionId",
                keyValue: 2,
                column: "CorrectOption",
                value: "C");

            migrationBuilder.AddForeignKey(
                name: "FK_Results_TestLevels_TestLevelId",
                table: "Results",
                column: "TestLevelId",
                principalTable: "TestLevels",
                principalColumn: "TestLevelId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Results_Users_UserId",
                table: "Results",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
