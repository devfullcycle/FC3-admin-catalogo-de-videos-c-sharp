using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.UpdateVideo;

public interface IUpdateVideo 
    : IRequestHandler<UpdateVideoInput, VideoModelOutput>
{ }
