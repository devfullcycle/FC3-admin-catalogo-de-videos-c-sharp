using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;

namespace FC.Codeflix.Catalog.Api.Configurations;

public static class SecurityConfiguration
{
    public static IServiceCollection AddSecurity(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddKeycloakAuthentication(configuration);
        services.AddKeycloakAuthorization(configuration);
        return services;
    }
}
