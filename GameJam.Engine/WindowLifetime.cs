using Microsoft.Extensions.Hosting;
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
        
        _gameLoop = Task.Run(async () =>
        {
            _window.Initialize();
            while (!_window.IsClosing)
            {
                _gameTime.Update();

                _window.DoEvents();
                if (_window.IsClosing) continue;
                _window.DoUpdate();
                await _worldManager.Update(_appLifetime.ApplicationStopping);
                await _worldManager.Sim(_appLifetime.ApplicationStopping);

                await _worldManager.Render(_appLifetime.ApplicationStopping);
                _window.DoRender();
                _window.Run();
            }
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