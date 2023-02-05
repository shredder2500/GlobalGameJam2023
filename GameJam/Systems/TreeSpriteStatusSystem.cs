using GameJam.Components;
using GameJam.Engine.Rendering;
using GameJam.Engine.Rendering.Components;
using GameJam.Engine.Resources;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static GameJam.Config;

namespace GameJam.Systems;

public class TreeSpriteStatusSystem : ISystem, IDisposable
{
    private readonly IWorld _world;
    private readonly SpriteSheet _spritesheet;
    private readonly IResourceManager _resources;
    private readonly Sprite[] _treeDeadSprites;

    public TreeSpriteStatusSystem(IWorld world, IResourceManager resources)
    {
        _world = world;
        _resources = resources;
        _spritesheet = new(resources.Load<Texture>("sprite.stumpy-tileset"), StumpyTileSheetSize,
           new(PPU, PPU));
        _treeDeadSprites = new[] {
             _spritesheet.GetSprite(186, new(3, 3)),
        };
    }

    public void Dispose()
    {
        _resources.Unload<Texture>("sprite.stumpy-tileset");
    }

    public ValueTask Execute(CancellationToken cancellationToken)
    {
        var energyStatus = _world.GetEntityBuckets()
            .Where(x => x.HasComponent<EnergyManagement>() && x.HasComponent<Sprite>())
            .Select(x => x.GetIndices().Select(i => (x.GetEntity(i), x.GetComponent<LastEnergy>(i), x.GetComponent<EnergyManagement>(i))))
            .SelectMany(x => x)
            .FirstOrDefault();

        if (energyStatus.Item2 == null || energyStatus.Item3 == null) return ValueTask.CompletedTask;

        int energyDiff = energyStatus.Item3.Value - energyStatus.Item2.Value;
        if (energyDiff == 0 && energyStatus.Item3.Value <= 0)
        {
            _world.SetComponent(energyStatus.Item1, _treeDeadSprites[0]);
            var treeEyes = _world.GetEntityBuckets()
                .Where(x => x.HasComponent<Eye>())
                .Select(x => x.GetIndices().Select(i => x.GetEntity(i)))
                .SelectMany(x => x).FirstOrDefault();
            _world.RemoveComponent<Sprite>(treeEyes);
        }
  
        return ValueTask.CompletedTask;
    }
}
