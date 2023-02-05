using GameJam.Components;
using GameJam.Engine.Components;
using GameJam.Engine.Rendering;
using GameJam.Engine.Rendering.Components;
using GameJam.Engine.Resources;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace GameJam.Systems.Init;

public class GridSetupSystem : ISystem, IDisposable
{
    public GamePhase Phase => GamePhase.Init;

    private const int GridWidth = 14;
    private const int GridHeight = 10;
    private const int CellWidth = 16;
    private const int CellHeight = 16;

    private readonly IWorld _world;
    private readonly IResourceManager _resources;
    private readonly SpriteSheet _spriteSheet;

    public GridSetupSystem(IWorld world, IResourceManager resources)
    {
        _world = world;
        _resources = resources;
        _spriteSheet = new(resources.Load<Texture>("sprite.stumpy-tileset"), new(320, 128),
                 new(16, 16));
    }

    public ValueTask Execute(CancellationToken cancellationToken)
    {
        var halfWidth = GridWidth / 2;
        var halfhHeight = GridHeight / 2;
        for (var x = -halfWidth; x < halfWidth; x ++)
        {
            for (var y = -halfhHeight; y < halfhHeight; y ++)
            {
                var entity = _world.CreateEntity();
                _world.SetComponent(entity, new Node());
                _world.SetComponent(entity, new Position(new(x * CellWidth, y * CellHeight)));
                _world.SetComponent(entity, _spriteSheet.GetSprite(0));
                _world.SetComponent(entity, new SpriteLayer(-1, 0));
                
               

                if (y == halfhHeight - 1)
                {
                    if (x == 0)
                    {
                        _world.SetComponent(entity, new Active());
                    }

                    var grassEntity = _world.CreateEntity();
                    _world.SetComponent(grassEntity, new Position(new(x * CellWidth, y * CellHeight)));
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