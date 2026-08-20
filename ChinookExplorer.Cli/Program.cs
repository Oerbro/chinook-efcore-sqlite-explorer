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
async Task<List<ViewRow.ArtistRow>> LoadArtists(int page) =>
    await context.Artists
        .OrderBy(a => a.ArtistId)
        .Skip(PageSize * (page - 1))
        .Take(PageSize)
        .Select(a => new ViewRow.ArtistRow(a.ArtistId, a.Name ?? "N/A"))
        .ToListAsync();

async Task Show(Screen screen)
{
    Clear();
    switch (screen)
    {
        case Screen.StartScreen:
            terminal.StartScreen();
            break;

        case Screen.ArtistsScreen s:
            terminal.Artists(await LoadArtists(s.Page));
            break;
    }
}

Screen screen = new Screen.StartScreen();

while (true)
{
    await Show(screen);

    var command = KeyBinding.MakeCommand(ReadKey(intercept: true), selectedId: null);
    if (command is null) continue;

    var (signal, next) = interpreter.Apply(screen, command, lastArtistsPage: 5);
    if (signal == LoopSignal.Exit) break;

    screen = next;
}
