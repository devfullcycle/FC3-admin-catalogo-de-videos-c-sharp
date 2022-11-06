using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.DeleteVideo;

public record DeleteVideoInput(Guid VideoId) : IRequest;
