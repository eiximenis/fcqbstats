using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FcbqStats.Config;
using FcbqStats.Data;

namespace FcbqStats.Extensions;
static class CurrentConfigManagerExtensions
{
    public static async Task FillMainState(this CurrentConfigManager self)
    {
        var config = await self.Load();
        await using var db = new StatsDbContext();
        if (config.ClubId!= -1)
        {
            MainState.SelectedClub = await db.Clubs.FindAsync(config.ClubId);
        }
        if (config.TeamId != -1)
        {
            MainState.SelectedTeam = await db.Teams.FindAsync(config.TeamId);
        }
        if (config.MatchId != -1)
        {
            MainState.SelectedMatch = await db.Matches.FindAsync(config.MatchId);
        }
    }

    public static async Task SaveMainState(this CurrentConfigManager self)
    {
        await using var db = new StatsDbContext();
        var config = new CurrentConfig(MainState.SelectedClub?.Id ?? -1, MainState.SelectedTeam?.Id ?? -1,
            MainState.SelectedMatch?.Id ?? -1);
        await self.Save(config);
    }
}
