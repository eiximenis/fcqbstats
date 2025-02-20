using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FcbqStats.Migrations
{
    /// <inheritdoc />
    public partial class TotalStats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalFouls",
                table: "Stats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalPoints",
                table: "Stats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalSeconds",
                table: "Stats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalFouls",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "TotalPoints",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "TotalSeconds",
                table: "Stats");
        }
    }
}
