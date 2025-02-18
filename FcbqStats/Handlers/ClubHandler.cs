using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FcbqStats.Data;
using FcbqStats.HtmlParsing;
using Microsoft.EntityFrameworkCore;

namespace FcbqStats.Handlers;
internal class ClubHandler
{
    public static async Task<int> RefreshClubTeams(int clubId, StatsDbContext db)
    {
        var parser = new ClubPageParser();
        var teams = await parser.ParseContent(clubId);

        var dbTeams = await db.Teams.Where(t => t.ClubId == clubId).ToListAsync();
        foreach (var team in teams)
        {
            if (dbTeams.All(dbt => dbt.Id != team.Code))
            {
                db.Teams.Add(new Team()
                {
                    Id = team.Code,
                    Name = team.Text,
                    ClubId = clubId
                });
            }
        }

        return teams.Count();
    }

}
