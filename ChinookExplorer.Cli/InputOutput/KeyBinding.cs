using ChinookExplorer.Cli.StateMachine;

namespace ChinookExplorer.Cli.InputOutput
{
    static class KeyBinding
    {
        public static Command? MakeCommand(ConsoleKeyInfo key, int? selectedId)
        {
            return key.Key switch
            {
                ConsoleKey.LeftArrow =>
                    new Command.Previous(),
                ConsoleKey.RightArrow =>
                    new Command.Next(),
                ConsoleKey.Enter when selectedId is int id =>
                    new Command.Select(id),
                ConsoleKey.Backspace =>
                    new Command.Back(),
                ConsoleKey.Q =>
                    new Command.Quit(),
                _ => null
            };
        }
    }
}
