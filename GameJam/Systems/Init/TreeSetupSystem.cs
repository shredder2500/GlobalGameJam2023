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
    private readonly IResourceManager _resources;
    private readonly SpriteSheet _spriteSheet;
    private readonly IWorld _world;

    public TreeSetupSystem(IWorld world, IResourceManager resources)
    {
        _world = world;
        _spriteSheet = new(resources.Load<Texture>("sprite.stumpy-tileset"), new(320, 128),
                 new(16, 16));
    }

    public void Dispose()
    {
        _resources.Unload<Texture>("sprite.stump-tileset");
    }

    public ValueTask Execute(CancellationToken cancellationToken)
    {
        // Create Stumpy
        var newTreePart = _world.CreateEntity();
        _world.SetComponent(newTreePart, _spriteSheet.GetSprite(62)); // 22, 42, 62
        _world.SetComponent(newTreePart, new Position(new(0, 80)));
        

        return ValueTask.CompletedTask;

        
    }
}
