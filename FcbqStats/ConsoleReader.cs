using Spectre.Console;

class ConsoleReader
{
    public static string ReadCommandValue()
    {

        var line = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter command (? for help):")
        );

        return line;

    }

}