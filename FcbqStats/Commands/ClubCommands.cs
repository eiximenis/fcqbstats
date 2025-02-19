using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FcbqStats.Data;
using FcbqStats.Handlers;
using FcbqStats.HtmlParsing;
using FcbqStats.Responses;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;
using static System.Net.WebRequestMethods;

namespace FcbqStats.Commands;
internal class ClubCommands
{
    public static async Task<CommandResult> RefreshClubsList(IEnumerable<string> args)
    {
        await using var db = new StatsDbContext();
        await AnsiConsole.Progress().StartAsync(async ctx =>
        {
            var task1 = ctx.AddTask("[green]Loading current clubs...[/]");
            var task2 = ctx.AddTask("[green]Fetching clubs from FCBQ...[/]");
            var task3 = ctx.AddTask("[green]Synchronizing data...[/]");
            var url = "https://www.basquetcatala.cat/clubs/ajax";
            // Get current club data
            var allClubs = await db.Clubs.ToListAsync();
            task1.Increment(100);
            // Read Json using HttpClient
            var client = new HttpClient();
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<ClubsListItem[]>(json, JsonSerializerOptions.Web);
            task2.Increment(100);
            var delta = 100.0 / data.Length;
            foreach (var club in data)
            {
                if (allClubs.All(c => c.Id != club.Id))
                {
                    db.Clubs.Add(new Club
                    {
                        Id = club.Id,
                        ClubCode = club.ClubCode,
                        Name = club.Name
                    });
                }
                task3.Increment(delta);
            }
            task3.Increment(100);
            await db.SaveChangesAsync();
        });
        return CommandResult.Ok;
    }

    private static async Task<Club> SearchClub(string searchFilter)
    {
        await using var db = new StatsDbContext();
        var clubs = searchFilter is null ? await db.Clubs.ToListAsync() : await db.Clubs.Where(c => c.Name.Contains(searchFilter)).ToListAsync();
        var club = AnsiConsole.Prompt(
            new SelectionPrompt<Club>()
                .Title("Select a Club")
                .PageSize(10)
                .AddChoices(clubs)
                .UseConverter(c => $"{c.Id} - {c.Name}")
        );
        return club;
    }

    public static async Task<CommandResult> SelectClub(IEnumerable<string> parameters)
    {
        if (!parameters.Any())
        {
            await SelectClubInList();
            return CommandResult.Ok;
        }


        var clubId = int.TryParse(parameters.First(), out var cid) ? cid : -1;
        if (clubId == -1)
        {
            await SelectClubInList(parameters.First());
            return CommandResult.Ok;
        }

        await using var db = new StatsDbContext();
        var club = await db.Clubs.FindAsync(clubId);
        if (club == null)
        {
            Console.WriteLine("Club not found");
            return CommandResult.Error;
        }
        MainState.SelectedClub = club;
        Console.WriteLine($"Selected club: {MainState.SelectedClub.Id} - {MainState.SelectedClub!.Name}");
        return CommandResult.Ok;
    }

    private static async Task SelectClubInList(string? filter = null)
    {
        var club = await SearchClub(filter);
        MainState.SelectedClub = club;
        Console.WriteLine($"Selected club: {MainState.SelectedClub.Id} - {MainState.SelectedClub!.Name}");
    }

    public async static Task<CommandResult> SelectTeam(IEnumerable<string> parameters)
    {

        if (MainState.SelectedClub is null)
        {
            Console.WriteLine("No club selected. Please run /sc before");
            return CommandResult.Error;
        }

        if (!parameters.Any())
        {
            var team = await SelectTeamInList(MainState.SelectedClub);
            MainState.SelectedTeam = team;
            Console.WriteLine($"Team {team.Id} - {team.Name} selected!");
            return CommandResult.Ok;
        }

        var teamId = int.TryParse(parameters.First(), out var cid) ? cid : -1;
        if (teamId == -1)
        {
            var team = await SelectTeamInList(MainState.SelectedClub, parameters.First());
            MainState.SelectedTeam = team;
            Console.WriteLine($"Team {team.Id} - {team.Name} selected!");
        }
        else
        {
            await using var db = new StatsDbContext();
            var team = db.Teams.FirstOrDefault(t => t.Id == teamId);
            if (team?.ClubId is null || team.ClubId != MainState.SelectedClub.Id)
            {
                Console.WriteLine($"Error: The specified team do not exist or do not belong to club {MainState.SelectedClub.Name}");
                return CommandResult.Error;
            }
            MainState.SelectedTeam = team;
        }
        return CommandResult.Ok;
    }

    private static async Task<Team> SelectTeamInList(Club selectedClub, string? filter = null)
    {

        await using var db = new StatsDbContext();
        var teams = await (
            filter is null
                ? db.Teams.Where(t => t.ClubId == selectedClub.Id).ToListAsync()
                : db.Teams.Where(t => t.ClubId == selectedClub.Id && t.Name.Contains(filter)).ToListAsync());
        var team = AnsiConsole.Prompt(
            new SelectionPrompt<Team>()
                .Title("Select Team")
                .PageSize(10)
                .AddChoices(teams)
                .UseConverter(t => $"{t.Id} - {t.Name}")
        );
        return team;
    }

    public static async Task<CommandResult> RefreshClubTeams(IEnumerable<string> args)
    {
        if (MainState.SelectedClub is null)
        {
            Console.WriteLine("No club selected. Please run /sc before");
            return CommandResult.Error;
        }

        await using var db = new StatsDbContext();
        var added = await ClubHandler.RefreshClubTeams(MainState.SelectedClub.Id, db);
        await db.SaveChangesAsync();
        Console.WriteLine($"Added {added} teams");
        return CommandResult.Ok;
    }

}
