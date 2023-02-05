using GameJam.Components;
using GameJam.Engine.Components;
using GameJam.Engine.Rendering;
using GameJam.Engine.Rendering.Components;
using GameJam.Engine.Resources;
using Silk.NET.OpenGL;

using static GameJam.Config;

namespace GameJam.Systems.Init;

public class RootSetupSystem : ISystem, IDisposable
{
    public GamePhase Phase => GamePhase.Init;
    private readonly IWorld _world;
    private readonly IResourceManager _resources;
    private readonly SpriteSheet _spriteSheet;

    public RootSetupSystem(IWorld world, IResourceManager resources)
    {
        _world = world;
        _resources = resources;
        _spriteSheet = new(resources.Load<Texture>("sprite.stumpy-tileset"), StumpyTileSheetSize,
            new(PPU, PPU));
    }

    public void Dispose()
    {
        _resources.Unload<Texture>("sprite.stumpy-tileset");
    }

    public ValueTask Execute(CancellationToken cancellationToken)
    {
        // Create three root entities as base
        for (int i = 0; i < 3; i++)
        {
            var newRoot = _world.CreateEntity();
            _world.SetComponent(newRoot, new Position(new((i - 1) * PPU, PPU * ((GridSize.Y - 1) / 2))));
            _world.SetComponent(newRoot, new SpriteLayer(0, 0));
            _world.SetComponent(newRoot, _spriteSheet.GetSprite(i + 1));
            _world.SetComponent(newRoot, new Root());
        }

        return ValueTask.CompletedTask;
    }
}
