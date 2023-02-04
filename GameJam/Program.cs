using Lamar.Microsoft.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

await CreateHostBuilder()
    .RunConsoleAsync();

static IHostBuilder CreateHostBuilder() =>
    Host.CreateDefaultBuilder()
        .UseWindow(new (800, 600), "I am Groot!")
        .UseEcs()
        .UseSerilog((_, _, loggerConfiguration) =>
        {
            loggerConfiguration
                .Enrich.WithThreadId()
                .Enrich.FromLogContext()
                .WriteTo.Async(c => c.Console());
        });
