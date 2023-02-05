using GameJam.Engine.Rendering.Components;
using GameJam.Engine.Rendering;
using GameJam.Engine.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GameJam.Config;
using Silk.NET.OpenGL;

namespace GameJam.Systems.Init;

public class UISetupSystem : ISystem, IDisposable
{
    public GamePhase Phase => GamePhase.Init;
    private readonly IResourceManager _resources;
    private readonly IWorld _world;
    private readonly SpriteSheet _spriteSheet;

    public UISetupSystem(IResourceManager resources, IWorld world)
    {
        _resources = resources;
        _world = world;
        _spriteSheet = new(resources.Load<Texture>("sprite.stumpy-tileset"), StumpyTileSheetSize,
           new(PPU, PPU));
    }

    public ValueTask Execute(CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }

    public void Dispose()
    {
        _resources.Unload<Texture>("sprite.stumpy-tileset");
    }
}
