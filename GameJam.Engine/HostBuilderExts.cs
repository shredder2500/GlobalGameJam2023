using System.Collections.Immutable;
using System.Drawing;
using JasperFx.Core;
using Lamar.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Silk.NET.Windowing;

namespace GameJam.Engine;

public static class HostBuilderExts
{
    public static IHostBuilder UseWindow(this IHostBuilder builder, Size windowSize, string title)
    {
        var window = Window.Create(WindowOptions.Default with
        {
            Size = new(windowSize.Width, windowSize.Height),
            Title = title
        });
        return builder.ConfigureServices((_, services) =>
        {
            services.AddSingleton<IWindow>(window);
        });
    }

    public static IHostBuilder UseEcs(this IHostBuilder builder)
    {
        return builder.UseLamar(services =>
        {
            services.AddSingleton<IWorldManager, WorldManager>();
            services.AddScoped<IWorld, World>();
            services.AddSingleton<IGameTime, GameTime>();
            services.AddHostedService<WindowLifetime>();
            services.Scan(x =>
            {
                x.AssembliesFromApplicationBaseDirectory();
                x.AddAllTypesOf<ISystem>(ServiceLifetime.Scoped);
            });
        });
    }
}