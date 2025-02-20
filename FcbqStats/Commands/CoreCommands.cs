using FcbqStats;
using FcbqStats.Config;
using FcbqStats.Data;
using FcbqStats.Extensions;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;

class CoreCommands
{
    public static async Task<CommandResult> Exit(IEnumerable<string> parameters)
    {
        var configManager = new CurrentConfigManager();
        await configManager.SaveMainState();
        return CommandResult.Exit;
    }

    public static async Task<CommandResult> Status(IEnumerable<string> arg)
    {
        await using var db = new StatsDbContext();
        var clubs = await db.Clubs.CountAsync();
        var teams = await db.Teams.CountAsync();
        var matches = await db.Matches.CountAsync();
        var players = await db.Players.CountAsync();

        if (clubs == 0)
        {
            AnsiConsole.MarkupLine("No clubs loaded, please run [white]/lc[/] to load clubs to start");
        }
        else
        {
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"[white]Loaded Clubs:[/] {clubs}.[white]Loaded Teams:[/] {teams}.[white] Loaded Players: {players}[/][white] Loaded Matches: {matches}[/]");
            if (MainState.SelectedClub != null)
            {
                AnsiConsole.MarkupLine($"[white]Selected Club:[/] {MainState.SelectedClub.Id} - {MainState.SelectedClub.Name}");
            }
            if (MainState.SelectedTeam != null)
            {
                AnsiConsole.MarkupLine($"[white]Selected Team:[/] {MainState.SelectedTeam.Id} - {MainState.SelectedTeam.Name}");
            }
            if (MainState.SelectedMatch != null)
            {
                AnsiConsole.MarkupLine($"[white]Selected Match:[/] {MainState.SelectedMatch.Id} - {MainState.SelectedMatch.LocalTeamName} vs {MainState.SelectedMatch.AwayTeamName}");
            }
            AnsiConsole.WriteLine();
        }

        return CommandResult.Ok;
    }
}