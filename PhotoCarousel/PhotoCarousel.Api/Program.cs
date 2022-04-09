using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace PhotoCarousel.Api;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureHostConfiguration(configBuilder =>
            {
                configBuilder.AddEnvironmentVariables();
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseKestrel();
                webBuilder.ConfigureKestrel((context, options) =>
                {
                    var certificateFileName = context.Configuration.GetValue<string>("CERTIFICATE_FILENAME");
                    var certificatePassword = context.Configuration.GetValue<string>("CERTIFICATE_PASSWORD");

                    if (string.IsNullOrEmpty(certificateFileName) || string.IsNullOrEmpty(certificatePassword))
                    {
                        options.Listen(IPAddress.Any, 5000);
                    }
                    else
                    {
                        options.Listen(IPAddress.Any, 5000,
                            listenOptions => { listenOptions.UseHttps(certificateFileName, certificatePassword); });
                    }
                });
                webBuilder.UseStartup<Startup>();
            });
}