using FcbqStats;
using FcbqStats.Data;
using FcbqStats.HtmlParsing;
using Microsoft.EntityFrameworkCore;

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
        var parser = new GlobalCalendarParser();
        var matches = await parser.ParseContent(MainState.SelectedClub!.Id, MainState.SelectedTeam!.Id);
        Console.WriteLine($"Found {matches.Count()} for team {MainState.SelectedTeam.Name}");
        var dbMatches = await db.Matches.Where(m => m.LocalTeamId== MainState.SelectedTeam.Id || m.AwayTeamId == MainState.SelectedTeam.Id).ToListAsync();
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
                    AwayTeamId = match.AwayId,
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
}