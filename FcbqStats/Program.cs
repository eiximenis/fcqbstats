// See https://aka.ms/new-console-template for more information


using FcbqStats;
using FcbqStats.Config;
using FcbqStats.Data;
using FcbqStats.Extensions;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;

using (var db = new StatsDbContext())
{
    await db.Database.MigrateAsync();
}

var configManager = new CurrentConfigManager();
await configManager.FillMainState();

AnsiConsole.Write(
    new FigletText("FCBQ")
        .LeftJustified()
        .Color(Color.Blue));

var commandHandler = new CommandHandler();
commandHandler.ShowCommands();
await commandHandler.HandleStrCommand(":status");

var exit = false;
do
{
    var cvalue = ConsoleReader.ReadCommandValue();
    if (!string.IsNullOrEmpty(cvalue))
    {
        var result = await commandHandler.HandleStrCommand(cvalue);
        if (result == CommandResult.Exit)
        {
            exit = true;
        }
    }
} while (!exit);
