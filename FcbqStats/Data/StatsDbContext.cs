using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FcbqStats.Data;
public class StatsDbContext : DbContext
{
    public DbSet<Club> Clubs { get; set; }
    public DbSet<Player> Players{ get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<Match> Matches { get; set; }
    public DbSet<Statistics> Stats { get; set; }

    public string DbPath { get; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");

    public StatsDbContext()
    {
        // Get current assembly running folder
        var path = AppDomain.CurrentDomain.BaseDirectory;
        DbPath = System.IO.Path.Join(path, "fcbqstats.db");
    }
}
