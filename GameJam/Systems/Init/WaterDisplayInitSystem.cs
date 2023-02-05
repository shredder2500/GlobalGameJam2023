using GameJam.Components;
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
public class WaterDisplayInitSystem : ISystem, IDisposable
{
    public GamePhase Phase => GamePhase.Init;
    private readonly IResourceManager _resources;
    private readonly SpriteSheet _spriteSheet;
    private readonly IWorld _world;

    public WaterDisplayInitSystem(IWorld world, IResourceManager resources)
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
        // Get how much starting energy you have
        var energyCount = _world.GetEntityBuckets()
            .Where(x => x.HasComponent<EnergyManagement>())
            .Select(x => x.GetIndices().Select(i => x.GetComponent<EnergyManagement>(i)))
            .SelectMany(x => x).FirstOrDefault();

        if (energyCount == null) return ValueTask.CompletedTask;

        for (int i = 0; i < energyCount; i++)
        {
            // Create water

        }

        return ValueTask.CompletedTask;
    }
}
