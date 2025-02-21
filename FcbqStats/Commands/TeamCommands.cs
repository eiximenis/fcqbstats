using FcbqStats;
using FcbqStats.Daos;
using FcbqStats.Data;
using FcbqStats.HtmlParsing;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;

internal class TeamCommands
{
    public static async Task<CommandResult> RefreshCalendar(IEnumerable<string>  _)
    {
        if (MainState.SelectedTeam is null)
        {
            Console.WriteLine("No team selected. Please run /st before");
            return CommandResult.Error;
        }
        using var db = new StatsDbContext();
        var matchDao = new MatchDao(db);
        var parser = new GlobalCalendarParser();
        var matches = await parser.ParseContent(MainState.SelectedClub!.Id, MainState.SelectedTeam!.Id);
        Console.WriteLine($"Found {matches.Count()} for team {MainState.SelectedTeam.Name}");
        var dbMatches = await matchDao.GetMatchesOfTeam(MainState.SelectedTeam.Id);
        var added = 0;
        var updated = 0;
        foreach (var match in matches)
        {
            var dbMatch = dbMatches.FirstOrDefault(m => m.Id == match.MatchId);
            if (dbMatch is null)
            {
                dbMatch = new Match
                {
                    Id = match.MatchId,
                    LocalTeamId = match.LocalId,
                    LocalTeamName = match.LocalName,
                    AwayTeamId = match.AwayId,
                    AwayTeamName = match.AwayName,
                    Date = match.Date,
                    Hour = match.Hour
                };
                db.Matches.Add(dbMatch);
                added++;
            }
            else
            {
                dbMatch.Date = match.Date;
                dbMatch.Hour = match.Hour;
                updated++;
            }
        }
        await db.SaveChangesAsync();
        Console.WriteLine($"Added {added} matches. Updated {updated} matches for team {MainState.SelectedTeam.Name}");
        return CommandResult.Ok; 

    }

    public static async Task<CommandResult> SelectMatch(IEnumerable<string> _)
    {
        if (MainState.SelectedTeam is null)
        {
            Console.WriteLine("No club selected. Please run /st before");
            return CommandResult.Error;
        }
        await using var db = new StatsDbContext();
        var matchDao = new MatchDao(db);
        var matches = await matchDao.GetMatchesOfTeam(MainState.SelectedTeam.Id);
        var match = AnsiConsole.Prompt(
            new SelectionPrompt<Match>()
                .Title("Select Match")
                .PageSize(10)
                .AddChoices(matches)
                .UseConverter(m => $"{m.Id} - {m.LocalTeamName} vs {m.AwayTeamName} - {m.Date} {m.Hour}")
        );

        MainState.SelectedMatch = match;

        return CommandResult.Ok;

    }
}