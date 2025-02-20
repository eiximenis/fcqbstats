using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FcbqStats.Migrations
{
    /// <inheritdoc />
    public partial class MatchHasNStatstics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Stats_MatchId",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "StatisticsId",
                table: "Matches");

            migrationBuilder.CreateIndex(
                name: "IX_Stats_MatchId",
                table: "Stats",
                column: "MatchId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Stats_MatchId",
                table: "Stats");

            migrationBuilder.AddColumn<int>(
                name: "StatisticsId",
                table: "Matches",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stats_MatchId",
                table: "Stats",
                column: "MatchId",
                unique: true);
        }
    }
}
