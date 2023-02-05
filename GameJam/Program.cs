using GameJam;
using GameJam.Systems;
using GameJam.Systems.Init;
using Lamar.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

await CreateHostBuilder()
    .RunConsoleAsync();

static IHostBuilder CreateHostBuilder() =>
    Host.CreateDefaultBuilder()
        .UseWindow(new (800, 600), "I am Groot!")
        .UseEcs()
        // .ConfigureServices(services => services.AddHostedService<InitSystem>())
        .ConfigureServices(services =>
        {
            services.AddScoped<ISystem, CameraSetupSystem>();
            services.AddScoped<ISystem, GridSetupSystem>();
            services.AddScoped<ISystem, ActiveSetupSystem>();
            services.AddScoped<ISystem, MoveActiveNodeSystem>();
            services.AddScoped<ISystem, ActiveSystem>();
        })
        .UseSerilog((_, _, loggerConfiguration) =>
        {
            loggerConfiguration
                .Enrich.WithThreadId()
                .Enrich.FromLogContext()
                .WriteTo.Async(c => c.Console());
        });
