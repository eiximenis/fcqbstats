using System.Diagnostics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using FcbqStats.Data;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Extensions.DependencyModel.Resolution;
using Spectre.Console;


public class PlayerMatchStats
{
    public long PlayerId { get; set; }
    public string PlayerName { get; set; }
    public string ShirtNumber { get; set; }

    public PlayerPeriodStats[] PeriodStats { get; }

    public PlayerMatchStats(int periodCount)
    {
        PeriodStats = new PlayerPeriodStats[periodCount];
        for (var idx = 0; idx < PeriodStats.Length; idx++)
        {
            PeriodStats[idx] = new PlayerPeriodStats() { PeriodIndex = idx };
        }
    }

    public int TotalPoints => PeriodStats.Sum(p => p.Points);
    public int TotalFoults => PeriodStats.Sum(p => p.Foults);
    public int TotalSeconds => PeriodStats.Sum(p => p.SecondsPlayed());
}

public record PlayingTime(int StartMin, int StartSec, int EndMin, int EndSec)
{
    public static PlayingTime Start(int min, int sec) => new PlayingTime(min, sec, -1, -1);

    public bool IsComplete() => EndMin != -1 && EndSec != -1;
}


public enum JumpStatus
{
    None,
    JumpWon,
    JumpLost
}
public class PlayerPeriodStats
{
    public int PeriodIndex { get; set; }
    public JumpStatus Jump { get; set; } = JumpStatus.None;
    public int OnePointAttempts { get; set; }
    public int OnePointMade { get; set; }
    public int TwoPointAttempts { get; set; }
    public int TwoPointsMade { get; set; }
    public int ThreePointAttempts { get; set; }
    public int ThreePointsMade { get; set; }
    public int Rebounds { get; set; }
    public int Assists { get; set; }
    public int Steals { get; set; }
    public int Blocks { get; set; }
    public List<PlayingTime> PlayingTimes { get; } = [];

    public int Points => ThreePointsMade * 3 + TwoPointsMade * 2 + OnePointMade;
    public int Foults { get; set; }

    public void EndLastPlayingTime(int min, int sec)
    {
        var lastPlayerTime = PlayingTimes[^1];
        var endedPlayerTime = lastPlayerTime with { EndMin = min, EndSec = sec };
        PlayingTimes[^1] = endedPlayerTime;
    }

    public int SecondsPlayed()
    {
        if (PlayingTimes.Count == 0)
        {
            return 0;
        }

        // TODO
        return PlayingTimes.Sum(pt => (pt.EndMin - pt.StartMin)*60);

    }
}
internal class StatsCalculator
{
    private const int ENTRA_CAMP = 112;
    private const  int CISTELLA_1 = 92;
    private const int CISTELLA_2 = 93;
    private const int ERROR_CISTELLA_1 = 96;
    private const int FINAL_PERIODE = 116;
    private const int SURT_CAMP = 115;
    private const int SALT_GUANYAT = 178;
    private const int FALTA_PERSONAL = 159;
    private const int FALTA_PERSONAL_2_TIRS = 161;

    record MatchPeriod(int Index, DateTime End, int StartMin, int StartSec)
    {
        public bool HasData() => StartMin != -1 && StartSec != -1;
    }

    private readonly List<MatchEvent> _events;
    private readonly Dictionary<long, PlayerMatchStats> _allPlayerStats = [];
    private readonly List<MatchPeriod> _periods = [];
    public StatsCalculator(IEnumerable<MatchEvent> events)
    {
        _events = new List<MatchEvent>(events);
    }

