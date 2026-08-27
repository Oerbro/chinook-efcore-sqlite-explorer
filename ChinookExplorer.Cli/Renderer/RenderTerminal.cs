namespace ChinookExplorer.Cli.Renderer
{
    readonly record struct Duration(int Ms)
    {
        public override string ToString() => $"{Ms / 60000}:{Ms / 1000 % 60:D2}";
    }

    abstract record ViewRow
    {
        private ViewRow() { }
        public sealed record ArtistRow(int ArtistId, string? Name) : ViewRow;
        public sealed record AlbumRow(int AlbumId, string Title) : ViewRow;
        public sealed record TrackRow(int TrackId, string Name, string? Composer, Duration Duration) : ViewRow;
    }

    class RenderTerminal
    {
        internal string Table<T>(IReadOnlyList<T> rows, string tableStart, string promptAsk) where T : ViewRow
        {
            if (rows.Count == 0) return $"{tableStart}\nNothing to show.\n\nb back: ";

            var props = typeof(T).GetProperties();
            var headers = props.Select(p => p.Name).ToArray();
            var grid = rows.Select(r => props.Select(p => p.GetValue(r)?.ToString() ?? "N/A").ToArray()).ToArray();
            var widths = headers.Select((h, i) => Math.Max(h.Length, grid.Max(g => g[i].Length))).ToArray();

            var lines = new[]
                {
            tableStart,
            string.Join("  ", headers.Select((h, i) => h.PadRight(widths[i]))),
            new string('-', widths.Sum() + 2 * (widths.Length - 1))
                }
                .Concat(grid.Select(g => string.Join("  ", g.Select((c, i) => c.PadRight(widths[i])))));

            return string.Join("\n", lines) + "\n" + promptAsk;
        }

        internal string StartScreen()
        {
            return
                "Chinook Explorer\n" +
                "================\n\n" +
                "1 for List Artists\n" +
                "q to Exit\n\n" +
                "Write your option: ";
        }

        internal string Artists(IReadOnlyList<ViewRow.ArtistRow> artists, int pageNumber, int lastPage) =>
            Table(artists, "Artists", $"PageNumber: {pageNumber}/{lastPage}\n\nArtistId to open | n next page | p previous page | b back | q quit: ");

        internal string ArtistAlbums(IReadOnlyList<ViewRow.AlbumRow> albums) =>
            Table(albums, "Artist's Albums", "\nAlbumId to open | b back | q quit: ");

        internal string AlbumTracks(IReadOnlyList<ViewRow.TrackRow> tracks) =>
            Table(tracks, "Album Tracks", "\nb back | q quit: ");
    }
}
