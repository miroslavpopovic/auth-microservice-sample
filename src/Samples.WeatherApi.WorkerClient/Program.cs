using System;
using IdentityModel.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;

namespace Samples.WeatherApi.WorkerClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services
                        .AddAccessTokenManagement(
                            options =>
                            {
                                options.Client.Clients.Add(
                                    "auth", new ClientCredentialsTokenRequest
                                    {
                                        Address = "https://localhost:44396/connect/token",
                                        ClientId = "weather-api-worker-client",
                                        ClientSecret = "secret",
                                        Scope = "weather-api"
                                    });
                            })
                        .ConfigureBackchannelHttpClient()
                        .AddTransientHttpErrorPolicy(
                            policy =>
                                policy.WaitAndRetryAsync(
                                    new[]
                                    {
                                        TimeSpan.FromSeconds(1),
                                        TimeSpan.FromSeconds(2),
                                        TimeSpan.FromSeconds(3)
                                    }));

                    var apiBaseUri = new Uri("https://localhost:44373/");

                    // Register regular HttpClient that knows how to handle tokens
                    services.AddClientAccessTokenClient(
                        "weather-api-client",
                        configureClient: client => { client.BaseAddress = apiBaseUri; });

                    services.AddHttpClient<IWeatherForecastClient, WeatherForecastClient>(
                        client => { client.BaseAddress = apiBaseUri; })
                        .AddClientAccessTokenHandler();

                    services.AddHostedService<Worker>();
                });
    }
}
