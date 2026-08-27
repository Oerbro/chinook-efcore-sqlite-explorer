using ChinookExplorer.Cli.InputOutput;
using ChinookExplorer.Cli.Renderer;
using ChinookExplorer.Cli.StateMachine;
using ChinookExplorer.Data.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static System.Console;

var databasePath = Path.Combine(
    AppContext.BaseDirectory,
    "database",
    "Chinook_Sqlite.sqlite");

var builder = new DbContextOptionsBuilder<ChinookContext>()
    .UseSqlite($"Data Source={databasePath};Mode=ReadOnly");

#if DEBUG
var logPath = Path.Combine(AppContext.BaseDirectory, "ef.log");
builder.LogTo(s =>
{
    System.Diagnostics.Debug.WriteLine(s);
    File.AppendAllText(logPath, s + Environment.NewLine);
}, LogLevel.Information);
#endif

var options = builder.Options;

await using var context = new ChinookContext(options);

var interpreter = new Interpreter();
var terminal = new RenderTerminal();

const int PageSize = 20;

//===========================Artist rows===========================
IQueryable<ViewRow.ArtistRow> artistRows = context.Artists
    .OrderBy(a => a.ArtistId)
    .Select(a => new ViewRow.ArtistRow(a.ArtistId, a.Name));

var artistCount = await artistRows.CountAsync();
var lastArtistsPage = Math.Max(1, (int)Math.Ceiling(artistCount / (double)PageSize));

Task<List<ViewRow.ArtistRow>> LoadArtists(int page) =>
    artistRows.Skip(PageSize * (page - 1)).Take(PageSize).ToListAsync();

//===========================Album rows===========================
Task<List<ViewRow.AlbumRow>> LoadAlbums(int artistId) =>
    context.Albums
        .Where(a => a.ArtistId == artistId)
        .OrderBy(a => a.Title)
        .Select(a => new ViewRow.AlbumRow(a.AlbumId, a.Title))
        .ToListAsync();

//===========================Track rows===========================
Task<List<ViewRow.TrackRow>> LoadTracks(int albumId) =>
    context.Tracks
        .Where(t => t.AlbumId == albumId)
        .OrderBy(t => t.TrackId)
        .Select(t => new ViewRow.TrackRow(
            t.TrackId,
            t.Name,
            t.Composer,
            new Duration(t.Milliseconds)))
        .ToListAsync();

async Task<string> View(Screen screen) => screen switch
{
    Screen.StartScreen => terminal.StartScreen(),
    Screen.ArtistsScreen s => terminal.Artists(await LoadArtists(s.Page), s.Page, lastArtistsPage),
    Screen.ArtistAlbumsScreen s => terminal.ArtistAlbums(await LoadAlbums(s.ArtistId)),
    Screen.AlbumTracksScreen s => terminal.AlbumTracks(await LoadTracks(s.AlbumId)),
    _ => ""
};

Screen screen = new Screen.StartScreen();
Screen? shown = null;
var view = "";

while (true)
{
    if (!screen.Equals(shown)) { view = await View(screen); shown = screen; }

    if (!IsOutputRedirected) Clear();
    Write(view);

    var line = ReadLine();
    if (line is null) break;

    var command = KeyBinding.MakeCommand(line);
    if (command is null) continue;

    var (signal, next) = interpreter.Apply(screen, command, lastArtistsPage);
    if (signal == LoopSignal.Exit) break;

    screen = next;
}
