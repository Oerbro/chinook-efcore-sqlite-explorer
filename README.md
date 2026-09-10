# Chinook Explorer

Browse the [Chinook](https://github.com/lerocha/chinook-database) sample music catalogue from the terminal — artists, albums, tracks — built with .NET 10, Entity Framework Core and SQLite.

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/)
[![EF Core](https://img.shields.io/badge/EF%20Core-10.0-512BD4)](https://learn.microsoft.com/ef/core/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## Demo

```text
Artists
ArtistId  Name
-------------------------------------
1         AC/DC
2         Accept
3         Aerosmith
4         Alanis Morissette
5         Alice In Chains
...
PageNumber: 1/14

ArtistId to open | n next page | p previous page | b back | q quit: 1

Artist's Albums
AlbumId  Title
----------------------------------------------
1        For Those About To Rock We Salute You
4        Let There Be Rock

AlbumId to open | b back | q quit: 1

Album Tracks
TrackId  Name                                     Composer                                   Duration
-----------------------------------------------------------------------------------------------------
1        For Those About To Rock (We Salute You)  Angus Young, Malcolm Young, Brian Johnson  5:43
6        Put The Finger On You                    Angus Young, Malcolm Young, Brian Johnson  3:25
7        Let's Get It Up                          Angus Young, Malcolm Young, Brian Johnson  3:53
```

## Why this project

I built this to practise Entity Framework Core against a real relational schema, and to see how far a
console app gets when navigation is modelled explicitly instead of growing organically out of nested loops.

Three decisions the code is organised around:

- **Navigation is a state machine, not control flow.** Every screen is a `record` (`StartScreen`,
  `ArtistsScreen`, `ArtistAlbumsScreen`, `AlbumTracksScreen`) and every recognised line of input maps to a `Command`.
  `Interpreter.Apply` is a single pattern match from `(Screen, Command)` to the next screen — pure, with no
  I/O and no database access, so the whole navigation model can be unit tested without a database.
- **Screens carry the context needed to go back.** `AlbumTracksScreen` holds the `ArtistId` and the
  `ArtistsPage` it was reached from, so `b` returns you to the page you left rather than to page one.
  No navigation stack required.
- **The database is queried, not loaded.** Each screen projects straight into a small view record
  (`ArtistRow`, `AlbumRow`, `TrackRow`), and the artist list pages with `Skip`/`Take`, so EF Core
  translates a narrow `SELECT` per screen instead of materialising entities the renderer doesn't need. The connection is
  opened `Mode=ReadOnly`, and Debug builds log the generated SQL to `ef.log` next to the executable
  so the queries can be inspected.

## Getting started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)

The SQLite database ships with the repository, so there is nothing to seed or restore.

### Run

```bash
git clone https://github.com/Oerbro/chinook-efcore-sqlite-explorer.git
cd chinook-efcore-sqlite-explorer
dotnet run --project ChinookExplorer.Cli
```

## Usage

Type a command and press <kbd>Enter</kbd>.

| Input  | Action                    | Available on            |
| ------ | ------------------------- | ----------------------- |
| `1`    | List artists              | Start screen            |
| `<id>` | Open the row with that id | Artists, Albums         |
| `n`    | Next page                 | Artists                 |
| `p`    | Previous page             | Artists                 |
| `b`    | Back one level            | Artists, Albums, Tracks |
| `q`    | Quit                      | All screens             |

Artists are paged 20 rows at a time. Unrecognised input is reported above the screen; commands that don't apply to the current screen are silently ignored.

## Project structure

```text
ChinookExplorer.Cli/          Console application
  Program.cs                    Composition root: DbContext setup, render/read/interpret loop
  StateMachine/                 Screen and Command records, the Interpreter, LoopSignal
  Catalog/DataLoader.cs         EF Core queries and paging
  Renderer/RenderTerminal.cs    Screen text and the generic column-aligned table
  InputOutput/KeyBinding.cs     Raw input to Command
ChinookExplorer.Data/         Scaffolded EF Core model
  Models/                       10 entity classes for the 11 Chinook tables
  Persistence/ChinookContext.cs Fluent configuration; PlaylistTrack is mapped as a many-to-many join
ChinookExplorer.Tests/        xUnit test project
database/                     Chinook SQLite database and its SQL script
```

The EF Core model was scaffolded from the SQLite file with `dotnet ef dbcontext scaffold`; `dotnet-ef` is
pinned as a local tool in [dotnet-tools.json](dotnet-tools.json), so `dotnet tool restore` gets you the same version.

## Roadmap

- [ ] Unit tests for `Interpreter` state transitions
- [ ] CI workflow running `dotnet build` and `dotnet test`

## Credits

The [Chinook database](https://github.com/lerocha/chinook-database) is a sample database by Luis Rocha,
released under the MIT License (Copyright © 2008-2024 Luis Rocha).

## License

Released under the [MIT License](LICENSE).
