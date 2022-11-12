using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.GetVideo;

public record GetVideoInput(Guid VideoId) : IRequest<GetVideoOutput>;
