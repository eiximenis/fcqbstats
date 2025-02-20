using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FcbqStats.Migrations
{
    /// <inheritdoc />
    public partial class Stats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "PlayerId",
                table: "Stats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "PeriodStatistics",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PeriodIndex = table.Column<int>(type: "INTEGER", nullable: false),
                    Points = table.Column<int>(type: "INTEGER", nullable: false),
                    Foults = table.Column<int>(type: "INTEGER", nullable: false),
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
                    StatisticsId = table.Column<int>(type: "INTEGER", nullable: false),
                    StatisticsId1 = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PeriodStatistics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PeriodStatistics_Stats_StatisticsId1",
                        column: x => x.StatisticsId1,
                        principalTable: "Stats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PeriodStatistics_StatisticsId1",
                table: "PeriodStatistics",
                column: "StatisticsId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PeriodStatistics");

            migrationBuilder.DropColumn(
                name: "PlayerId",
                table: "Stats");
        }
    }
}
