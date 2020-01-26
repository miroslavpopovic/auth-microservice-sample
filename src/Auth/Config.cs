using System.Collections.Generic;
using IdentityServer4;
using IdentityServer4.Models;

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
                    ClientId = "js",
                    ClientName = "JavaScript Client",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = false,

                    RedirectUris =           { "http://localhost:5003/callback.html" },
                    PostLogoutRedirectUris = { "http://localhost:5003/index.html" },
                    AllowedCorsOrigins =     { "http://localhost:5003" },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "weather-api"
                    }
                }
            };
    }
}
