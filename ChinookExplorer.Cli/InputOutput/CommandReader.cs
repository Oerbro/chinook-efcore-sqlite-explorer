namespace ChinookExplorer.Cli.InputOutput
{
    class CommandReader
    {
        private readonly Func<ConsoleKeyInfo> _readKey;
        private readonly TextReader _textReader;
        private readonly TextWriter _textWriter;

        public CommandReader(Func<ConsoleKeyInfo> readKey, TextReader input, TextWriter output)
        {
            _readKey = readKey;
            _textReader = input;
            _textWriter = output;
        }
    }
}
