using GameJam.Components;
using GameJam.Engine.Components;
using GameJam.Engine.Rendering;
using GameJam.Engine.Resources;
using Silk.NET.OpenGL;

using static GameJam.Config;

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
        _spriteSheet = new(resources.Load<Texture>("sprite.stumpy-tileset"), StumpyTileSheetSize,
            new(PPU, PPU));
    }

    public void Dispose()
    {
        _resources.Unload<Texture>("sprite.stump-tileset");
    }

    public ValueTask Execute(CancellationToken cancellationToken)
    {
        // Create Stumpy
        var treeEntity = _world.CreateEntity();
        _world.SetComponent(treeEntity, new Position(new(0, ((GridSize.Y / 2) + 1) * PPU)));
        _world.SetComponent(treeEntity, _spriteSheet.GetSprite(21, new (3, 3)));
        _world.SetComponent(treeEntity, new Size(new(PPU * 3, PPU * 3)));
        _world.SetComponent(treeEntity, new Score(0));
        _world.SetComponent(treeEntity, new EnergyManagement(5));

        return ValueTask.CompletedTask;
    }
}
