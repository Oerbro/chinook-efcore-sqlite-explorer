using ChinookExplorer.Cli.Renderer;
using ChinookExplorer.Data.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ChinookExplorer.Cli.Catalog
{
    sealed class DataLoader(ChinookContext context)
    {
        const int PageSize = 20;

        private static int LastPage(int rowCount) =>
            Math.Max(1, (int)Math.Ceiling(rowCount / (double)PageSize));

        public async Task<int> LastArtistsPage() =>
            LastPage(await context.Artists.CountAsync());

        public Task<List<ViewRow.ArtistRow>> LoadArtists(int page) =>
            context.Artists
                .OrderBy(a => a.ArtistId)
                .Select(a => new ViewRow.ArtistRow(a.ArtistId, a.Name))
                .Skip(PageSize * (page - 1))
                .Take(PageSize)
                .ToListAsync();

        public Task<List<ViewRow.AlbumRow>> LoadAlbums(int artistId) =>
            context.Albums
                .Where(a => a.ArtistId == artistId)
                .OrderBy(a => a.Title)
                .Select(a => new ViewRow.AlbumRow(a.AlbumId, a.Title))
                .ToListAsync();

        public Task<List<ViewRow.TrackRow>> LoadTracks(int albumId) =>
            context.Tracks
                .Where(t => t.AlbumId == albumId)
                .OrderBy(t => t.TrackId)
                .Select(t => new ViewRow.TrackRow(t.TrackId, t.Name, t.Composer, new Duration(t.Milliseconds)))
                .ToListAsync();
    }
}