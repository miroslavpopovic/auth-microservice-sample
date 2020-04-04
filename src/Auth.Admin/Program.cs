using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Auth.Admin
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureAppConfiguration(
                        (context, builder) =>
                        {
                            if (context.HostingEnvironment.IsEnvironment("Docker"))
                            {
                                builder.AddUserSecrets(typeof(Program).Assembly);
                            }
                        });
                    webBuilder.UseStartup<Startup>();
                });
    }
}
