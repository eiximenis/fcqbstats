using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FcbqStats.Data;

public class Match
{
    public int Id { get; set; }
    public int LocalTeamId { get; set; }
    public string LocalTeamName { get; set; } = null!;

    public int AwayTeamId { get; set; }
    public string AwayTeamName { get; set; } = null!;
    public string Date { get; set; } = "";
    public string Hour { get; set; } = "";

    public string StatisticsFcbqSid { get; set; } = "";
    public ICollection<Statistics?> Statistics { get; set; }
}
