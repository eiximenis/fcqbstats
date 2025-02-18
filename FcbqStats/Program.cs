// See https://aka.ms/new-console-template for more information


using FcbqStats;
using FcbqStats.Data;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;

using (var db = new StatsDbContext())
{
    await db.Database.MigrateAsync();
}

AnsiConsole.Write(
    new FigletText("FCBQ")
        .LeftJustified()
        .Color(Color.Blue));

var commandHandler = new CommandHandler();
commandHandler.ShowCommands();


var exit = false;
do
{
    var cvalue = ConsoleReader.ReadCommandValue();
    if (!string.IsNullOrEmpty(cvalue.StrCmd))
    {
        var result = await commandHandler.HandleStrCommand(cvalue.StrCmd);
        if (result == CommandResult.Exit)
        {
            exit = true;
        }
    }
} while (!exit);
