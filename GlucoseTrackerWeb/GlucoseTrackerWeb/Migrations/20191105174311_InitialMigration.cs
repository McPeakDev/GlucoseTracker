using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GlucoseTrackerWeb.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Auth",
                columns: table => new
                {
                    AuthId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Email = table.Column<string>(maxLength: 255, nullable: false),
                    Password = table.Column<string>(maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auth", x => x.AuthId);
                });

            migrationBuilder.CreateTable(
                name: "MealItem",
                columns: table => new
                {
                    MealId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
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
                    FirstName = table.Column<string>(maxLength: 150, nullable: false),
                    MiddleName = table.Column<string>(maxLength: 150, nullable: true),
                    LastName = table.Column<string>(maxLength: 150, nullable: false),
                    Email = table.Column<string>(maxLength: 255, nullable: true),
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
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PatientBloodSugar",
                columns: table => new
                {
                    BloodId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: false),
                    MealId = table.Column<int>(nullable: false),
                    LevelBefore = table.Column<float>(nullable: false),
                    LevelAfter = table.Column<float>(nullable: false),
                    TimeOfDay = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientBloodSugar", x => x.BloodId);
                    table.ForeignKey(
                        name: "FK_PatientBloodSugar_MealItem_MealId",
                        column: x => x.MealId,
                        principalTable: "MealItem",
                        principalColumn: "MealId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PatientBloodSugar_User_UserId",
                        column: x => x.UserId,
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
                    UserId = table.Column<int>(nullable: false),
                    MealId = table.Column<int>(nullable: false),
                    FoodCarbs = table.Column<int>(nullable: false),
                    TimeOfDay = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientCarbohydrates", x => x.CarbId);
                    table.ForeignKey(
                        name: "FK_PatientCarbohydrates_MealItem_MealId",
                        column: x => x.MealId,
                        principalTable: "MealItem",
                        principalColumn: "MealId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PatientCarbohydrates_User_UserId",
                        column: x => x.UserId,
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
                    UserId = table.Column<int>(nullable: false),
                    HoursExercised = table.Column<float>(nullable: false),
                    TimeOfDay = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientExercise", x => x.ExerciseId);
                    table.ForeignKey(
                        name: "FK_PatientExercise_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TokenAuth",
                columns: table => new
                {
                    TokenId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: false),
                    AuthId = table.Column<int>(nullable: false),
                    Token = table.Column<string>(maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TokenAuth", x => x.TokenId);
                    table.ForeignKey(
                        name: "FK_TokenAuth_Auth_AuthId",
                        column: x => x.AuthId,
                        principalTable: "Auth",
                        principalColumn: "AuthId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TokenAuth_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PatientBloodSugar_MealId",
                table: "PatientBloodSugar",
                column: "MealId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientBloodSugar_UserId",
                table: "PatientBloodSugar",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientCarbohydrates_MealId",
                table: "PatientCarbohydrates",
                column: "MealId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientCarbohydrates_UserId",
                table: "PatientCarbohydrates",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientExercise_UserId",
                table: "PatientExercise",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TokenAuth_AuthId",
                table: "TokenAuth",
                column: "AuthId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TokenAuth_UserId",
                table: "TokenAuth",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_User_DoctorId",
                table: "User",
                column: "DoctorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PatientBloodSugar");

            migrationBuilder.DropTable(
                name: "PatientCarbohydrates");

            migrationBuilder.DropTable(
                name: "PatientExercise");

            migrationBuilder.DropTable(
                name: "TokenAuth");

            migrationBuilder.DropTable(
                name: "MealItem");

            migrationBuilder.DropTable(
                name: "Auth");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
