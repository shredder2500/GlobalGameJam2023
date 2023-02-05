using System.Drawing;
using JasperFx.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace GameJam.Engine;

internal class WindowLifetime : IHostedService
{
    private readonly IWindow _window;
    private readonly IGameTime _gameTime;
    private readonly IHostApplicationLifetime _appLifetime;
    private readonly IHostApplicationLifetime _host;
    private readonly IWorldManager _worldManager;
    private readonly ILogger _logger;
    private readonly IMainThreadDispatcher _dispatcher;

    public WindowLifetime(IWindow window, IGameTime gameTime,
        IHostApplicationLifetime appLifetime, IWorldManager worldManager, IHostApplicationLifetime host, ILogger<WindowLifetime> logger, IMainThreadDispatcher dispatcher)
    {
        _window = window;
        _gameTime = gameTime;
        _appLifetime = appLifetime;
        _worldManager = worldManager;
        _host = host;
        _logger = logger;
        _dispatcher = dispatcher;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        
        _window.Initialize();
        var gl = GL.GetApi(_window);
        gl.Enable(GLEnum.Blend);
        gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        gl.ClearColor((Color)new ColorConverter().ConvertFromString("#67a4c5"));
        _worldManager.CreateWorld(); 
        _worldManager.Init(cancellationToken).AsTask().Wait(cancellationToken);
        _window.Run(() =>
        {
            _gameTime.Update();
            _window.DoEvents();
            _worldManager.Update(_appLifetime.ApplicationStopping)
                .AsTask()
                .Wait(_appLifetime.ApplicationStopping);
            _worldManager.Sim(_appLifetime.ApplicationStopping)
                .AsTask()
                .Wait(_appLifetime.ApplicationStopping);
            gl.Clear(ClearBufferMask.ColorBufferBit);
            _worldManager.Render(_appLifetime.ApplicationStopping)
                .AsTask()
                .Wait(_appLifetime.ApplicationStopping);
            _dispatcher.Execute();
            _window.SwapBuffers();
        });
        _host.StopApplication();

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _window.Close();
        return Task.CompletedTask;
    }
}