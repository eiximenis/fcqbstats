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
    public Team LocalTeam { get; set; } = null!;

    public int AwayTeamId { get; set; }
    public Team AwayTeam { get; set; } = null!;
    public string Date { get; set; } = "";
    public string Hour { get; set; } = "";

    public string StatisticsFcbqSid { get; set; } = "";

    public int? StatisticsId { get; set; }
    public Statistics? Statistics { get; set; }
}
