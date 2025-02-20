using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FcbqStats.Data;
public class Statistics
{
    public long Id { get; set; }
    public int MatchId { get; set; }
    public string FcbqSid { get; set; } = "";
    public long PlayerId { get; set; }
    public ICollection<PeriodStatistics> PeriodStats { get; } = [];
    public int TotalPoints { get; set; }
    public int TotalSeconds { get; set; }

    public int TotalFouls { get; set; }
}
