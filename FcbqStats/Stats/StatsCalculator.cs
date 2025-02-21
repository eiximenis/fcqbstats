using System.Diagnostics;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using FcbqStats.Data;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Extensions.DependencyModel.Resolution;
using Spectre.Console;


public class PlayerMatchStats
{
    public long PlayerId { get; set; }
    public int  PlayerLicenseId { get; set; }
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
        return PlayingTimes.Sum(pt => (pt.StartMin * 60 + pt.StartSec) - (pt.EndMin * 60 + pt.EndSec));

    }
}
internal class StatsCalculator
{
    private const int ENTRA_CAMP = 112;
    private const int CISTELLA_1 = 92;
    private const int CISTELLA_2 = 93;
    private const int CISTELLA_3 = 94;
    private const int ERROR_CISTELLA_1 = 96;
    private const int FINAL_PERIODE = 116;
    private const int SURT_CAMP = 115;
    private const int SALT_GUANYAT = 178;
    private const int FALTA_PERSONAL = 159;
    private const int FALTA_PERSONAL_2_TIRS = 161;

    record MatchPeriod(int Index, int StartMin, int StartSec, int EndMin, int EndSec)
    {
        public bool HasData() => StartMin != -1 && StartSec != -1;

        public bool IsClosed() => EndMin != -1 && EndSec != -1;
    }

    private readonly List<MatchEvent> _events;
    private readonly Dictionary<long, PlayerMatchStats> _allPlayerStats = [];
    private readonly List<MatchPeriod> _periods = [];
    private int _numPeriods;
    public StatsCalculator(IEnumerable<MatchEvent> events)
    {
        _events = new List<MatchEvent>(events);
    }

    public IEnumerable<Statistics> GenerateStatistics()
    {
        // We can't rely on Timestamp ordering because sometimes some events
        // come with invalid timestamps
        // We must assume that events are "ordered" in the API response
        var sortedEvents = _events.OrderBy(e => e.LocalIndex);
        _numPeriods = _events.Count(evt => evt.MoveId == FINAL_PERIODE);
        foreach (var evt in sortedEvents)
        {
            ProcessEvent(evt);
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
                TotalSeconds = playerStats.TotalSeconds,
                PlayerLicenseId = playerStats.PlayerLicenseId

            };
            foreach (var periodStats in playerStats.PeriodStats)
            {
                var pstat = new PeriodStatistics();
                pstat.PeriodIndex = periodStats.PeriodIndex;
                pstat.Points = periodStats.Points;
                pstat.Fouls = periodStats.Foults;
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

    private void CloseAllPendingTimes()
    {
        foreach (var pstat in _allPlayerStats)
        {
            foreach (var periodStats in pstat.Value.PeriodStats)
            {
                if (periodStats.PlayingTimes.Count > 0 && !periodStats.PlayingTimes.Last().IsComplete())
                {
                    periodStats.EndLastPlayingTime(0, 0);
                }
            }
        }
    }


    private void ProcessEvent(MatchEvent evt)
    {

        if (_periods.Count == 0)
        {
            AddPeriod(evt);
        }
        var period = _periods[^1];
        if (period.IsClosed())
        {
            AddPeriod(evt);
            period = _periods[^1];
        }

        if (evt.TeamAction)
        {
            ProcessGlobalEvent(evt);
            return;
        }

        PlayerMatchStats playerStats = null!;
        if (_allPlayerStats.TryGetValue(evt.PlayerId, out var ps))
        {
            playerStats = ps;
        }
        else
        {
            playerStats = new PlayerMatchStats(_numPeriods);
            playerStats.PlayerLicenseId = evt.LicenseId;
            playerStats.PlayerId = evt.PlayerId;
            _allPlayerStats.Add(evt.PlayerId, playerStats);
        }

        var periodStats = playerStats.PeriodStats[period.Index];

        var currentPlayTime = periodStats.PlayingTimes.LastOrDefault();
        if (currentPlayTime is null)
        {
            currentPlayTime = evt.MoveId == ENTRA_CAMP ? PlayingTime.Start(evt.Min, evt.Sec) : PlayingTime.Start(period.StartMin, period.StartSec);
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
            case CISTELLA_3:
                {
                    periodStats.ThreePointAttempts++;
                    periodStats.ThreePointsMade++;
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

    private void ProcessGlobalEvent(MatchEvent evt)
    {
        switch (evt.MoveId)
        {
            case FINAL_PERIODE:
                {
                    CloseLastPeriod(evt);
                    CloseAllPendingTimes();
                    break;
                }
        }
    }

    private void CloseLastPeriod(MatchEvent evt)
    {
        _periods[^1] = _periods[^1] with { EndMin = evt.Min, EndSec = evt.Sec };
    }

    private void AddPeriod(MatchEvent evt)
    {
        _periods.Add(new MatchPeriod(_periods.Count, evt.Min, evt.Sec, -1, -1));
    }
};

