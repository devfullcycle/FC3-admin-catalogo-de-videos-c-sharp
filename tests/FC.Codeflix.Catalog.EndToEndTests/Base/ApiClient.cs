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
    ) where TOutput : class
    {
        var response = await _httpClient.PostAsync(
            route,
            new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            )
        );
        var output = await GetOutput<TOutput>(response);
        return (response, output);
    }
    public async Task<(HttpResponseMessage?, TOutput?)> Put<TOutput>(
        string route,
        object payload
    ) where TOutput : class
    {
        var response = await _httpClient.PutAsync(
            route,
            new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            )
        );
        var output = await GetOutput<TOutput>(response);
        return (response, output);
    }

    public async Task<(HttpResponseMessage?, TOutput?)> Get<TOutput>(
        string route
    ) where TOutput : class
    {
        var response = await _httpClient.GetAsync(route);
        var output = await GetOutput<TOutput>(response);
        return (response, output);
    }
    public async Task<(HttpResponseMessage?, TOutput?)> Delete<TOutput>(
        string route
    ) where TOutput : class
    {
        var response = await _httpClient.DeleteAsync(route);
        var output = await GetOutput<TOutput>(response);
        return (response, output);
    }

    private async Task<TOutput?> GetOutput<TOutput>(HttpResponseMessage response)
        where TOutput : class
    {
        var outputString = await response.Content.ReadAsStringAsync();
        TOutput? output = null;
        if (!string.IsNullOrWhiteSpace(outputString))
            output = JsonSerializer.Deserialize<TOutput>(outputString,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }
            );
        return output;
    }
}
