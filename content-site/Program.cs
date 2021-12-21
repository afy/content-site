using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Nancy.Hosting.Kestrel;
using content_site.src.backend;

namespace content_site
{
    internal class Startup {
        public void Configure(IApplicationBuilder app) => app.UseNancy();
    }

    public class Program
    {
        public static ContentTracker contentTracker;
        
        static void Main(string[] args)
        {
            string addr = "127.0.0.1";
            int port = 8999;
            contentTracker = new ContentTracker();

            WebHostBuilder webBuilder = new WebHostBuilder();
            webBuilder.ConfigureKestrel(serverOptions => {
                serverOptions.ConfigureEndpointDefaults(listenOptions => {
                    listenOptions.IPEndPoint.Address = System.Net.IPAddress.Parse(addr);
                    listenOptions.IPEndPoint.Port = port;
                });
            });

            var host = webBuilder
                .UseKestrel()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }

        // I am way too tired for whatever fucking nonsense is going on
        // WILL be fixed at some point in the future maybe
        public static string getUri() {
            return "http://localhost:8999";
        }
    }
}
