using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.UpdateMediaStatus;
public interface IUpdateMediaStatus
    : IRequestHandler<UpdateMediaStatusInput, VideoModelOutput>
{
}
