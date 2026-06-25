using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace TraineeManagement.Api.Migrations
{
    /// <inheritdoc />
    public partial class SubmissionJob : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "Users",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Trainees",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Mentors",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.CreateTable(
                name: "ProcessingJobs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    CorrelationId = table.Column<Guid>(type: "char(36)", nullable: false),
                    FileId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Attempts = table.Column<int>(type: "int", nullable: false),
                    ErrorSummary = table.Column<string>(type: "longtext", nullable: true),
                    StartedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessingJobs", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "PasswordHash", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 6, 24, 8, 25, 48, 156, DateTimeKind.Utc).AddTicks(7791), "$2a$11$HmfnKN.Gtk0OAhxVGNYl9O6w8HB7Gs1tW1Xs5i/Hxj6LDgqli3qdO", new DateTime(2026, 6, 24, 8, 25, 48, 156, DateTimeKind.Utc).AddTicks(8922) });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName",
                table: "Users",
                column: "UserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Trainees_Email",
                table: "Trainees",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Trainees_Status",
                table: "Trainees",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Mentors_Email",
                table: "Mentors",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Mentors_Status",
                table: "Mentors",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_LearningTasks_DueDate",
                table: "LearningTasks",
                column: "DueDate");

            migrationBuilder.CreateIndex(
                name: "IX_LearningTasks_Status",
                table: "LearningTasks",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessingJobs_CorrelationId",
                table: "ProcessingJobs",
                column: "CorrelationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProcessingJobs_FileId",
                table: "ProcessingJobs",
                column: "FileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProcessingJobs");

            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_UserName",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Trainees_Email",
                table: "Trainees");

            migrationBuilder.DropIndex(
                name: "IX_Trainees_Status",
                table: "Trainees");

            migrationBuilder.DropIndex(
                name: "IX_Mentors_Email",
                table: "Mentors");

            migrationBuilder.DropIndex(
                name: "IX_Mentors_Status",
                table: "Mentors");

            migrationBuilder.DropIndex(
                name: "IX_LearningTasks_DueDate",
                table: "LearningTasks");

            migrationBuilder.DropIndex(
                name: "IX_LearningTasks_Status",
                table: "LearningTasks");

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "Users",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Trainees",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Mentors",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "PasswordHash", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 6, 19, 14, 10, 52, 836, DateTimeKind.Utc).AddTicks(7981), "$2a$11$R9h6tcOf6gC69/07uGf7peXgXpE5vLz87Yn2A2rN9A7E1bC9vP7K.", new DateTime(2026, 6, 19, 14, 10, 52, 836, DateTimeKind.Utc).AddTicks(8506) });
        }
    }
}
