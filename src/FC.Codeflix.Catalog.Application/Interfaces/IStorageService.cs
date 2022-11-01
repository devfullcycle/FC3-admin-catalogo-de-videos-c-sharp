namespace FC.Codeflix.Catalog.Application.Interfaces;

public interface IStorageService
{
    Task<string> Upload(
        string fileName, 
        Stream fileStream, 
        CancellationToken cancellationToken);
}