    public IEnumerable<Statistics> GenerateStatistics()
    {
        const int GLOBAL_EVENT = 0;
        var sortedEvents = _events.OrderBy(e => e.Timestamp);
        var globalEvents = sortedEvents.Where(e => e.PlayerId == GLOBAL_EVENT);
        ProcessGlobalEvents(globalEvents);

        foreach (var evt in sortedEvents.Where(e => e.PlayerId != GLOBAL_EVENT))
        {
            ProcessPlayerEvent(evt);
        }

        var dbStats = new List<Statistics>();

        foreach (var playerStats in _allPlayerStats.Values)
        {
            var stat = new Statistics()
            {
                MatchId = _events.First().MatchId,
                PlayerId = playerStats.PlayerId,
                TotalFouls = playerStats.TotalFoults,
                TotalPoints = playerStats.TotalPoints,
                TotalSeconds = playerStats.TotalSeconds
            };
            foreach (var periodStats in playerStats.PeriodStats)
            {
                var pstat = new PeriodStatistics();
                pstat.PeriodIndex = periodStats.PeriodIndex;
                pstat.Points = periodStats.Points;
                pstat.Foults = periodStats.Foults;
                pstat.OnePointAttempts = periodStats.OnePointAttempts;
                pstat.OnePointMade = periodStats.OnePointMade;
                pstat.TwoPointAttempts = periodStats.TwoPointAttempts;
                pstat.TwoPointsMade = periodStats.TwoPointsMade;
                pstat.ThreePointAttempts = periodStats.ThreePointAttempts;
                pstat.ThreePointsMade = periodStats.ThreePointsMade;
                pstat.OnePointPercent = periodStats.OnePointAttempts == 0
                    ? 0
                    : (periodStats.OnePointMade / periodStats.OnePointAttempts) * 100;
                pstat.TwoPointPercent = periodStats.TwoPointAttempts == 0
                    ? 0
                    : (periodStats.TwoPointsMade / periodStats.TwoPointAttempts) * 100;
                pstat.ThreePointPercent = periodStats.ThreePointAttempts == 0
                    ? 0
                    : (periodStats.ThreePointsMade / periodStats.ThreePointAttempts) * 100;
                pstat.SecondsPlayed = periodStats.SecondsPlayed();
                pstat.Statistics = stat;
                stat.PeriodStats.Add(pstat);
            }

            dbStats.Add(stat);
        }

        return dbStats;
    }

    private void ProcessGlobalEvents(IEnumerable<MatchEvent> globalEvents)
    {
        var pidx = 0;
        foreach (var ge in globalEvents)
        {
            switch (ge.MoveId)
            {
                case FINAL_PERIODE:
                    _periods.Add(new MatchPeriod(pidx++, ge.Timestamp, -1, -1));
                    break;
            }
        }
    }


    private void ProcessPlayerEvent(MatchEvent evt)
    {
        PlayerMatchStats playerStats = null!;
        if (_allPlayerStats.TryGetValue(evt.PlayerId, out var ps))
        {
            playerStats = ps;
        }
        else
        {
            playerStats = new PlayerMatchStats(_periods.Count);
            _allPlayerStats.Add(evt.PlayerId, playerStats);
        }

        var period = _periods.First(p => p.End >= evt.Timestamp);

        if (!period.HasData())
        {
            period = period with { StartMin = evt.Min, StartSec = evt.Sec };
            UpdatePeriod(period);
        }

        var periodStats = playerStats.PeriodStats[period.Index];

        var currentPlayTime = periodStats.PlayingTimes.LastOrDefault();
        if (currentPlayTime is null)
        {
            currentPlayTime = PlayingTime.Start(period.StartMin, period.StartSec);
            periodStats.PlayingTimes.Add(currentPlayTime);
        }
        else if (currentPlayTime.IsComplete())
        {
            currentPlayTime = PlayingTime.Start(evt.Min, evt.Sec);
            periodStats.PlayingTimes.Add(currentPlayTime);
        }


        switch (evt.MoveId)
        {
            case SALT_GUANYAT:
                {
                    periodStats.Jump = JumpStatus.JumpWon;
                    break;
                }
            case ENTRA_CAMP:
                {
                    break;
                }
            case SURT_CAMP:
                {
                    periodStats.EndLastPlayingTime(evt.Min, evt.Sec);
                    break;
                }
            case ERROR_CISTELLA_1:
            {
                periodStats.OnePointAttempts++;
                break;
            }
            case CISTELLA_1:
            {
                periodStats.OnePointAttempts++;
                periodStats.OnePointMade++;
                break;
            }
            case CISTELLA_2:
                {
                    periodStats.TwoPointAttempts++;
                    periodStats.TwoPointsMade++;
                    break;
                }
            case FALTA_PERSONAL:
            case FALTA_PERSONAL_2_TIRS:
            {
                periodStats.Foults++;
                break;
            }
        }

    }

    private void UpdatePeriod(MatchPeriod updatedPeriod)
    {
        _periods[updatedPeriod.Index] = updatedPeriod;
    }
};

