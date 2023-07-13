using FC.Codeflix.Catalog.Api.Authorization;
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
        services
            .AddAuthorization(options =>
            {
                options.AddPolicy(Policies.VideosManager,
                    builder => builder.RequireRealmRoles(
                        Roles.Videos,
                        Roles.Admin));
            })
            .AddKeycloakAuthorization(configuration);
        return services;
    }
}
