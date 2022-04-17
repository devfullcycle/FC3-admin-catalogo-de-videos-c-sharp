using Newtonsoft.Json.Serialization;

namespace FC.Codeflix.Catalog.Api.Extensions.String;

public static class StringSnakeCaseExtension
{
    private readonly static NamingStrategy _snakeCaseNamingStrategy =
        new SnakeCaseNamingStrategy();

    public static string ToSnakeCase(this string stringToConvert)
    {
        ArgumentNullException
            .ThrowIfNull(stringToConvert, nameof(stringToConvert));
        return _snakeCaseNamingStrategy
            .GetPropertyName(stringToConvert, false);
    }
}
