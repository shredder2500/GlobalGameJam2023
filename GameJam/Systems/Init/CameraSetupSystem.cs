using GameJam.Engine.Components;
using GameJam.Engine.Rendering.Components;

using static GameJam.Config;

namespace GameJam.Systems.Init;

public class CameraSetupSystem : ISystem
{
    public GamePhase Phase => GamePhase.Init;

    private readonly IWorld _world;
    
    public CameraSetupSystem(IWorld world)
    {
        _world = world;
    }

    public ValueTask Execute(CancellationToken cancellationToken)
    {
        var camEntity = _world.CreateEntity();
         _world.SetComponent(camEntity, new Camera(320));
         _world.SetComponent(camEntity, new Position(new(0, 2 * PPU)));
         
         return ValueTask.CompletedTask;
    }
}