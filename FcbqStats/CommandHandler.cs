using System.ComponentModel.DataAnnotations;
using FcbqStats;
using FcbqStats.Commands;
using FcbqStats.Data;
using Spectre.Console;

record CommandInfo (string Desc, Func<IEnumerable<string>, Task<CommandResult>> CommandFunc);

public class CommandHandler
{

    private Dictionary<string, CommandInfo> _commands = new Dictionary<string, CommandInfo>()
    {
        ["/lc"] = new CommandInfo("Reload clubs from FCBQ", ClubCommands.RefreshClubsList),
        ["/sc"] = new CommandInfo("Select club", ClubCommands.SelectClub),
        ["/st"] = new CommandInfo("Select team", ClubCommands.SelectTeam),
        ["/lm"] = new CommandInfo("Load Matches", TeamCommands.RefreshCalendar),
        [":q"] = new CommandInfo("Exit app", CoreCommands.Exit)
    };


    public async Task<CommandResult> HandleStrCommand(string line)
    {
        var tokens = line.Split(' ');
        if (_commands.TryGetValue(tokens[0], out var cmdInfo))
        {
            return await cmdInfo.CommandFunc(tokens.Skip(1));
        }
        else
        {
            Console.WriteLine("Not implemented command");
            return CommandResult.Error;
        }
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

class CoreCommands
{
    public static Task<CommandResult> Exit(IEnumerable<string> parameters) => Task.FromResult(CommandResult.Exit);
}