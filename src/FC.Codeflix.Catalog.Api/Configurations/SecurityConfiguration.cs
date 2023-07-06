using Keycloak.AuthServices.Authentication;

namespace FC.Codeflix.Catalog.Api.Configurations;

public static class SecurityConfiguration
{
    public static IServiceCollection AddSecurity(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddKeycloakAuthentication(configuration);
        return services;
    }
}
