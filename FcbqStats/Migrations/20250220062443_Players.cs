using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FcbqStats.Migrations
{
    /// <inheritdoc />
    public partial class Players : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Players",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "Players",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Players");
        }
    }
}
