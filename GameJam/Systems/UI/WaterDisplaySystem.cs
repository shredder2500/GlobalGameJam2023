using GameJam.Components;
using GameJam.Engine.Components;
using GameJam.Engine.Rendering;
using GameJam.Engine.Rendering.Components;
using GameJam.Engine.Resources;
using Microsoft.Extensions.Logging;
using Silk.NET.Maths;
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
    private readonly ILogger _logger;
    private readonly IResourceManager _resources;
    private readonly SpriteSheet _spriteSheet;
    private readonly IWorld _world;
    private int spawnOffset = 6;

    public WaterDisplaySystem(IWorld world, IResourceManager resources, ILogger<WaterDisplaySystem> logger)
    {
        _logger = logger;
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
        // Get existing UI elements
        var waterUi = _world.GetEntityBuckets()
            .Where(x => x.HasComponent<WaterDroplet>() && x.HasComponent<Position>() && x.HasComponent<SpriteLayer>())
            .Select(x => x.GetIndices().Select(i => (x.GetEntity(i), x.GetComponent<SpriteLayer>(i)!.Value.Order)))
            .SelectMany(x => x)
            .OrderBy(x => x.Item2)
            .Select(x => x.Item1)
            .ToArray();

        var energy = _world.GetEntityBuckets()
            .Where(x => x.HasComponent<EnergyManagement>())
            .Select(x => x.GetIndices().Select(i => x.GetComponent<EnergyManagement>(i)!.Value))
            .SelectMany(x => x)
            .First();

        int discrepancy = energy - waterUi.Length;
        int dir = discrepancy > 0 ? 1 : -1;

        for (int i = 0; i < Math.Abs(discrepancy); i++)
        {
            if (dir == 1)
            {
                // add entity
                var newWaterUI = _world.CreateEntity();
                var offset = (waterUi.Length + i) * spawnOffset;
                _world.SetComponent(newWaterUI, _spriteSheet.GetSprite(10));
                _world.SetComponent(newWaterUI, new WaterDroplet());
                _world.SetComponent(newWaterUI, new Position(new(-GridSize.X / 2 * PPU + offset, GridSize.Y / 2 * PPU + PPU * 3)));
                _world.SetComponent(newWaterUI, new Size(new((int)(PPU * 1.5), (int)(PPU * 1.5))));
                _world.SetComponent(newWaterUI, new SpriteLayer(1, waterUi.Length + i));
            } else 
            {
                // remove entity
                _world.DestroyEntity(waterUi.Last());
            }
        }

        return ValueTask.CompletedTask;
    }
}
