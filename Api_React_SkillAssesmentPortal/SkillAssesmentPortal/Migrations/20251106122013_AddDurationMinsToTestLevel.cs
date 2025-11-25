using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillAssesmentPortal.Migrations
{
    /// <inheritdoc />
    public partial class AddDurationMinsToTestLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DurationMins",
                table: "TestLevels",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "TestLevels",
                keyColumn: "TestLevelId",
                keyValue: 1,
                column: "DurationMins",
                value: 0);

            migrationBuilder.UpdateData(
                table: "TestLevels",
                keyColumn: "TestLevelId",
                keyValue: 2,
                column: "DurationMins",
                value: 0);

            migrationBuilder.UpdateData(
                table: "TestLevels",
                keyColumn: "TestLevelId",
                keyValue: 3,
                column: "DurationMins",
                value: 0);

            migrationBuilder.UpdateData(
                table: "TestLevels",
                keyColumn: "TestLevelId",
                keyValue: 4,
                column: "DurationMins",
                value: 0);

            migrationBuilder.UpdateData(
                table: "TestLevels",
                keyColumn: "TestLevelId",
                keyValue: 5,
                column: "DurationMins",
                value: 0);

            migrationBuilder.UpdateData(
                table: "TestLevels",
                keyColumn: "TestLevelId",
                keyValue: 6,
                column: "DurationMins",
                value: 0);

            migrationBuilder.UpdateData(
                table: "TestLevels",
                keyColumn: "TestLevelId",
                keyValue: 7,
                column: "DurationMins",
                value: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DurationMins",
                table: "TestLevels");
        }
    }
}
