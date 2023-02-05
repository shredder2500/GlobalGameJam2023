using GameJam.Components;
using GameJam.Engine.Components;
using GameJam.Engine.Rendering;
using GameJam.Engine.Rendering.Components;
using GameJam.Engine.Resources;
using Silk.NET.OpenGL;
using Size = GameJam.Engine.Components.Size;

using static GameJam.Config;

namespace GameJam.Systems.Init;

public class GridSetupSystem : ISystem, IDisposable
{
    public GamePhase Phase => GamePhase.Init;

    private readonly IWorld _world;
    private readonly IResourceManager _resources;
    private readonly SpriteSheet _spriteSheet;

    public GridSetupSystem(IWorld world, IResourceManager resources)
    {
        _world = world;
        _resources = resources;
        _spriteSheet = new(resources.Load<Texture>("sprite.stumpy-tileset"), StumpyTileSheetSize,
            new(PPU, PPU));
    }

    public ValueTask Execute(CancellationToken cancellationToken)
    {
        var gridWidth = GridSize.X;
        var gridHeight = GridSize.Y;
        var cellWidth = PPU;
        var cellHeight = PPU;
        var halfWidth = gridWidth / 2;
        var halfhHeight = gridHeight / 2;
        for (var x = -halfWidth; x < halfWidth; x ++)
        {
            for (var y = -halfhHeight; y < halfhHeight; y ++)
            {
                var entity = _world.CreateEntity();
                _world.SetComponent(entity, new Node());
                _world.SetComponent(entity, new Position(new(x * cellWidth, y * cellHeight)));
                _world.SetComponent(entity, _spriteSheet.GetSprite(0));
                _world.SetComponent(entity, new SpriteLayer(-1, 0));
                
                if (x == 0 && y == halfhHeight - 2)
                {
                    _world.SetComponent(entity, new Active());
                }

                if (y == halfhHeight - 1)
                {

                    var grassEntity = _world.CreateEntity();
                    _world.SetComponent(grassEntity, new Position(new(x * cellWidth, y * cellHeight)));
                    _world.SetComponent(grassEntity, _spriteSheet.GetSprite(20));
                    _world.SetComponent(grassEntity, new SpriteLayer(-1, 1));
                }
            }
        }

        return ValueTask.CompletedTask;
    }

    public void Dispose()
    {
        _resources.Unload<Texture>("sprite.stumpy-tileset");
    }
}