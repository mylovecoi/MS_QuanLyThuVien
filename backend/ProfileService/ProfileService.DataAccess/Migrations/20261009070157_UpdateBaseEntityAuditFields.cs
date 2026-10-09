using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProfileService.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBaseEntityAuditFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "Profiles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "Profiles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "PhongBans",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "PhongBans",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "ChucVus",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "ChucVus",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "PhongBans");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "PhongBans");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ChucVus");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "ChucVus");
        }
    }
}
