using System;
using System.Net;
using Auth.Data;
using Auth.Email;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace Auth
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDatabaseDeveloperPageExceptionFilter();

            var connectionString = Configuration.GetConnectionString("auth-db");
            Console.WriteLine(connectionString);

            services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(connectionString, builder =>
                {
                    builder.EnableRetryOnFailure(10, TimeSpan.FromSeconds(30), null);
                }));

            services
                .AddDefaultIdentity<AuthUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services
                .AddIdentityServer(
                    options =>
                    {
                        options.Events.RaiseErrorEvents = true;
                        options.Events.RaiseFailureEvents = true;
                        options.Events.RaiseInformationEvents = true;
                        options.Events.RaiseSuccessEvents = true;
                    })
                //.AddInMemoryIdentityResources(Config.Ids)
                //.AddInMemoryApiScopes(Config.ApiScopes)
                //.AddInMemoryApiResources(Config.ApiResources)
                //.AddInMemoryClients(Config.Clients)
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext =
                        builder => builder.UseSqlServer(
                            connectionString,
                            sql => sql.MigrationsAssembly(typeof(Startup).Assembly.GetName().Name));
                })
                .AddAspNetIdentity<AuthUser>()
                .AddDeveloperSigningCredential();

            services
                .AddAuthentication()
                .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
                {
                    // We are leaving the default auth scheme
                    //options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                    options.ClientId = Configuration["Providers:Google:ClientId"];
                    options.ClientSecret = Configuration["Providers:Google:ClientSecret"];
                })
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, "Demo IdentityServer", options =>
                {
                    // We are leaving the default auth scheme
                    //options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    //options.SignOutScheme = IdentityServerConstants.SignoutScheme;
                    options.SaveTokens = true;

                    options.Authority = "https://demo.identityserver.io/";
                    options.ClientId = Configuration["Providers:IdentityServerDemo:ClientId"];
                    options.ClientSecret = Configuration["Providers:IdentityServerDemo:ClientSecret"];
                    options.ResponseType = "code";

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = "name",
                        RoleClaimType = "role"
                    };
                });

            services.AddEmail(Configuration);

            services.AddRazorPages();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            Config.InitializeDatabase(app, Configuration);

            if (env.IsDevelopment() || env.IsEnvironment("Docker"))
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseIdentityServer();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapRazorPages(); });
        }
    }
}
