using GameJam.Components;
using GameJam.Engine.Components;
using GameJam.Engine.Rendering;
using GameJam.Engine.Resources;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameJam.Systems.Init;

public class TreeSetupSystem : ISystem, IDisposable
{
    public GamePhase Phase => GamePhase.Init;
    private readonly IResourceManager _resources;
    private readonly SpriteSheet _spriteSheet;
    private readonly IWorld _world;

    public TreeSetupSystem(IWorld world, IResourceManager resources)
    {
        _world = world;
        _resources = resources;
        _spriteSheet = new(_resources.Load<Texture>("sprite.stumpy-tileset"), new(320, 128),
                 new(16, 16));
    }

    public void Dispose()
    {
        _resources.Unload<Texture>("sprite.stump-tileset");
    }

    public ValueTask Execute(CancellationToken cancellationToken)
    {
        var player = _world.CreateEntity();
        _world.SetComponent(player, new Score(0));
        _world.SetComponent(player, new EnergyManagement(5));

        return ValueTask.CompletedTask;

        
    }
}
