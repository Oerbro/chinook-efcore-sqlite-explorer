namespace ChinookExplorer.Cli.StateMachine
{
    sealed class Interpreter
    {
        public (LoopSignal LoopSignal, Screen NextScreen) Apply(
            Screen screen,
            Command command,
            int lastArtistsPage)
        {
            return (screen, command) switch
            {
                (_, Command.Quit _) =>
                    (LoopSignal.Exit, screen),

                (Screen.StartScreen, Command.Next) =>
                    (LoopSignal.Continue, new Screen.ArtistsScreen(1)),

                (Screen.ArtistsScreen artists, Command.Next)
                    when artists.Page < lastArtistsPage =>
                    (LoopSignal.Continue, artists with { Page = artists.Page + 1 }),

                (Screen.ArtistsScreen artists, Command.Previous)
                    when artists.Page > 1 =>
                    (LoopSignal.Continue, artists with { Page = artists.Page - 1 }),

                (Screen.ArtistsScreen artists, Command.Select selection) =>
                    (
                        LoopSignal.Continue,
                        new Screen.ArtistAlbumsScreen(
                            selection.Id,
                            artists.Page)
                    ),

                (Screen.ArtistAlbumsScreen artist, Command.Select selection) =>
                    (
                        LoopSignal.Continue,
                        new Screen.AlbumTracksScreen(
                            selection.Id,
                            artist.ArtistId,
                            artist.ArtistsPage)
                    ),

                (Screen.AlbumTracksScreen album, Command.Back _) =>
                    (LoopSignal.Continue,
                        new Screen.ArtistAlbumsScreen(
                            album.ArtistId,
                            album.ArtistsPage)),

                (Screen.ArtistAlbumsScreen artist, Command.Back _) =>
                    (LoopSignal.Continue,
                        new Screen.ArtistsScreen(artist.ArtistsPage)),

                (Screen.ArtistsScreen _, Command.Back _) =>
                    (LoopSignal.Continue, new Screen.StartScreen()),

                _ => (LoopSignal.Continue, screen)
            };
        }
    }
}
