using System.Collections.Generic;
using System.Linq;
using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Auth
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> Ids =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };

        public static IEnumerable<ApiResource> Apis =>
            new List<ApiResource>
            {
                new ApiResource("weather-api", "Weather API"),
                new ApiResource("weather-summary-api", "Weather Summary API")
            };

        public static IEnumerable<Client> GetClients(IConfiguration configuration)
        {
            var clientsConfig = configuration.GetSection("Clients");
            var mvcClientConfig = clientsConfig.GetSection("MvcClient");
            var authAdminClientConfig = clientsConfig.GetSection("AuthAdminClient");
            var javaScriptClientConfig = clientsConfig.GetSection("JavaScriptClient");

            return new List<Client>
            {
                // Machine to machine client
                new Client
                {
                    ClientId = "weather-api-console-client",
                    ClientSecrets = {new Secret("secret".Sha256())},

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = {"weather-api"}
                },

                // Machine to machine client
                new Client
                {
                    ClientId = "weather-api-worker-client",
                    ClientSecrets = {new Secret("secret".Sha256())},

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = {"weather-api"}
                },

                // Machine to machine client
                new Client
                {
                    ClientId = "weather-api-mvc-client",
                    ClientSecrets = {new Secret("secret".Sha256())},

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = {"weather-api", "weather-summary-api"}
                },

                // interactive ASP.NET MVC Core client
                new Client
                {
                    ClientId = "mvc-client",
                    ClientSecrets = {new Secret("secret".Sha256())},

                    AllowedGrantTypes = GrantTypes.Code,
                    RequireConsent = true,
                    RequirePkce = true,

                    // where to redirect to after login
                    RedirectUris =
                        mvcClientConfig.GetSection("RedirectUris").Get<string[]>(),

                    // where to redirect to after logout
                    PostLogoutRedirectUris =
                        mvcClientConfig.GetSection("PostLogoutRedirectUris").Get<string[]>(),

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "weather-api",
                        "weather-summary-api"
                    },

                    AllowOfflineAccess = true
                },

                // Auth.Admin client
                new Client
                {
                    ClientId = "auth-admin-client",
                    ClientSecrets = {new Secret("secret".Sha256())},

                    AllowedGrantTypes = GrantTypes.Code,
                    RequireConsent = true,
                    RequirePkce = true,

                    // where to redirect to after login
                    RedirectUris =
                        authAdminClientConfig.GetSection("RedirectUris").Get<string[]>(),

                    // where to redirect to after logout
                    PostLogoutRedirectUris =
                        authAdminClientConfig.GetSection("PostLogoutRedirectUris").Get<string[]>(),

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    },

                    AllowOfflineAccess = true
                },

                // JavaScript Client
                new Client
                {
                    ClientId = "aurelia",
                    ClientName = "Aurelia Client",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = false,
                    RequireConsent = true,

                    RedirectUris =
                        javaScriptClientConfig.GetSection("RedirectUris").Get<string[]>(),
                    PostLogoutRedirectUris =
                        javaScriptClientConfig.GetSection("PostLogoutRedirectUris").Get<string[]>(),
                    AllowedCorsOrigins =
                        javaScriptClientConfig.GetSection("AllowedCorsOrigins").Get<string[]>(),

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "weather-api"
                    }
                },

                // WPF Client
                new Client
                {
                    ClientId = "wpf-client",
                    ClientName = "WPF Client",

                    AllowedGrantTypes = GrantTypes.DeviceFlow,
                    RequireClientSecret = false,

                    AlwaysIncludeUserClaimsInIdToken = true,
                    AllowOfflineAccess = true,

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "weather-api"
                    }
                }
            };
        }

        public static void InitializeDatabase(IApplicationBuilder app, IConfiguration configuration)
        {
            using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();

            var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
            context.Database.Migrate();

            if (!context.Clients.Any())
            {
                foreach (var client in GetClients(configuration))
                {
                    context.Clients.Add(client.ToEntity());
                }
                context.SaveChanges();
            }

            if (!context.IdentityResources.Any())
            {
                foreach (var resource in Ids)
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }

            if (!context.ApiResources.Any())
            {
                foreach (var resource in Apis)
                {
                    context.ApiResources.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }
        }
    }
}
