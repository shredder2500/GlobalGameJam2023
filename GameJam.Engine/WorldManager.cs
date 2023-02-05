using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GameJam.Engine;

public class WorldManager : IWorldManager
{
    private IEnumerable<ISystem> _updateSystems;
    private IEnumerable<ISystem> _simSystems;
    private IEnumerable<ISystem> _renderSystems;
    private IEnumerable<ISystem> _initSystems;
    private readonly List<IWorld> _worlds;
    private readonly IServiceProvider _services;
    private readonly ILogger _logger;

    public WorldManager(IServiceProvider services, ILogger<WorldManager> logger)
    {
        _services = services;
        _logger = logger;
        _updateSystems = ArraySegment<ISystem>.Empty;
        _simSystems = ArraySegment<ISystem>.Empty;
        _renderSystems = ArraySegment<ISystem>.Empty;
        _initSystems = ArraySegment<ISystem>.Empty;
        _worlds = new();
    }

    public IWorld CreateWorld()
    {
        var scope = _services.CreateScope();
        var world = scope.ServiceProvider.GetRequiredService<IWorld>();
        GetSystems();
        _worlds.Add(world);
        return world;

        void GetSystems()
        {
            var allSystems = scope!.ServiceProvider.GetRequiredService<IEnumerable<ISystem>>().ToList();
            _logger.LogInformation("Adding {NumberOfSystems} Systems", allSystems.Count);
            allSystems.ForEach(s => _logger.LogInformation("Adding System: {SystemName}", s.GetType().Name));
            _updateSystems = _updateSystems.Concat(allSystems.Where(x => x.Phase == GamePhase.Update).ToArray());
            _simSystems = _simSystems.Concat(allSystems.Where(x => x.Phase == GamePhase.Simulation).ToArray());
            _renderSystems = _renderSystems.Concat(allSystems.Where(x => x.Phase == GamePhase.Presentation).ToArray());
            _initSystems = _renderSystems.Concat(allSystems.Where(x => x.Phase == GamePhase.Init).ToArray());
        }
    }

    public async ValueTask Init(CancellationToken cancellationToken)
    {
        await Parallel.ForEachAsync(_initSystems, cancellationToken, (system, token) => system.Execute(token));
        await Parallel.ForEachAsync(_worlds, cancellationToken, (world, _) =>
        {
            world.Sync();
            return ValueTask.CompletedTask;
        });
    }

    public async ValueTask Update(CancellationToken cancellationToken)
    {
        await Parallel.ForEachAsync(_updateSystems, cancellationToken, (system, token) => system.Execute(token));
        await Parallel.ForEachAsync(_worlds, cancellationToken, (world, _) =>
        {
            world.Sync();
            return ValueTask.CompletedTask;
        });
    }
    
    public async ValueTask Sim(CancellationToken cancellationToken)
    {
        await Parallel.ForEachAsync(_simSystems, cancellationToken, (system, token) => system.Execute(token));
        await Parallel.ForEachAsync(_worlds, cancellationToken, (world, _) =>
        {
            world.Sync();
            return ValueTask.CompletedTask;
        });
    }
    
    public async ValueTask Render(CancellationToken cancellationToken)
    {
        await Parallel.ForEachAsync(_renderSystems, cancellationToken, (system, token) => system.Execute(token));
        await Parallel.ForEachAsync(_worlds, cancellationToken, (world, _) =>
        {
            world.Sync();
            return ValueTask.CompletedTask;
        });
    }
}