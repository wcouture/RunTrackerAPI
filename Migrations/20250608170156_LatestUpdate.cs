using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RunTrackerAPI.Migrations
{
    /// <inheritdoc />
    public partial class LatestUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "RunData",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "AccountData",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "RunData");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "AccountData");
        }
    }
}
