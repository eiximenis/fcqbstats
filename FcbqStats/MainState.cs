using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FcbqStats.Data;

namespace FcbqStats;
public static class MainState
{
    private static readonly Dictionary<int, IEnumerable<Statistics>> _loadedStats = [];

    public static Club? SelectedClub { get; set; }
    public static Team? SelectedTeam { get; set; }
    public static Match? SelectedMatch { get; set; }

    public static void AddStatistics(int matchId, IEnumerable<Statistics> stats)
    {
        if (_loadedStats.ContainsKey(matchId))
        {
            _loadedStats.Remove(matchId);
        }
        _loadedStats.Add(matchId, new List<Statistics>(stats));
    }

    public static IEnumerable<Statistics> GetStatsForMatch(int selectedMatchId)
    {
        return _loadedStats[selectedMatchId];
    }
}
