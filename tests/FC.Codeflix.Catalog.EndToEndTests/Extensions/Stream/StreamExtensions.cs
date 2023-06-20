using System.IO;

namespace FC.Codeflix.Catalog.EndToEndTests.Extensions.Stream;
internal static class StreamExtensions
{
    public static string ToContentString(this System.IO.Stream stream)
    {
        stream.Seek(0, SeekOrigin.Begin);
        var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
