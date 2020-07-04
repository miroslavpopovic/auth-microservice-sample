using System.Net.Http;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace Samples.WeatherApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            var urlPrefix = Configuration.GetValue<string>("ApplicationUrlPrefix");

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                    {
                        options.Authority = $"{urlPrefix}:44396";
                        options.Audience = "weather-api";

#if DEBUG
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            // HACK: DON'T SKIP ISSUER VALIDATION IN PRODUCTION!
                            // We are using multiple issuers in debug - localhost and auth.sample.local
                            ValidateIssuer = false
                        };
#endif

                        options.BackchannelHttpHandler = new HttpClientHandler
                        {
                            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                        };
                    });

            // CORS policy for JavaScript clients
            services.AddCors(
                options =>
                {
                    options.AddPolicy(
                        "default", policy =>
                        {
                            policy
                                .WithOrigins($"{urlPrefix}:44336")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                        });
                });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("default");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
