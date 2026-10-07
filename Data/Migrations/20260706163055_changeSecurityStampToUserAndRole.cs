using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class changeSecurityStampToUserAndRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SecurityStamp",
                table: "UserRoles");

            migrationBuilder.AddColumn<string>(
                name: "SecurityStamp",
                table: "User",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SecurityStamp",
                table: "Role",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SecurityStamp",
                table: "User");

            migrationBuilder.DropColumn(
                name: "SecurityStamp",
                table: "Role");

            migrationBuilder.AddColumn<string>(
                name: "SecurityStamp",
                table: "UserRoles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
