using ChinookExplorer.Data.Persistence;
using Microsoft.EntityFrameworkCore;

var databasePath = Path.Combine(
    AppContext.BaseDirectory,
    "database",
    "Chinook_Sqlite.sqlite");

var options = new DbContextOptionsBuilder<ChinookContext>()
    .UseSqlite($"Data Source={databasePath};Mode=ReadOnly")
    .Options;

await using var context = new ChinookContext(options);

var artists = await context.Artists
    .AsNoTracking()
    .OrderBy(artist => artist.Name)
    .Take(20)
    .ToListAsync();

Console.WriteLine($"{"ID",-5} Artist");
Console.WriteLine(new string('-', 45));

foreach (var artist in artists)
{
    Console.WriteLine($"{artist.ArtistId,-5} {artist.Name}");
}
