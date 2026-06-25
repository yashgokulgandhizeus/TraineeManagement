using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TraineeManagement.Api.Migrations
{
    /// <inheritdoc />
    public partial class FixStaticAdminSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "PasswordHash", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 6, 19, 13, 26, 12, 394, DateTimeKind.Utc).AddTicks(9725), "$2a$11$R9h6tcOf6gC69/07uGf7peXgXpE5vLz87Yn2A2rN9A7E1bC9vP7K.", new DateTime(2026, 6, 19, 13, 26, 12, 395, DateTimeKind.Utc).AddTicks(207) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "PasswordHash", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 6, 19, 13, 23, 47, 95, DateTimeKind.Utc).AddTicks(2597), "$2a$11$GlvJ0iM.E8KIRXhxbvu.duyyuL2sI9qMPoGX8aqkuXd66InfytuFW", new DateTime(2026, 6, 19, 13, 23, 47, 95, DateTimeKind.Utc).AddTicks(2963) });
        }
    }
}
