using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FcbqStats.Data;
public class MatchEvent
{
    public string Id { get; set; }
    public int MatchId { get; set; }
    public int TeamId { get; set; }
    public long PlayerId { get; set; }
    public string ActorShirtNumber { get; set; }
    public int MoveId { get; set; }
    public string MoveDesc { get; set; }
    public int Min { get; set; }
    public int Sec { get; set; }
    public int Period { get; set; }
    public string Score { get; set; }
    public bool TeamAction { get; set; }
    public DateTime Timestamp { get; set; }

}
