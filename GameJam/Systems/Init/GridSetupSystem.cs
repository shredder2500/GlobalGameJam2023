using GameJam.Components;
using GameJam.Engine.Components;
using GameJam.Engine.Rendering;
using GameJam.Engine.Rendering.Components;
using GameJam.Engine.Resources;
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
                 new(16, 16), 12);
    }

    public ValueTask Execute(CancellationToken cancellationToken)
    {
        var halfWidth = GridWidth * CellWidth / 2;
        var halfhHeight = GridHeight * CellHeight / 2;
        for (var x = -halfWidth; x < halfWidth; x += CellWidth)
        {
            for (var y = -halfhHeight; y < halfhHeight; y += CellHeight)
            {
                var entity = _world.CreateEntity();
                _world.SetComponent(entity, new Node());
                _world.SetComponent(entity, new Position(new(x, y)));
                _world.SetComponent(entity, _spriteSheet.GetSprite(0));
                _world.SetComponent(entity, new SpriteLayer(-1, 0));
            }
        }

        return ValueTask.CompletedTask;
    }

    public void Dispose()
    {
        _resources.Unload<Texture>("sprite.stumpy-tileset");
    }
}