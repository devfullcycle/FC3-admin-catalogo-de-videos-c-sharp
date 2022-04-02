using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FC.Codeflix.Catalog.EndToEndTests.Base;
public class ApiClient
{
    private readonly HttpClient _httpClient;

    public ApiClient(HttpClient httpClient)
        => _httpClient = httpClient;

    public async Task<(HttpResponseMessage?, TOutput?)> Post<TOutput>(
        string route,
        object payload
    )
        where TOutput : class
    {
        var response = await _httpClient.PostAsync(
            route,
            new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            )
        );
        var outputString = await response.Content.ReadAsStringAsync();
        TOutput? output = null;
        if (!string.IsNullOrWhiteSpace(outputString))
            output = JsonSerializer.Deserialize<TOutput>(outputString,
                new JsonSerializerOptions { 
                    PropertyNameCaseInsensitive = true
                }
            );
        return (response, output);
    }

    public async Task<(HttpResponseMessage?, TOutput?)> Get<TOutput>(
        string route
    )
        where TOutput : class
    {
        var response = await _httpClient.GetAsync(route);
        var outputString = await response.Content.ReadAsStringAsync();
        TOutput? output = null;
        if (!string.IsNullOrWhiteSpace(outputString))
            output = JsonSerializer.Deserialize<TOutput>(outputString,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }
            );
        return (response, output);
    }
}
