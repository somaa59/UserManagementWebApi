using Microsoft.EntityFrameworkCore.Migrations;
using UserManagementWebApi.DTO.Account.Enum;

#nullable disable

namespace UserManagementWebApi.Migrations
{
    /// <inheritdoc />
    public partial class SeedRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[]{ "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                values: new object [] {Guid.NewGuid().ToString(),Roles.User.ToString(), "User".ToUpper(),Guid.NewGuid().ToString() }
            );

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                values: new object[] { Guid.NewGuid().ToString(), Roles.Admin.ToString(), "Admin".ToUpper(), Guid.NewGuid().ToString() }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [UserRoles]");
        }
    }
}
