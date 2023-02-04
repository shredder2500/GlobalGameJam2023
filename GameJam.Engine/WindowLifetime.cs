using System.Drawing;
using Microsoft.Extensions.Hosting;
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
    private Task _gameLoop;

    public WindowLifetime(IWindow window, IGameTime gameTime,
        IHostApplicationLifetime appLifetime, IWorldManager worldManager, IHostApplicationLifetime host)
    {
        _window = window;
        _gameTime = gameTime;
        _appLifetime = appLifetime;
        _worldManager = worldManager;
        _host = host;
    }

    public Task StartAsync(CancellationToken _)
    {
        
        _gameLoop = Task.Run(() =>
        {
            _window.Initialize();
            var gl = GL.GetApi(_window);
            gl.Enable(GLEnum.Blend);
            gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            gl.ClearColor(Color.Aqua);
            _window.Run(() => {
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
                _window.SwapBuffers();
            });
            _host.StopApplication();
            
        }, _appLifetime.ApplicationStopping);
            return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _window.Close();
        return _gameLoop;
    }
}