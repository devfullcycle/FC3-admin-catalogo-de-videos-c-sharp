namespace FC.Codeflix.Catalog.Domain.ValueObject;
public class Image : SeedWork.ValueObject
{
    public string Path { get; }

    public Image(string path) => Path = path;
    
    public override bool Equals(SeedWork.ValueObject? other) =>
        other is Image image &&
        Path == image.Path;

    protected override int GetCustomHashCode()
        => HashCode.Combine(Path);
}
