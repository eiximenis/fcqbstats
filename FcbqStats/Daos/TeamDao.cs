using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FcbqStats.Data;

namespace FcbqStats.Daos;
class TeamDao
{
    private readonly StatsDbContext _db;
    public TeamDao(StatsDbContext db)
    {
        _db = db;
    }

    public Team? GetTeamById(int teamId)
    {
        return _db.Teams.FirstOrDefault(t => t.Id == teamId);
    }

}
