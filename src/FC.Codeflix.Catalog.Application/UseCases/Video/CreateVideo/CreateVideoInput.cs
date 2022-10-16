using FC.Codeflix.Catalog.Domain.Enum;
using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.CreateVideo;

public record CreateVideoInput  (
    string Title,
    string Description,
    int YearLaunched,
    bool Ppened,
    bool Published,
    int Duration,
    Rating Rating
) : IRequest<CreateVideoOutput>;