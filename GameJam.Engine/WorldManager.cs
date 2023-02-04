using Microsoft.Extensions.DependencyInjection;

namespace GameJam.Engine;

public class WorldManager : IWorldManager
{
    private IEnumerable<ISystem> _updateSystems;
    private IEnumerable<ISystem> _simSystems;
    private IEnumerable<ISystem> _renderSystems;
    private readonly List<IWorld> _worlds;
    private readonly IServiceProvider _services;

    public WorldManager(IServiceProvider services)
    {
        _services = services;
        _updateSystems = ArraySegment<ISystem>.Empty;
        _simSystems = ArraySegment<ISystem>.Empty;
        _renderSystems = ArraySegment<ISystem>.Empty;
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
            var allSystems = scope!.ServiceProvider.GetRequiredService<IEnumerable<ISystem>>().ToArray();
            _updateSystems = _updateSystems.Concat(allSystems.Where(x => x.Phase == GamePhase.Update).ToArray());
            _simSystems = _simSystems.Concat(allSystems.Where(x => x.Phase == GamePhase.Simulation).ToArray());
            _renderSystems = _renderSystems.Concat(allSystems.Where(x => x.Phase == GamePhase.Presentation).ToArray());
        }
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