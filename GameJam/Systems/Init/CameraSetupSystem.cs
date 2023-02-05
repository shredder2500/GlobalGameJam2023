using GameJam.Engine.Components;
using GameJam.Engine.Rendering.Components;

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
         _world.SetComponent(camEntity, new Camera(200));
         _world.SetComponent(camEntity, new Position(new(0, 0)));
         
         return ValueTask.CompletedTask;
    }
}