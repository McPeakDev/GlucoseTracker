using Microsoft.EntityFrameworkCore.Migrations;

namespace GlucoseTrackerWeb.Migrations
{
    public partial class FixBlood : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientBloodSugar_MealItem_MealId",
                table: "PatientBloodSugar");

            migrationBuilder.AlterColumn<int>(
                name: "MealId",
                table: "PatientBloodSugar",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_PatientBloodSugar_MealItem_MealId",
                table: "PatientBloodSugar",
                column: "MealId",
                principalTable: "MealItem",
                principalColumn: "MealId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientBloodSugar_MealItem_MealId",
                table: "PatientBloodSugar");

            migrationBuilder.AlterColumn<int>(
                name: "MealId",
                table: "PatientBloodSugar",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientBloodSugar_MealItem_MealId",
                table: "PatientBloodSugar",
                column: "MealId",
                principalTable: "MealItem",
                principalColumn: "MealId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
