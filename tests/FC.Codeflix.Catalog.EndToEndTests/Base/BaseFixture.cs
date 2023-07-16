using Bogus;
using FC.Codeflix.Catalog.Infra.Data.EF;
using Keycloak.AuthServices.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace FC.Codeflix.Catalog.EndToEndTests.Base;
public class BaseFixture : IDisposable
{
    protected Faker Faker { get; set; }

    public CustomWebApplicationFactory<Program> WebAppFactory { get; set; }
    public HttpClient HttpClient { get; set; }
    public ApiClient ApiClient { get; set; }
    private readonly string _dbConnectionString;

    public BaseFixture()
    {
        Faker = new Faker("pt_BR");
        WebAppFactory = new CustomWebApplicationFactory<Program>();
        HttpClient = WebAppFactory.CreateClient();
        var configuration = WebAppFactory.Services
            .GetRequiredService<IConfiguration>();
        var keycloakOptions = configuration
            .GetSection(KeycloakAuthenticationOptions.Section)
            .Get<KeycloakAuthenticationOptions>();
        ApiClient = new ApiClient(HttpClient, keycloakOptions);
        ArgumentNullException.ThrowIfNull(configuration);
        _dbConnectionString = configuration
            .GetConnectionString("CatalogDb");
    }

    public CodeflixCatalogDbContext CreateDbContext()
    {
        var context = new CodeflixCatalogDbContext(
            new DbContextOptionsBuilder<CodeflixCatalogDbContext>()
            .UseMySql(
                _dbConnectionString, 
                ServerVersion.AutoDetect(_dbConnectionString)
            )
            .Options
        );
        return context;
    }
    public void CleanPersistence()
    {
        var context = CreateDbContext();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        WebAppFactory.Dispose();
    }
}
