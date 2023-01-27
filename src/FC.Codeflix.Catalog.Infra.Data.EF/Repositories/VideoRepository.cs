using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Domain.SeedWork;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Repositories;

public class VideoRepository : IVideoRepository
{
    private readonly CodeflixCatalogDbContext _context;
    private DbSet<Video> _videos => _context.Set<Video>();
    private DbSet<Media> _medias => _context.Set<Media>();
    private DbSet<VideosCategories> _videosCategories 
        => _context.Set<VideosCategories>();
    private DbSet<VideosGenres> _videosGenres
        => _context.Set<VideosGenres>();
    private DbSet<VideosCastMembers> _videosCastMembers
            => _context.Set<VideosCastMembers>();

    public VideoRepository(CodeflixCatalogDbContext context)
        => _context = context;


    public async Task Insert(Video video, CancellationToken cancellationToken)
    {
        await _videos.AddAsync(video, cancellationToken);

        if(video.Categories.Count > 0)
        {
            var relations = video.Categories
                .Select(categoryId => new VideosCategories(
                    categoryId,
                    video.Id
                ));
            await _videosCategories.AddRangeAsync(relations);
        }
        if(video.Genres.Count > 0)
        {
            var relations = video.Genres
                .Select(genreId => new VideosGenres(
                    genreId,
                    video.Id
                ));
            await _videosGenres.AddRangeAsync(relations);
        }
        if(video.CastMembers.Count > 0)
        {
            var relations = video.CastMembers
                .Select(castMemberId => new VideosCastMembers(
                    castMemberId,
                    video.Id
                ));
            await _videosCastMembers.AddRangeAsync(relations);
        }
    }

    public async Task Update(Video video, CancellationToken cancellationToken)
    {
        _videos.Update(video);

        _videosCategories.RemoveRange(_videosCategories
            .Where(x => x.VideoId == video.Id));
        _videosCastMembers.RemoveRange(_videosCastMembers
            .Where(x => x.VideoId == video.Id));
        _videosGenres.RemoveRange(_videosGenres
            .Where(x => x.VideoId == video.Id));

        if(video.Categories.Count > 0)
        {
            var relations = video.Categories
                .Select(categoryId => new VideosCategories(
                    categoryId,
                    video.Id
                ));
            await _videosCategories.AddRangeAsync(relations);
        }
        if(video.Genres.Count > 0)
        {
            var relations = video.Genres
                .Select(genreId => new VideosGenres(
                    genreId,
                    video.Id
                ));
            await _videosGenres.AddRangeAsync(relations);
        }
        if(video.CastMembers.Count > 0)
        {
            var relations = video.CastMembers
                .Select(castMemberId => new VideosCastMembers(
                    castMemberId,
                    video.Id
                ));
            await _videosCastMembers.AddRangeAsync(relations);
        }
    }

    public Task Delete(Video video, CancellationToken cancellationToken)
    {
        _videosCategories.RemoveRange(_videosCategories
            .Where(x => x.VideoId == video.Id));
        _videosCastMembers.RemoveRange(_videosCastMembers
            .Where(x => x.VideoId == video.Id));
        _videosGenres.RemoveRange(_videosGenres
            .Where(x => x.VideoId == video.Id));

        if(video.Trailer is not null)
            _medias.Remove(video.Trailer);

        if(video.Media is not null)
            _medias.Remove(video.Media);

        _videos.Remove(video);
        return Task.CompletedTask;
    }

    public async Task<Video> Get(Guid id, CancellationToken cancellationToken)
    {
        var video = await _videos.FirstOrDefaultAsync(video => video.Id == id);
        NotFoundException.ThrowIfNull(video, $"Video '{id}' not found.");

        var categoryIds = await _videosCategories
            .Where(x => x.VideoId == video!.Id)
            .Select(x => x.CategoryId)
            .ToListAsync(cancellationToken);
        categoryIds.ForEach(video!.AddCategory);

        var genresIds = await _videosGenres
            .Where(x => x.VideoId == video!.Id)
            .Select(x => x.GenreId)
            .ToListAsync(cancellationToken);
        genresIds.ForEach(video!.AddGenre);

        var castMembersIds = await _videosCastMembers
            .Where(x => x.VideoId == video!.Id)
            .Select(x => x.CastMemberId)
            .ToListAsync(cancellationToken);
        castMembersIds.ForEach(video!.AddCastMember);

        return video;
    }

    public async Task<SearchOutput<Video>> Search(SearchInput input, CancellationToken cancellationToken)
    {
        var toSkip = (input.Page - 1) * input.PerPage;
        var query = _videos.AsNoTracking();

        var count = query.Count();
        var items = await query.Skip(toSkip).Take(input.PerPage)
            .ToListAsync(cancellationToken);
        
        return new(
            input.Page, 
            input.PerPage, 
            count,
            items);
    }
}
