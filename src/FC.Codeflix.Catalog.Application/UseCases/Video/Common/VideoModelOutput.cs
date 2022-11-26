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
    IReadOnlyCollection<RelatedAggregate> Categories,
    IReadOnlyCollection<RelatedAggregate> Genres,
    IReadOnlyCollection<RelatedAggregate> CastMembers,

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
        video.Categories.Select(id => new RelatedAggregate(id)).ToList(),
        video.Genres.Select(id => new RelatedAggregate(id)).ToList(),
        video.CastMembers.Select(id => new RelatedAggregate(id)).ToList(),
        video.Thumb?.Path,
        video.Banner?.Path,
        video.ThumbHalf?.Path,
        video.Media?.FilePath,
        video.Trailer?.FilePath);
}

public record RelatedAggregate(Guid Id, string? Name = null);