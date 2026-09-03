using ChinookExplorer.Cli.StateMachine;

namespace ChinookExplorer.Cli.InputOutput
{
    static class KeyBinding
    {
        public static Command? MakeCommand(string line) => line.Trim().ToLowerInvariant() switch
        {
            "q" => new Command.Quit(),
            "n" => new Command.Next(),
            "p" => new Command.Previous(),
            "b" => new Command.Back(),
            var s when int.TryParse(s, out var id) => new Command.Select(id),
            _ => null
        };
    }
}
