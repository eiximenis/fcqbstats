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
    public static async Task<CommandResult> View(IEnumerable<string> arg)
    {
        if (MainState.SelectedMatch == null)
        {
            AnsiConsole.MarkupLine("No match selected, please run [white]/lm[/] to load a match");
            return CommandResult.Error;
        }

        var stats = MainState.GetStatsForMatch(MainState.SelectedMatch.Id);

        var grid = new Grid();
        grid.AddColumn();
        grid.AddColumn();
        grid.AddColumn();
        grid.AddColumn();
        grid.AddRow(new string[] { "Player", "Points", "Fouls", "Seconds" });

        var players = new List<Player>();

        await using var db = new StatsDbContext();
        foreach (var stat in stats)
        {
            var player = players.FirstOrDefault(p => p.Id == stat.PlayerId);
            if (player is not null)
            {
                player = await db.Players.FindAsync(stat.PlayerId);
                players.Add(player);
            }
            grid.AddRow(new string[]
            {
                player?.Name ?? "Unknown",
                stat.TotalPoints.ToString(),
                stat.TotalFouls.ToString(),
                stat.TotalSeconds.ToString()
            }); 
        }

        AnsiConsole.Write(grid);


        return CommandResult.Ok;
    }
}
