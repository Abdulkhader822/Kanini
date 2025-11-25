using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillAssesmentPortal.Migrations
{
    /// <inheritdoc />
    public partial class FixCascadeDeleteCycle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Certificates_Tests_TestId",
                table: "Certificates");

            migrationBuilder.DropForeignKey(
                name: "FK_Results_TestLevels_TestLevelId",
                table: "Results");

            migrationBuilder.AddForeignKey(
                name: "FK_Certificates_Tests_TestId",
                table: "Certificates",
                column: "TestId",
                principalTable: "Tests",
                principalColumn: "TestId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Results_TestLevels_TestLevelId",
                table: "Results",
                column: "TestLevelId",
                principalTable: "TestLevels",
                principalColumn: "TestLevelId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Certificates_Tests_TestId",
                table: "Certificates");

            migrationBuilder.DropForeignKey(
                name: "FK_Results_TestLevels_TestLevelId",
                table: "Results");

            migrationBuilder.AddForeignKey(
                name: "FK_Certificates_Tests_TestId",
                table: "Certificates",
                column: "TestId",
                principalTable: "Tests",
                principalColumn: "TestId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Results_TestLevels_TestLevelId",
                table: "Results",
                column: "TestLevelId",
                principalTable: "TestLevels",
                principalColumn: "TestLevelId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
