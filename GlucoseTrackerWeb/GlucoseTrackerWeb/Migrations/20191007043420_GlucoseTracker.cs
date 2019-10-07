using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GlucoseTrackerWeb.Migrations
{
    public partial class GlucoseTracker : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MealItem",
                columns: table => new
                {
                    MealId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FoodId = table.Column<int>(nullable: false),
                    FoodName = table.Column<string>(nullable: true),
                    Carbs = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealItem", x => x.MealId);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Password = table.Column<string>(maxLength: 255, nullable: false),
                    FirstName = table.Column<string>(maxLength: 150, nullable: false),
                    MiddleName = table.Column<string>(maxLength: 150, nullable: true),
                    LastName = table.Column<string>(maxLength: 150, nullable: false),
                    Email = table.Column<string>(maxLength: 255, nullable: false),
                    PhoneNumber = table.Column<string>(maxLength: 11, nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    DoctorId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_User_User_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Login",
                columns: table => new
                {
                    LoginId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Email = table.Column<string>(maxLength: 255, nullable: false),
                    Password = table.Column<string>(maxLength: 255, nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Login", x => x.LoginId);
                    table.ForeignKey(
                        name: "FK_Login_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PatientBloodSugar",
                columns: table => new
                {
                    BloodId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PatientId = table.Column<int>(nullable: false),
                    LevelBefore = table.Column<float>(nullable: false),
                    LevelAfter = table.Column<float>(nullable: false),
                    Meal = table.Column<string>(nullable: true),
                    TimeOfDay = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientBloodSugar", x => x.BloodId);
                    table.ForeignKey(
                        name: "FK_PatientBloodSugar_User_PatientId",
                        column: x => x.PatientId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PatientCarbohydrates",
                columns: table => new
                {
                    CarbId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PatientId = table.Column<int>(nullable: false),
                    TotalCarbs = table.Column<int>(nullable: false),
                    MealName = table.Column<string>(nullable: true),
                    FoodCarbs = table.Column<int>(nullable: false),
                    Meal = table.Column<string>(nullable: true),
                    TimeOfDay = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientCarbohydrates", x => x.CarbId);
                    table.ForeignKey(
                        name: "FK_PatientCarbohydrates_User_PatientId",
                        column: x => x.PatientId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PatientExercise",
                columns: table => new
                {
                    ExerciseId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PatientId = table.Column<int>(nullable: false),
                    HoursExercised = table.Column<int>(nullable: false),
                    TimeOfDay = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientExercise", x => x.ExerciseId);
                    table.ForeignKey(
                        name: "FK_PatientExercise_User_PatientId",
                        column: x => x.PatientId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Login_UserId",
                table: "Login",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientBloodSugar_PatientId",
                table: "PatientBloodSugar",
                column: "PatientId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PatientCarbohydrates_PatientId",
                table: "PatientCarbohydrates",
                column: "PatientId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PatientExercise_PatientId",
                table: "PatientExercise",
                column: "PatientId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_DoctorId",
                table: "User",
                column: "DoctorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Login");

            migrationBuilder.DropTable(
                name: "MealItem");

            migrationBuilder.DropTable(
                name: "PatientBloodSugar");

            migrationBuilder.DropTable(
                name: "PatientCarbohydrates");

            migrationBuilder.DropTable(
                name: "PatientExercise");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
