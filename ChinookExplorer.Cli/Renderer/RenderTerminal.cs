using static System.Console;

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
        internal void Table<T>(IReadOnlyList<T> rows, string tableStart, string promptAsk) where T : ViewRow
        {
            WriteLine(tableStart);

            if (rows.Count == 0) { WriteLine("Nothing to show. Type b to go back."); return; }

            var props = typeof(T).GetProperties();

            string[] headers = props.Select(p => p.Name).ToArray();

            string[][] grid = rows
                .Select(r => props.Select(p => p.GetValue(r)?.ToString() ?? "N/A").ToArray())
                .ToArray();

            int[] widths = headers
                .Select((h, i) => Math.Max(h.Length, grid.Max(g => g[i].Length)))
                .ToArray();

            WriteLine(string.Join("  ", headers.Select((h, i) => h.PadRight(widths[i]))));
            WriteLine(new string('-', widths.Sum() + 2 * (widths.Length - 1)));

            foreach (var g in grid)
                WriteLine(string.Join("  ", g.Select((c, i) => c.PadRight(widths[i]))));

            Write(promptAsk);
        }

        internal void StartScreen()
        {
            WriteLine("Chinook Explorer");
            WriteLine("================");
            WriteLine();
            WriteLine("1. List Artists");
            WriteLine("2. Exit");
            WriteLine();
            Write("Write your option: ");
        }

        internal void Artists(IReadOnlyList<ViewRow.ArtistRow> artists, int pageNumber, int lastPage) =>
            Table(artists, "Artists", $"PageNumber: {pageNumber}/{lastPage}\n\nArtistId to open | n next page | p previous page | b back | q quit: ");

        internal void ArtistAlbums(IReadOnlyList<ViewRow.AlbumRow> albums) =>
            Table(albums, "Artist's Albums", "\nAlbumId to open | b back | q quit: ");

        internal void AlbumTracks(IReadOnlyList<ViewRow.TrackRow> tracks) =>
            Table(tracks, "Album Tracks", "\nb back | q quit: ");
    }
}
