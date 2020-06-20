using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.ApplicationInsights.Sinks.ApplicationInsights.TelemetryConverters;
using Serilog.Sinks.SystemConsole.Themes;

namespace FoundDefect
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
#if DesktopRelease
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Literate)
#else
                .WriteTo.ApplicationInsights(new EventTelemetryConverter(), LogEventLevel.Debug)
#endif
                .CreateLogger();
            Log.Information("Starting host.");
            CreateHostBuilder(args).Build().Run();
        }

#if DesktopRelease
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseSerilog();
                })
                .ConfigureWebHost(config =>
                {
                    config.UseUrls("http://*:5050");
                })
                .UseWindowsService();
#else
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseSerilog();
                });
#endif
    }
}
