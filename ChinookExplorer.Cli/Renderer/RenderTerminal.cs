using static System.Console;

namespace ChinookExplorer.Cli.Renderer
{
    readonly record struct Row(int Id, IReadOnlyList<string> Cells);
    record ArtistRow(int Id, string Artist);
    record AlbumRow(int Id, string Album);
    record TrackRow(int Id, string Track, string Composer, string Length);

    class RenderTerminal
    {
        internal void Table<T>(IReadOnlyList<T> rows, string prompt)
        {
            if (rows.Count == 0) { WriteLine("Nothing to show."); return; }

            var props = typeof(T).GetProperties();

            string[] headers = props.Select(p => p.Name).ToArray();

            string[][] grid = rows
                .Select(r => props.Select(p => p.GetValue(r)?.ToString() ?? "").ToArray())
                .ToArray();

            int[] widths = headers
                .Select((h, i) => Math.Max(h.Length, grid.Max(g => g[i].Length)))
                .ToArray();

            WriteLine(string.Join("  ", headers.Select((h, i) => h.PadRight(widths[i]))));
            WriteLine(new string('-', widths.Sum() + 2 * (widths.Length - 1)));

            foreach (var g in grid)
                WriteLine(string.Join("  ", g.Select((c, i) => c.PadRight(widths[i]))));

            Write(prompt);
        }

        internal void StartScreen()
        {
            Clear();
            WriteLine("Chinook Explorer");
            WriteLine("================");
            WriteLine();
            WriteLine("1. List Artists");
            WriteLine("2. Exit");
            WriteLine();
            Write("Write your option: ");
        }

        internal void ArtistsScreen(List<Row> artists)
        {
            WriteLine($"ArtistsScreen Start");
            WriteLine(new string('-', 45));
            foreach (var artist in artists)
            {
                //WriteLine($"{artist.Id,-5} {artist.Label}");
            }
            Write("ArtistsScreen End");
        }

        internal void ArtistAlbumsScreen(List<Row> albums)
        {
            Clear();
        }

        internal void AlbumTracksScreen(int id)
        {
            Clear();
        }
    }
}
