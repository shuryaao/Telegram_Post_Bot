using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net;

namespace Telegram_Memes_Post_Bot
{
    public class Program
    {
        public static readonly string Namespace = typeof(Program).Namespace;
        public static readonly string AppName = Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);
        public static int Main(string[] args)
        {
            var configuration = GetConfiguration();

            try
            {
                var host = BuildHost(configuration, args);

                host.Run();

                return 0;
            }
            catch (Exception)
            {
                return 1;
            }
        }

        private static IHost BuildHost(IConfiguration configuration, string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webHost =>
                {
                    webHost.CaptureStartupErrors(false)
                    .ConfigureKestrel(options =>
                    {
                        var (httpPort, healthCheckPort) = GetDefinedPorts(configuration);

                        options.Listen(IPAddress.Any, httpPort, listenOptions =>
                        {
                            listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                        });

                        if (httpPort != healthCheckPort)
                        {
                            options.Listen(IPAddress.Any, healthCheckPort, listenOptions =>
                            {
                                listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                            });
                        }
                    })
                    .UseStartup<Startup>()
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .UseConfiguration(configuration);
                })
                .Build();

        private static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            return builder.Build();
        }


        private static (int httpPort, int healthCheckPort) GetDefinedPorts(IConfiguration config)
        {
            var httpPort = config.GetValue("PORT", 5013);
            var healthCheckPort = config.GetValue("HEALTH_CHECK_PORT", 5014);
            return (httpPort, healthCheckPort);
        }

    }
}