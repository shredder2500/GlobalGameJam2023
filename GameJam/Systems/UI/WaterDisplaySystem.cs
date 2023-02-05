using GameJam.Engine.Rendering;
using GameJam.Engine.Resources;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GameJam.Config;
namespace GameJam.Systems.UI;

public class WaterDisplaySystem : ISystem, IDisposable
{ 
    private readonly IResourceManager _resources;
    private readonly SpriteSheet _spriteSheet;
    private readonly IWorld _world;

    public WaterDisplaySystem(IWorld world, IResourceManager resources)
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


        return ValueTask.CompletedTask;
    }
}
