using Newtonsoft.Json.Serialization;
using System;

namespace FC.Codeflix.Catalog.EndToEndTests.Extensions.String;
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
