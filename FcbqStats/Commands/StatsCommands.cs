using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FcbqStats.Data;
using Spectre.Console;

namespace FcbqStats.Commands;
internal class StatsCommands
{

    enum ViewStatsInfo
    {
        Minutes,
        Points,
        Fouls
    };

    public static async Task<CommandResult> View(IEnumerable<string> args)
    {
        if (MainState.SelectedMatch == null)
        {
            AnsiConsole.MarkupLine("No match selected, please run [white]/lm[/] to load a match");
            return CommandResult.Error;
        }

        var infoToView = ViewStatsInfo.Minutes;


        if (args.Any())
        {
            infoToView = args.First().ToLower() switch
            {
                "minutes" => ViewStatsInfo.Minutes,
                "points" => ViewStatsInfo.Points,
                "fouls" => ViewStatsInfo.Fouls,
                _ => ViewStatsInfo.Minutes
            };
        }

        var stats = MainState.GetStatsForMatch(MainState.SelectedMatch.Id);

        var periods = stats.First().PeriodStats.Count;
        var grid = new Grid();
        grid.AddColumns(4 + periods);
        var titles = new List<string> { "Player", "Points", "Fouls", "Minutes" };
        for (var i = 1; i <= periods; i++)
        {
            titles.Add($"P{i}");
        }

        grid.AddRow(titles.ToArray());

        var players = new List<Player>();

        await using var db = new StatsDbContext();
        // Load players data
        foreach (var stat in stats)
        {
            var player = players.FirstOrDefault(p => p.Id == stat.PlayerLicenseId);
            if (player is null)
            {
                player = await db.Players.FindAsync(stat.PlayerLicenseId) ?? new Player
                { Id = stat.PlayerLicenseId, Name = $"Unknown {stat.PlayerLicenseId}" };
                players.Add(player);
            }

            string[] data =
            [
                player.Name, stat.TotalPoints.ToString(), stat.TotalFouls.ToString(), stat.TimePlayed()
            ];
            data = infoToView switch
            {
                ViewStatsInfo.Points => data.Concat(stat.PeriodStats.Select(p => p.Points.ToString())).ToArray(),
                ViewStatsInfo.Fouls => data.Concat(stat.PeriodStats.Select(p => p.Fouls.ToString())).ToArray(),
                _ => data.Concat(stat.PeriodStats.Select(p => p.TimePlayed())).ToArray(),
            };

            grid.AddRow(data);
        }


        AnsiConsole.Write(grid);

        return CommandResult.Ok;
    }

}
