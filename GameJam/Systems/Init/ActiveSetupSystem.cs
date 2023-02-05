using GameJam.Components;
using GameJam.Engine.Animation.Components;
using GameJam.Engine.Components;
using GameJam.Engine.Rendering;
using GameJam.Engine.Rendering.Components;
using GameJam.Engine.Resources;
using Microsoft.Extensions.Logging;
using Silk.NET.OpenGL;

using static GameJam.Config;

namespace GameJam.Systems.Init;

public class ActiveSetupSystem : ISystem, IDisposable
{
    public GamePhase Phase => GamePhase.Init;
    private readonly IWorld _world;
    private readonly IResourceManager _resources;
    private readonly SpriteSheet _spriteSheet;

    public ActiveSetupSystem(IWorld world, IResourceManager resources)
    {
        _world = world;
        _resources = resources;
        _spriteSheet = new(resources.Load<Texture>("sprite.stumpy-tileset"), StumpyTileSheetSize,
                 new(PPU, PPU));
    }

    public ValueTask Execute(CancellationToken cancellationToken)
    {
        var initActive = _world.CreateEntity();
        _world.SetComponent(initActive, new Position(new (0, 0)));
        _world.SetComponent(initActive, new SpriteLayer(1, 0));
        _world.SetComponent(initActive, _spriteSheet.GetSprite(87));
        _world.SetComponent(initActive, new Selector());
        _world.SetComponent(initActive, new Animation(_spriteSheet, 87, 88));
        _world.SetComponent(initActive, new LoopAnimation());

        var textEntity = _world.CreateEntity();
        _world.SetComponent(textEntity, new Position(new(0, 0)));
        _world.SetComponent(textEntity, new SpriteLayer(100, 0));
        _world.SetComponent<Text>(textEntity, "Hello World");
        
        return ValueTask.CompletedTask;
    }

    public void Dispose()
    {
        _resources.Unload<Texture>("sprite.stumpy-tileset");
    }
}


