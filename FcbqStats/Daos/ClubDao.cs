using FcbqStats.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FcbqStats.Daos;
class ClubDao
{
    private readonly StatsDbContext _db;
    public ClubDao(StatsDbContext db)
    {
        _db = db;
    }

    public Club? GetClubById(int clubId)
    {
        return _db.Clubs.FirstOrDefault(t => t.Id == clubId);
    }

    public async Task<IEnumerable<Team>> GetTeamsForClub(int clubId, string? filter = null)
    {
        return string.IsNullOrWhiteSpace(filter)
            ? await _db.Teams.Where(t => t.ClubId == clubId).ToListAsync()
            : await _db.Teams.Where(t => t.ClubId == clubId && t.Name.Contains(filter)).ToListAsync();
    }

    public async Task<IEnumerable<Club>> GetAllClubs(string? filter = null)
    {
        return string.IsNullOrWhiteSpace(filter) ? await _db.Clubs.ToListAsync() : await _db.Clubs.Where(c => c.Name.Contains(filter)).ToListAsync();
    }
}
