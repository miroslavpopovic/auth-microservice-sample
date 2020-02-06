﻿using System.Collections.Generic;
using System.Linq;
using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
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

        public static IEnumerable<Client> Clients =>
            new List<Client>
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
                    RequireConsent = false,
                    RequirePkce = true,

                    // where to redirect to after login
                    RedirectUris = { "https://localhost:44316/signin-oidc" },

                    // where to redirect to after logout
                    PostLogoutRedirectUris = { "https://localhost:44316/signout-callback-oidc" },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "weather-api",
                        "weather-summary-api"
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
                    RequireConsent = false,

                    RedirectUris =           { "https://localhost:44336/login" },
                    PostLogoutRedirectUris = { "https://localhost:44336/" },
                    AllowedCorsOrigins =     { "https://localhost:44336" },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "weather-api"
                    }
                }
            };

        public static void InitializeDatabase(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();

            var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
            context.Database.Migrate();

            if (!context.Clients.Any())
            {
                foreach (var client in Clients)
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