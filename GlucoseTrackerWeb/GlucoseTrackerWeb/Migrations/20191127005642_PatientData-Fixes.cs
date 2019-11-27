using Microsoft.EntityFrameworkCore.Migrations;

namespace GlucoseTrackerWeb.Migrations
{
    public partial class PatientDataFixes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MealTime",
                table: "MealItem",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MealTime",
                table: "MealItem");
        }
    }
}
