namespace ChinookExplorer.Cli.InputOutput
{
    interface ITerminalInput
    {
        ConsoleKeyInfo ReadKey(bool intercept);
        string? ReadLine();
    }
}
