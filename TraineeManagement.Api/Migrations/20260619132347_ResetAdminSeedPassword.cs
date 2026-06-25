using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TraineeManagement.Api.Migrations
{
    /// <inheritdoc />
    public partial class ResetAdminSeedPassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "PasswordHash", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 6, 19, 13, 23, 47, 95, DateTimeKind.Utc).AddTicks(2597), "$2a$11$GlvJ0iM.E8KIRXhxbvu.duyyuL2sI9qMPoGX8aqkuXd66InfytuFW", new DateTime(2026, 6, 19, 13, 23, 47, 95, DateTimeKind.Utc).AddTicks(2963) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "PasswordHash", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 6, 19, 13, 16, 50, 272, DateTimeKind.Utc).AddTicks(8340), "$2a$11$xyyO77axyDbNDmG15A6Vm.uWyxqJV1OEU1dm1sggXZoGZ.QznYy9i", new DateTime(2026, 6, 19, 13, 16, 50, 272, DateTimeKind.Utc).AddTicks(8715) });
        }
    }
}
