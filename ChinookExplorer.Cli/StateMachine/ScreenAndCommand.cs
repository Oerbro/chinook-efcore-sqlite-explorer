namespace ChinookExplorer.Cli.StateMachine
{
    abstract record Screen
    {
        private Screen() { }

        public sealed record StartScreen : Screen;
        public sealed record ArtistsScreen(int Page) : Screen;
        public sealed record ArtistAlbumsScreen(int ArtistId, int ArtistsPage) : Screen;
        public sealed record AlbumTracksScreen(
            int AlbumId,
            int ArtistId,
            int ArtistsPage) : Screen;
    }

    abstract record Command
    {
        private Command() { }

        public sealed record Previous : Command;
        public sealed record Next : Command;
        public sealed record Select(int Id) : Command;
        public sealed record Back : Command;
        public sealed record Quit : Command;
    }
}
