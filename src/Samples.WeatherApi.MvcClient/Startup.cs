using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;

namespace Samples.WeatherApi.MvcClient
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

            services.AddControllersWithViews();

            services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                    {
                        options.Authority = Configuration.GetServiceUri("auth")!.ToString().TrimEnd('/');

                        options.BackchannelHttpHandler = new HttpClientHandler
                        {
                            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                        };

                        options.ClientId = "mvc-client";
                        options.ClientSecret = "secret";
                        options.ResponseType = "code";

                        options.SaveTokens = true;

                        options.Scope.Add("weather-api");
                        options.Scope.Add("weather-summary-api");
                        options.Scope.Add("offline_access");
                    });

            // Register and configure Token Management and Weather API HTTP clients for DI
            // This is using a separate Client to access API using Client Credentials
            services
                .AddAccessTokenManagement(
                    options =>
                    {
                        options.Client.Clients.Add(
                            "auth", new ClientCredentialsTokenRequest
                            {
                                Address = $"{Configuration.GetServiceUri("auth")}connect/token",
                                ClientId = "weather-api-mvc-client",
                                ClientSecret = "secret",
                                Scope = "weather-api"
                            });
                    })
                .ConfigureBackchannelHttpClient()
                .ConfigurePrimaryHttpMessageHandler(
                    () => new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback =
                            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    })
                .AddTransientHttpErrorPolicy(
                    policy =>
                        policy.WaitAndRetryAsync(
                            new[]
                            {
                                TimeSpan.FromSeconds(1),
                                TimeSpan.FromSeconds(2),
                                TimeSpan.FromSeconds(3)
                            }));

            services
                .AddClientAccessTokenClient(
                    "weather-api-client",
                    configureClient: client =>
                    {
                        client.BaseAddress = new Uri(Configuration.GetServiceUri("weather-api")!.ToString());
                    })
                .ConfigurePrimaryHttpMessageHandler(
                    () => new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback =
                            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    });

            services
                .AddClientAccessTokenClient(
                    "weather-summary-api-client",
                    configureClient: client =>
                    {
                        client.BaseAddress = new Uri(Configuration.GetServiceUri("weather-summary-api")!.ToString());
                    })
                .ConfigurePrimaryHttpMessageHandler(
                    () => new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback =
                            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints
                    .MapDefaultControllerRoute()
                    .RequireAuthorization();
            });
        }
    }
}
