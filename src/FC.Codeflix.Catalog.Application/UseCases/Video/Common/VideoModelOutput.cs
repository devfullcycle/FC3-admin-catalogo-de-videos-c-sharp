using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Domain.Extensions;
using DomainEntities = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.Common;

public record VideoModelOutput(
    Guid Id,
    DateTime CreatedAt,
    string Title,
    bool Published,
    string Description,
    string Rating,
    int YearLaunched,
    bool Opened,
    int Duration,
    IReadOnlyCollection<Guid> CategoriesIds,
    IReadOnlyCollection<Guid> GenresIds,
    IReadOnlyCollection<Guid> CastMembersIds,
    string? ThumbFileUrl,
    string? BannerFileUrl,
    string? ThumbHalfFileUrl,
    string? VideoFileUrl,
    string? TrailerFileUrl)
{
    public static VideoModelOutput FromVideo(DomainEntities.Video video) => new(
        video.Id,
        video.CreatedAt,
        video.Title,
        video.Published,
        video.Description,
        video.Rating.ToStringSignal(),
        video.YearLaunched,
        video.Opened,
        video.Duration,
        video.Categories,
        video.Genres,
        video.CastMembers,
        video.Thumb?.Path,
        video.Banner?.Path,
        video.ThumbHalf?.Path,
        video.Media?.FilePath,
        video.Trailer?.FilePath);
}

