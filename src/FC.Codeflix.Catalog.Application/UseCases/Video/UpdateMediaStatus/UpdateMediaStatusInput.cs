using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.Domain.Enum;
using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.UpdateMediaStatus;
public record UpdateMediaStatusInput(
    Guid VideoId,
    MediaStatus Status,
    string? EncodedPath = null,
    string? ErrorMessage = null
) : IRequest<VideoModelOutput>;
