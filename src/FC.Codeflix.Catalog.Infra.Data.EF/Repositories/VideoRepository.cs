using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using Microsoft.EntityFrameworkCore;

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

        DeleteOrphanMedias(video);
    }

    private void DeleteOrphanMedias(Video video)
    {
        if (_context.Entry(video).Reference(v => v.Trailer).IsModified)
        {
            var oldTrailerId = _context.Entry(video)
                .OriginalValues.GetValue<Guid?>($"{nameof(Video.Trailer)}Id");
            if (oldTrailerId != null && oldTrailerId != video.Trailer?.Id)
            {
                var oldTrailer = _medias.Find(oldTrailerId);
                _medias.Remove(oldTrailer!);
            }
        }

        if (_context.Entry(video).Reference(v => v.Media).IsModified)
        {
            var oldMediaId = _context.Entry(video)
                .OriginalValues.GetValue<Guid?>($"{nameof(Video.Media)}Id");
            if (oldMediaId != null && oldMediaId != video.Media?.Id)
            {
                var oldMedia = _medias.Find(oldMediaId);
                _medias.Remove(oldMedia!);
            }
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

        if(!string.IsNullOrWhiteSpace(input.Search))
            query = query.Where(video => video.Title.Contains(input.Search));
        query = InsertOrderBy(input, query);

        var count = query.Count();
        var items = await query.Skip(toSkip).Take(input.PerPage)
            .ToListAsync(cancellationToken);

        var videosIds = items.Select(video => video.Id).ToList();
        await AddCategoriesToVideos(items, videosIds);
        await AddGenresToVideos(items, videosIds);
        await AddCastMembersToVideos(items, videosIds);
        
        return new(
            input.Page,
            input.PerPage,
            count,
            items);
    }

    private async Task AddCategoriesToVideos(List<Video> items, List<Guid> videosIds)
    {
        var categoriesRelations = await _videosCategories
            .Where(relation => videosIds.Contains(relation.VideoId))
            .ToListAsync();
        var relationsWithCategoriesByVideoId =
            categoriesRelations.GroupBy(x => x.VideoId).ToList();
        relationsWithCategoriesByVideoId.ForEach(relationGroup =>
        {
            var video = items.Find(video => video.Id == relationGroup.Key);
            if(video is null) return;
            relationGroup.ToList()
                .ForEach(relation => video.AddCategory(relation.CategoryId));
        });
    }

    private async Task AddGenresToVideos(List<Video> items, List<Guid> videosIds)
    {
        var genresRelations = await _videosGenres
            .Where(relation => videosIds.Contains(relation.VideoId))
            .ToListAsync();
        var relationsWithGenresByVideoId =
            genresRelations.GroupBy(x => x.VideoId).ToList();
        relationsWithGenresByVideoId.ForEach(relationGroup =>
        {
            var video = items.Find(video => video.Id == relationGroup.Key);
            if(video is null) return;
            relationGroup.ToList()
                .ForEach(relation => video.AddGenre(relation.GenreId));
        });
    }

    private async Task AddCastMembersToVideos(List<Video> items, List<Guid> videosIds)
    {
        var castMembersRelations = await _videosCastMembers
            .Where(relation => videosIds.Contains(relation.VideoId))
            .ToListAsync();
        var relationsWithCastMembersByVideoId =
            castMembersRelations.GroupBy(x => x.VideoId).ToList();
        relationsWithCastMembersByVideoId.ForEach(relationGroup =>
        {
            var video = items.Find(video => video.Id == relationGroup.Key);
            if(video is null) return;
            relationGroup.ToList()
                .ForEach(relation => video.AddCastMember(relation.CastMemberId));
        });
    }

    private static IQueryable<Video> InsertOrderBy(SearchInput input, IQueryable<Video> query)
        => input switch
        {
            { Order: SearchOrder.Asc } when input.OrderBy.ToLower() is "title"
                => query.OrderBy(video => video.Title).ThenBy(video => video.Id),
            { Order: SearchOrder.Desc } when input.OrderBy.ToLower() is "title"
                => query.OrderByDescending(video => video.Title).ThenByDescending(video => video.Id),
            { Order: SearchOrder.Asc } when input.OrderBy.ToLower() is "id"
                => query.OrderBy(video => video.Id),
            { Order: SearchOrder.Desc } when input.OrderBy.ToLower() is "id"
                => query.OrderByDescending(video => video.Id),
            { Order: SearchOrder.Asc } when input.OrderBy.ToLower() is "createdat"
                => query.OrderBy(video => video.CreatedAt),
            { Order: SearchOrder.Desc } when input.OrderBy.ToLower() is "createdat"
                => query.OrderByDescending(video => video.CreatedAt),
            _ => query = query.OrderBy(video => video.Title).ThenBy(video => video.Id)
        };
}
