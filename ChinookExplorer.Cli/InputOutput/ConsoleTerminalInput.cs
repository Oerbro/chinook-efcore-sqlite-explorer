namespace ChinookExplorer.Cli.InputOutput
{
    sealed class ConsoleTerminalInput : ITerminalInput
    {
        public ConsoleKeyInfo ReadKey(bool intercept)
            => Console.ReadKey(intercept);

        public string? ReadLine()
            => Console.ReadLine();
    }
}
