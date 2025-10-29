using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MFA.Migrations
{
    /// <inheritdoc />
    public partial class ReAddOtpColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OTP",
                table: "ITUserMasters",
                newName: "MfaType");

            migrationBuilder.AddColumn<bool>(
                name: "IsOtpLocked",
                table: "ITUserMasters",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OtpAttempts",
                table: "ITUserMasters",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "OtpLockedOn",
                table: "ITUserMasters",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OtpMaster",
                columns: table => new
                {
                    OtpId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    OtpCode = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    OtpType = table.Column<string>(type: "text", nullable: false),
                    IsUsed = table.Column<bool>(type: "boolean", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OtpMaster", x => x.OtpId);
                    table.ForeignKey(
                        name: "FK_OtpMaster_ITUserMasters_UserId",
                        column: x => x.UserId,
                        principalTable: "ITUserMasters",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OtpMaster_UserId",
                table: "OtpMaster",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OtpMaster");

            migrationBuilder.DropColumn(
                name: "IsOtpLocked",
                table: "ITUserMasters");

            migrationBuilder.DropColumn(
                name: "OtpAttempts",
                table: "ITUserMasters");

            migrationBuilder.DropColumn(
                name: "OtpLockedOn",
                table: "ITUserMasters");

            migrationBuilder.RenameColumn(
                name: "MfaType",
                table: "ITUserMasters",
                newName: "OTP");
        }
    }
}
