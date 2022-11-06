using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.DeleteVideo;

public interface IDeleteVideo : IRequestHandler<DeleteVideoInput>
{ }
