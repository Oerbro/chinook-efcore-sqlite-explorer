using ChinookExplorer.Cli.StateMachine;

namespace ChinookExplorer.Cli.InputOutput
{
    class Navigator
    {
        private readonly ITerminalInput _input;
        private readonly TextWriter _output;

        public Navigator(ITerminalInput input, TextWriter output)
        {
            _input = input;
            _output = output;
        }

        public Command? ReadCommand()
        {
            ConsoleKeyInfo key = _input.ReadKey(intercept: true);

            int? selectedId = null;

            if (key.Key == ConsoleKey.Enter)
            {
                _output.Write("ID: ");

                if (!int.TryParse(_input.ReadLine(), out int id))
                {
                    _output.WriteLine("Invalid ID.");
                    return null;
                }

                selectedId = id;
            }

            return KeyBinding.MakeCommand(key, selectedId);
        }
    }
}
