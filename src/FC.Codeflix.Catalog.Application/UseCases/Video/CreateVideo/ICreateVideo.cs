using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.CreateVideo;

public interface ICreateVideo : IRequestHandler<CreateVideoInput, VideoModelOutput>
{}
