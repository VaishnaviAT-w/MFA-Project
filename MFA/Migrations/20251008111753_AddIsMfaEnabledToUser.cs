using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MFA.Migrations
{
    /// <inheritdoc />
    public partial class AddIsMfaEnabledToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ITUserMaster",
                table: "ITUserMaster");

            migrationBuilder.DropColumn(
                name: "Mobileno",
                table: "ITUserMaster");

            migrationBuilder.RenameTable(
                name: "ITUserMaster",
                newName: "ITUserMasters");

            migrationBuilder.AlterColumn<bool>(
                name: "IsVerified",
                table: "ITUserMasters",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsMfaEnabled",
                table: "ITUserMasters",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ITUserMasters",
                table: "ITUserMasters",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ITUserMasters",
                table: "ITUserMasters");

            migrationBuilder.DropColumn(
                name: "IsMfaEnabled",
                table: "ITUserMasters");

            migrationBuilder.RenameTable(
                name: "ITUserMasters",
                newName: "ITUserMaster");

            migrationBuilder.AlterColumn<bool>(
                name: "IsVerified",
                table: "ITUserMaster",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AddColumn<string>(
                name: "Mobileno",
                table: "ITUserMaster",
                type: "text",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ITUserMaster",
                table: "ITUserMaster",
                column: "UserId");
        }
    }
}
