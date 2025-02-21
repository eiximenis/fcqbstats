using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FcbqStats.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clubs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClubCode = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clubs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Matches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LocalTeamId = table.Column<int>(type: "INTEGER", nullable: false),
                    LocalTeamName = table.Column<string>(type: "TEXT", nullable: false),
                    AwayTeamId = table.Column<int>(type: "INTEGER", nullable: false),
                    AwayTeamName = table.Column<string>(type: "TEXT", nullable: false),
                    Date = table.Column<string>(type: "TEXT", nullable: false),
                    Hour = table.Column<string>(type: "TEXT", nullable: false),
                    StatisticsFcbqSid = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matches", x => x.Id);
                });

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
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LocalIndex = table.Column<int>(type: "INTEGER", nullable: false),
                    LicenseId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    TeamId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ClubId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Teams_Clubs_ClubId",
                        column: x => x.ClubId,
                        principalTable: "Clubs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Stats",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MatchId = table.Column<int>(type: "INTEGER", nullable: false),
                    FcbqSid = table.Column<string>(type: "TEXT", nullable: false),
                    PlayerId = table.Column<long>(type: "INTEGER", nullable: false),
                    TotalPoints = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalSeconds = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalFouls = table.Column<int>(type: "INTEGER", nullable: false),
                    PlayerLicenseId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stats_Matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PeriodStatistics",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PeriodIndex = table.Column<int>(type: "INTEGER", nullable: false),
                    Points = table.Column<int>(type: "INTEGER", nullable: false),
                    Fouls = table.Column<int>(type: "INTEGER", nullable: false),
                    OnePointAttempts = table.Column<int>(type: "INTEGER", nullable: false),
                    OnePointMade = table.Column<int>(type: "INTEGER", nullable: false),
                    TwoPointAttempts = table.Column<int>(type: "INTEGER", nullable: false),
                    TwoPointsMade = table.Column<int>(type: "INTEGER", nullable: false),
                    ThreePointAttempts = table.Column<int>(type: "INTEGER", nullable: false),
                    ThreePointsMade = table.Column<int>(type: "INTEGER", nullable: false),
                    OnePointPercent = table.Column<int>(type: "INTEGER", nullable: false),
                    TwoPointPercent = table.Column<int>(type: "INTEGER", nullable: false),
                    ThreePointPercent = table.Column<int>(type: "INTEGER", nullable: false),
                    SecondsPlayed = table.Column<int>(type: "INTEGER", nullable: false),
                    StatisticsId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PeriodStatistics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PeriodStatistics_Stats_StatisticsId",
                        column: x => x.StatisticsId,
                        principalTable: "Stats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PeriodStatistics_StatisticsId",
                table: "PeriodStatistics",
                column: "StatisticsId");

            migrationBuilder.CreateIndex(
                name: "IX_Stats_MatchId",
                table: "Stats",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_ClubId",
                table: "Teams",
                column: "ClubId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MatchEvents");

            migrationBuilder.DropTable(
                name: "PeriodStatistics");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropTable(
                name: "Stats");

            migrationBuilder.DropTable(
                name: "Clubs");

            migrationBuilder.DropTable(
                name: "Matches");
        }
    }
}
