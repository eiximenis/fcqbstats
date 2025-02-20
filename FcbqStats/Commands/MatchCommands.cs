using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using FcbqStats;
using FcbqStats.Data;
using FcbqStats.HtmlParsing;
using FcbqStats.Responses;
using Spectre.Console;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

public class MatchCommands
{
    public static async Task<CommandResult> LoadStatsForMatch(IEnumerable<string> args)
    {
        if (MainState.SelectedMatch == null)
        {
            AnsiConsole.MarkupLine("No match selected, please run [white]/lm[/] to load a match to start");
            return CommandResult.Error;
        }

        var stats = await LoadStatsForMatchFromDb(MainState.SelectedMatch.Id);
        if (!stats.Any())
        {
            stats = await CreateAndSaveStatsForMatch(MainState.SelectedMatch.Id);
        }

        MainState.AddStatistics(MainState.SelectedMatch.Id, stats);
        return CommandResult.Ok;
    }

    private static async Task<IEnumerable<Statistics>> CreateAndSaveStatsForMatch(int matchId)
    {

        // Check if we have match events in database
        var events = await LoadMatchMovesFromDb(matchId);
        if (!events.Any())
        {
            AnsiConsole.MarkupLine("No match history in database. Trying to fetch data from FCBQ");
            events = await GetMatchMovesFromFcbq(matchId);
            AnsiConsole.MarkupLine("Data fetched succesfully and saved to database");
        }

        var statsCalculator = new StatsCalculator(events);
        var stats = statsCalculator.GenerateStatistics();
        await using var db = new StatsDbContext();
        db.Stats.AddRange(stats);
        await db.SaveChangesAsync();
        AnsiConsole.MarkupLine($"[green]Saved[/] stats for match [white]{matchId}[/]");
        return stats;
    }

    private static async Task<IEnumerable<MatchEvent>> GetMatchMovesFromFcbq(int matchId)
    {
        var matchDetails = await GetMatchDetailsFromFcbq(matchId);
        if (matchDetails is not null)
        {
            Console.WriteLine($"Found statistics id {matchDetails.statsUid} for match {MainState.SelectedMatch.Id}");
            await GenerateAndSaveEventsForMatch(matchDetails.statsUid, matchId);
            await using var db = new StatsDbContext();
            return await db.MatchEvents.Where(mv => mv.MatchId == matchId).ToListAsync();
        }

        Console.WriteLine("Statistics unavailable for match :(");
        return Array.Empty<MatchEvent>();

    }

    private static async Task GenerateAndSaveEventsForMatch(string statsUid, int matchId)
    {
        var fcbqEvents = await GetEventsForMatchFromFcbq(statsUid);
        await using var db = new StatsDbContext();
        foreach (var evt in fcbqEvents)
        {
            // Convert to MatchEvent
            var dbEvent = new MatchEvent()
            {
                Id = evt.EventUuid,
                MatchId = matchId,
                ActorShirtNumber = evt.ActorShirtNumber,
                MoveId = evt.IdMove,
                MoveDesc = evt.Move,
                TeamId = evt.IdTeam,
                Min = evt.Min,
                Sec = evt.Sec,
                Score = evt.Score,
                TeamAction = evt.TeamAction,
                PlayerId = evt.ActorId,
                Period = evt.Period,
                Timestamp = DateTime.ParseExact(evt.Timestamp, "yyyyMMddHHmmss", null)
            };
            db.MatchEvents.Add(dbEvent);
            if (evt.ActorId !=0)
            {
                var dbPlayer = await db.Players.FindAsync(evt.ActorId);
                if (dbPlayer is null)
                {
                    var player = new Player()
                    {
                        Id = evt.ActorId,
                        Name = evt.ActorName
                    };
                    db.Players.Add(player);
                }
            }
        }
        var saved = await db.SaveChangesAsync();

        AnsiConsole.MarkupLine($"[green]Saved [white]{saved}[/] events for match {matchId}[/]");
    }

    private static async Task<IEnumerable<MatchEventResponseItem>> GetEventsForMatchFromFcbq(string statsUid)
    {
        var url = $"https://msstats.optimalwayconsulting.com/v1/fcbq/getJsonWithMatchMoves/{statsUid}/?currentSeason=true";
        var client = new HttpClient();
        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<MatchEventResponseItem[]>(json, JsonSerializerOptions.Web);
        return data;
    }

    private static async Task<MatchDetailParser.MatchDetails?> GetMatchDetailsFromFcbq(int matchId)
    {
        var parser = new MatchDetailParser();
        var matchDetails = await parser.ParseContent(MainState.SelectedMatch.Id);
        return matchDetails;
    }



    private static async Task<IEnumerable<Statistics>> LoadStatsForMatchFromDb(int matchId)
    {
        await using var db = new StatsDbContext();
        return await db.Stats.Where(s => s.MatchId == matchId).ToListAsync();
    }


    private static async Task<IEnumerable<MatchEvent>> LoadMatchMovesFromDb(int matchId)
    {
        await using var db = new StatsDbContext();
        var moves = await db.MatchEvents.Where(mv => mv.MatchId == matchId).ToListAsync();
        return moves;
    }

    public static async Task<CommandResult> DeleteStats(IEnumerable<string> arg)
    {
        if (MainState.SelectedMatch == null)
        {
            AnsiConsole.MarkupLine("No match selected, please run [white]/lm[/] to load a match to start");
            return CommandResult.Error;
        }

        await using var db = new StatsDbContext();
        var stats = await db.Stats.Where(s => s.MatchId == MainState.SelectedMatch.Id).ToListAsync();
        db.Stats.RemoveRange(stats);
        await db.SaveChangesAsync();
        AnsiConsole.MarkupLine($"[red]Deleted[/] stats for match [white]{MainState.SelectedMatch.Id}[/]");
        return CommandResult.Ok;
    }
}