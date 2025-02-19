using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FcbqStats.Migrations
{
    /// <inheritdoc />
    public partial class MatchEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Stats_StatisticsId1",
                table: "Matches");

            migrationBuilder.DropIndex(
                name: "IX_Matches_StatisticsId1",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "StatisticsId1",
                table: "Matches");

            migrationBuilder.AddColumn<int>(
                name: "MatchId",
                table: "Stats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "MatchEvents",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    MatchId = table.Column<int>(type: "INTEGER", nullable: false),
                    TeamId = table.Column<int>(type: "INTEGER", nullable: false),
                    PlayerId = table.Column<long>(type: "INTEGER", nullable: false),
                    ActorShirtNumber = table.Column<string>(type: "TEXT", nullable: false),
                    MoveId = table.Column<int>(type: "INTEGER", nullable: false),
                    MoveDesc = table.Column<string>(type: "TEXT", nullable: false),
                    Min = table.Column<int>(type: "INTEGER", nullable: false),
                    Sec = table.Column<int>(type: "INTEGER", nullable: false),
                    Period = table.Column<int>(type: "INTEGER", nullable: false),
                    Score = table.Column<string>(type: "TEXT", nullable: false),
                    TeamAction = table.Column<bool>(type: "INTEGER", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchEvents", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Stats_MatchId",
                table: "Stats",
                column: "MatchId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Stats_Matches_MatchId",
                table: "Stats",
                column: "MatchId",
                principalTable: "Matches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stats_Matches_MatchId",
                table: "Stats");

            migrationBuilder.DropTable(
                name: "MatchEvents");

            migrationBuilder.DropIndex(
                name: "IX_Stats_MatchId",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "MatchId",
                table: "Stats");

            migrationBuilder.AddColumn<long>(
                name: "StatisticsId1",
                table: "Matches",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Matches_StatisticsId1",
                table: "Matches",
                column: "StatisticsId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Stats_StatisticsId1",
                table: "Matches",
                column: "StatisticsId1",
                principalTable: "Stats",
                principalColumn: "Id");
        }
    }
}
