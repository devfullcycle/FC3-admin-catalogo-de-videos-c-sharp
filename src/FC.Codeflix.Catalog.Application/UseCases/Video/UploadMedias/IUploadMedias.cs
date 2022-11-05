using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.UploadMedias;

public interface IUploadMedias : IRequestHandler<UploadMediasInput>
{ }
