using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.GetVideo;

public interface IGetVideo : IRequestHandler<GetVideoInput, VideoModelOutput>
{ }
