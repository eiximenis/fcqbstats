using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FcbqStats.Data;
public class PeriodStatistics
{
    public long Id { get; set; }
    public int PeriodIndex { get; set; }
    public int Points { get; set; }
    public int Fouls { get; set; }
    public int OnePointAttempts { get; set; }
    public int OnePointMade { get; set; }
    public int TwoPointAttempts { get; set; }
    public int TwoPointsMade { get; set; }
    public int ThreePointAttempts { get; set; }
    public int ThreePointsMade { get; set; }
    public int OnePointPercent { get; set; }
    public int TwoPointPercent { get; set; }
    public int ThreePointPercent { get; set; }
    public int SecondsPlayed { get; set; }

    public long StatisticsId { get; set; }
    public Statistics Statistics { get; set; }

    public string TimePlayed() => $"{SecondsPlayed / 60}m {SecondsPlayed % 60}s";
}
