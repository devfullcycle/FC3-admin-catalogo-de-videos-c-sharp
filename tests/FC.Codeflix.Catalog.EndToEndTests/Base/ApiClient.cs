using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.Infra.Messaging.JsonPolicies;
using Keycloak.AuthServices.Authentication;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FC.Codeflix.Catalog.EndToEndTests.Base;

public class ApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _defaultSerializeOptions;
    private readonly KeycloakAuthenticationOptions _keycloakOptions;
    private const string _adminUser = "admin";
    private const string _adminPassword = "123456";

    public ApiClient(HttpClient httpClient,
        KeycloakAuthenticationOptions keycloakOptions)
    {
        _httpClient = httpClient;
        _defaultSerializeOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = new JsonSnakeCasePolicy(),
            PropertyNameCaseInsensitive = true
        };
        _keycloakOptions = keycloakOptions;
        AddAuthorizationHeader();
    }

    private void AddAuthorizationHeader()
    {
        var accessToken = GetAccessTokenAsync(_adminUser, _adminPassword)
            .GetAwaiter().GetResult();
        _httpClient.DefaultRequestHeaders
            .Authorization = new AuthenticationHeaderValue(
                "Bearer", accessToken);
    }

    public async Task<string> GetAccessTokenAsync(string user, string password)
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"{_keycloakOptions.KeycloakUrlRealm}/protocol/openid-connect/token");
        var collection = new List<KeyValuePair<string, string>>
        {
            new("grant_type", "password"),
            new("client_id", _keycloakOptions.Resource),
            new("client_secret", _keycloakOptions.Credentials.Secret),
            new("username", user),
            new("password", password)
        };
        var content = new FormUrlEncodedContent(collection);
        request.Content = content;
        var response = await client.SendAsync(request);
        var credentials = await GetOutput<Credentials>(response);
        return credentials!.AccessToken;
    }

    public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        => await _httpClient.SendAsync(request);

    public async Task<(HttpResponseMessage?, TOutput?)> Post<TOutput>(
        string route,
        object payload
    ) where TOutput : class
    {
        var payloadJson = JsonSerializer.Serialize(
            payload,
            _defaultSerializeOptions
        );
        var response = await _httpClient.PostAsync(
            route,
            new StringContent(
                payloadJson,
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
                JsonSerializer.Serialize(
                    payload,
                    _defaultSerializeOptions
                ),
                Encoding.UTF8,
                "application/json"
            )
        );
        var output = await GetOutput<TOutput>(response);
        return (response, output);
    }

    public async Task<(HttpResponseMessage?, TOutput?)> Get<TOutput>(
        string route,
        object? queryStringParametersObject = null 
    ) where TOutput : class
    {
        var url = PrepareGetRoute(route, queryStringParametersObject);
        var response = await _httpClient.GetAsync(url);
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
            output = JsonSerializer.Deserialize<TOutput>(
                outputString,
                _defaultSerializeOptions
            );
        return output;
    }

    private string PrepareGetRoute(
        string route, 
        object? queryStringParametersObject
    )
    {
        if(queryStringParametersObject is null)
            return route;
        var parametersJson = JsonSerializer.Serialize(
            queryStringParametersObject,
            _defaultSerializeOptions
        );
        var parametersDictionary = Newtonsoft.Json.JsonConvert
            .DeserializeObject<Dictionary<string, string>>(parametersJson);
        return QueryHelpers.AddQueryString(route, parametersDictionary!);
    }

    internal async Task<(HttpResponseMessage?, TOutput?)>
        PostFormData<TOutput>(string route, FileInput file)
            where TOutput: class
    {
        var fileContent = new StreamContent(file.FileStream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

        using var content = new MultipartFormDataContent
        {
            { fileContent, "media_file", $"media.{file.Extension}" }
        };
        var response = await _httpClient.PostAsync(route, content);
        var output = await GetOutput<TOutput>(response);
        return (response, output);
    }
}
