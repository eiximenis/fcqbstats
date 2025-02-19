using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FcbqStats.Responses;

public record MatchEventResponseItem(
    int IdTeam,
    string ActorName,
    long ActorId,
    string ActorShirtNumber,
    int IdMove,
    string Move,
    int Min,
    int Sec,
    int Period,
    string Score,
    bool TeamAction,
    string Timestamp,
    int LicenseId,
    string EventUuid
);
