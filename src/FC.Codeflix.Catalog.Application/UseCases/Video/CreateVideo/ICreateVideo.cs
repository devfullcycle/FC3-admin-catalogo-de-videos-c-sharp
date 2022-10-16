using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.CreateVideo;

public interface ICreateVideo : IRequestHandler<CreateVideoInput, CreateVideoOutput>
{}
