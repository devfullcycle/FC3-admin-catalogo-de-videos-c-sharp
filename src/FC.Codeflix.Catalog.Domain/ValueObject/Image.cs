namespace FC.Codeflix.Catalog.Domain.ValueObject;
public class Image
{
    public string Path { get; }

    public Image(string path) => Path = path;
}
