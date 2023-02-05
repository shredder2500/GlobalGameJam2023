using GameJam;
using GameJam.Systems;
using GameJam.Systems.Init;
using GameJam.Systems.UI;
using Lamar.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

await CreateHostBuilder()
    .RunConsoleAsync();

static IHostBuilder CreateHostBuilder() =>
    Host.CreateDefaultBuilder()
        .UseWindow(new (1000, 800), "I am Groot!")
        .UseEcs()
        // .ConfigureServices(services => services.AddHostedService<InitSystem>())
        .ConfigureServices(services =>
        {
            services.AddScoped<ISystem, CameraSetupSystem>();
            services.AddScoped<ISystem, GridSetupSystem>();
            services.AddScoped<ISystem, ActiveSetupSystem>();
            services.AddScoped<ISystem, RootSetupSystem>();
            services.AddScoped<ISystem, UndergroundSetupSystem>();
            services.AddScoped<ISystem, TreeSetupSystem>();
            services.AddScoped<ISystem, UISetupSystem>();
            services.AddScoped<ISystem, WaterDisplaySystem>();
            services.AddScoped<ISystem, MoveActiveNodeSystem>();
            services.AddScoped<ISystem, ScoreDisplaySystem>();
            services.AddScoped<ISystem, ActiveSystem>();
            services.AddScoped<ISystem, AddRootSystem>();
            services.AddScoped<ISystem, WaterConsumption>();
            services.AddScoped<ISystem, RichSoilConsumption>();
            services.AddScoped<ISystem, BugConsumption>();
            services.AddScoped<ISystem, CanPlaceIndicatorSystem>();
            services.AddScoped<ISystem, TreeSpriteStatusSystem>();
            services.AddScoped<ISystem, PlayGameMusic>();
        })
        .UseSerilog((_, _, loggerConfiguration) =>
        {
            loggerConfiguration
                .Enrich.WithThreadId()
                .Enrich.FromLogContext()
                .WriteTo.Async(c => c.Console());
        });
