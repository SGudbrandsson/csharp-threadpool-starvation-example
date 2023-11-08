using System;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DiagnosticScenarios
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
            CreateHealthCheck(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
public static IWebHostBuilder CreateHealthCheck(string[] args)
{
    // Custom kestrel on port 7000 with a single thread priority 4
    var liveHostBuilder = WebHost.CreateDefaultBuilder(args)
        .ConfigureKestrel((context, options) =>
        {
            // Configure a dedicated thread with priority 4 for the live endpoint
            options.ListenAnyIP(7000, listenOptions =>
            {
                listenOptions.UseThreadingSettings(new ThreadingSettings()
                {
                    MinThreads = 1,
                    MaxThreads = 1,
                    ThreadPriority = ThreadPriority.AboveNormal
                });
            });
        })
        .ConfigureServices(services =>
        {
            // Register the live endpoint handler
            services.AddSingleton<IApplicationService, LiveApplicationService>();
        })
        .Configure(app =>
        {
            // Add routing for the live endpoint
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/live", async context =>
                {
                    // Get the application service from the dependency injection container
                    var appService = context.RequestServices.GetRequiredService<IApplicationService>();

                    // Call the live endpoint handler
                    var liveMessage = "OK";

                    // Write the live message to the response
                    context.Response.ContentType = "text/plain";
                    await context.Response.WriteAsync(liveMessage);
                });
            });
        });

    return liveHostBuilder;
}
    }
}
