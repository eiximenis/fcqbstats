using FcbqStats.Data;

internal class StatsCalculator
{
    private readonly List<MatchEvent> _events;
    public StatsCalculator(IEnumerable<MatchEvent> events)
    {
        _events = new List<MatchEvent>(events);
    }

    public IEnumerable<Statistics> GenerateStatistics()
    {
        var groupedEvents = _events.GroupBy(e => e.PlayerId);
        foreach (var playerId in groupedEvents.Select(g => g.Key))
        {
            var playerEvents = groupedEvents.Where(g => g.Key == playerId).SelectMany(g => g).OrderBy(m => m.Timestamp);
            int i = 0;

        }
        return Array.Empty<Statistics>();
    }
}