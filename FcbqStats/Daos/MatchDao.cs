using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FcbqStats.Data;
using Microsoft.EntityFrameworkCore;

namespace FcbqStats.Daos;
class MatchDao
{
    private readonly StatsDbContext _db;

    public MatchDao(StatsDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Match>> GetMatchesOfTeam(int teamId)
    {
        return await _db.Matches
            .Where(m => m.LocalTeamId == teamId || m.AwayTeamId == teamId)
            .ToListAsync();
    }
}
