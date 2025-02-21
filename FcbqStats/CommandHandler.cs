using System.ComponentModel.DataAnnotations;
using System.Reflection.Emit;
using System.Security.AccessControl;
using FcbqStats;
using FcbqStats.Commands;
using Spectre.Console;

record CommandInfo(string Desc, Func<IEnumerable<string>, Task<CommandResult>> CommandFunc);

public class CommandHandler
{

    private readonly Dictionary<string, CommandInfo> _commands;
    public CommandHandler() 
    {
        _commands = new Dictionary<string, CommandInfo>()
        {
            ["/lc"] = new CommandInfo("Reload clubs from FCBQ", ClubCommands.RefreshClubsList),
            ["/sc"] = new CommandInfo("Select club", ClubCommands.SelectClub),
            ["/lt"] = new CommandInfo("Reload teams from selected club", ClubCommands.RefreshClubTeams),
            ["/st"] = new CommandInfo("Select team", ClubCommands.SelectTeam),
            ["/lm"] = new CommandInfo("Load Matches", TeamCommands.RefreshCalendar),
            ["/sm"] = new CommandInfo("Select Match", TeamCommands.SelectMatch),
            ["/ls"] = new CommandInfo("Load stats for match", MatchCommands.LoadStatsForMatch),
            ["/vs"] = new CommandInfo("View loaded stats", StatsCommands.View),
            ["!ds"] = new CommandInfo("Delete stats for current match", MatchCommands.DeleteStats),
            ["/help"] = new CommandInfo("Help", Help),
            ["?"] = new CommandInfo("Help", Help),
            [":status"] = new CommandInfo("View Status", CoreCommands.Status),
            [":q"] = new CommandInfo("Exit app", CoreCommands.Exit)
        };
    }
                                                                                                                                                                                                                                                                                                                                                                     

    private Task<CommandResult> Help(IEnumerable<string> arg)
    {
        ShowCommands();
        return Task.FromResult(CommandResult.Ok);
    }

    public async Task<CommandResult> HandleStrCommand(string line)
    {
        var tokens = line.Split(' ');
        if (_commands.TryGetValue(tokens[0], out var cmdInfo))
        {
            return await cmdInfo.CommandFunc(tokens.Skip(1));
        }

        AnsiConsole.MarkupLine("[red]Not implemented command[/]");
        return CommandResult.Error;
    }
    public void ShowCommands()
    {
        foreach (var cmd in _commands)
        {
            AnsiConsole.Markup($"[white]{cmd.Key} => [/] {cmd.Value.Desc}");
            Console.WriteLine();
        }
    }

}