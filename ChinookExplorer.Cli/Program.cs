using ChinookExplorer.Data.Persistence;
using ChinookExplorer.Cli.StateMachine;
using Microsoft.EntityFrameworkCore;
using ChinookExplorer.Cli.Renderer;
using static System.Console;

var databasePath = Path.Combine(
    AppContext.BaseDirectory,
    "database",
    "Chinook_Sqlite.sqlite");

var options = new DbContextOptionsBuilder<ChinookContext>()
    .UseSqlite($"Data Source={databasePath};Mode=ReadOnly")
    .Options;

await using var context = new ChinookContext(options);

var interpreter = new Interpreter();
var terminal = new RenderTerminal();

while (true)
{
    Clear();
    var artists = await context.Artists
    .AsNoTracking()
    .OrderBy(artist => artist.Name)
    .Take(20)
    .ToListAsync();

    Start:
    terminal.StartScreen();
    WriteLine(); Write("Write your option: ");
    var readLine = int.TryParse(ReadLine(), out int choice);
    if (choice == 1)
    {
        WriteLine($"{"ID",-5} Artist");
        WriteLine(new string('-', 45));
        foreach (var artist in artists)
        {
            WriteLine($"{artist.ArtistId,-5} {artist.Name}");
        }
        Write("Write your artistId: ");
        if (int.TryParse(ReadLine(), out int artistId))
        {
            var _art = artists.First(a => a.ArtistId == artistId);
            WriteLine(_art.Name);
            WriteLine("Press any key to go back to the main menu..."); 
            ReadKey();
            goto Start;
        }
    }
    if (choice == 2)
    {
        break;
    }
}
