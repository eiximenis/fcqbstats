using Spectre.Console;

class ConsoleReader
{
    public static (int Option,  string StrCmd) ReadCommandValue()
    {

        var line = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter command:")
        );

        if (int.TryParse(line, out var ov))
        {
            return (ov, "");
        }
        else
        {
            return (-1, line ?? "");
        }
    }

    public static int ReadIntValue()
    {
        var input = Console.ReadLine();
        return int.TryParse(input, out var ov) ? ov : -1;
    }
}