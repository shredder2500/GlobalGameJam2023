using System.Drawing;
using GameJam.Engine.Animation.Systems;
using GameJam.Engine.Rendering;
using GameJam.Engine.Rendering.Systems;
using GameJam.Engine.Resources;
using GameJam.Engine.Resources.Loaders;
using Lamar.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace GameJam.Engine;

public static class HostBuilderExts
{
    public static IHostBuilder UseWindow(this IHostBuilder builder, Size windowSize, string title)
    {
        var window = Window.Create(WindowOptions.Default with
        {
            Size = new(windowSize.Width, windowSize.Height),
            Title = title,
            IsEventDriven = false,
            WindowBorder = WindowBorder.Fixed
        });
        return builder.ConfigureServices((_, services) =>
        {
            services.AddSingleton<IWindow>(window);
            services.AddScoped((_) => window.CreateInput());
        });
    }

    public static IHostBuilder UseEcs(this IHostBuilder builder)
    {
        return builder.UseLamar(services =>
        {
            AddResource<Texture, TextureLoader>();
            AddResource<Shader, ShaderLoader>();
            
            services.AddScoped<IResourceManager, ResourceManager>();
            services.AddSingleton<IWorldManager, WorldManager>();
            services.AddScoped<IWorld, World>();
            services.AddSingleton<IGameTime, GameTime>();
            services.AddHostedService<WindowLifetime>();
            services.AddScoped<IRenderQueue, RenderQueue>();
            services.AddSingleton<IMainThreadDispatcher, MainThreadDispatcher>();

            services.AddScoped<ISystem, Render2DSystem>();
            services.AddScoped<ISystem, SpriteAnimationSystem>();

            void AddResource<T, TLoader>() where TLoader : class, IResourceLoader<T>
            {
                services.AddScoped<IResourceCache<T>, ResourceCache<T>>();
                services.AddScoped<IResourceLoader<T>, TLoader>();
            }
        });
    }
}