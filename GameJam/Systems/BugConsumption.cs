using GameJam.Components;
using GameJam.Engine.Components;
using GameJam.Engine.Rendering;
using GameJam.Engine.Resources;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GameJam.Config;

namespace GameJam.Systems;

public class BugConsumption : ISystem, IDisposable
{
    private readonly IResourceManager _resources;
    private readonly IWorld _world;
    private readonly SpriteSheet _spriteSheet;

    public BugConsumption(IResourceManager resources, IWorld world)
    {
        _resources = resources;
        _world = world;
        _spriteSheet = new(resources.Load<Texture>("sprite.stumpy-tileset"), StumpyTileSheetSize,
    new(PPU, PPU));
    }

    public void Dispose()
    {
        _resources.Unload<Texture>("sprite.stumpy-tileset");
    }

    public ValueTask Execute(CancellationToken cancellationToken)
    {
        // Remove water component if there is a root at same location
        var search = _world.GetEntityBuckets()
            .Where(x => x.HasComponent<Root>() && x.HasComponent<Position>())
            .Select(x => x.GetIndices().Select(i => x.GetComponent<Position>(i)))
            .SelectMany(x => x);

        var bug = _world.GetEntityBuckets()
            .Where(x => x.HasComponent<Bug>() && x.HasComponent<Position>())
            .Select(x => x.GetIndices().Select(i => (x.GetEntity(i), x.GetComponent<Position>(i))))
            .SelectMany(x => x).Where(x =>
            {
                return search.Contains(x.Item2);
            });

        foreach (var (entity, _) in bug)
        {
            _world.RemoveComponent<Bug>(entity);
            _world.SetComponent(entity, _spriteSheet.GetSprite(8));
            IncreaseScore(1);
        }

        void IncreaseScore(int amount)
        {
            var player = _world.GetEntityBuckets()
                .Where(x => x.HasComponent<Score>())
                .Select(x => x.GetIndices().Select(i => (x.GetEntity(i), x.GetComponent<Score>(i))))
                .SelectMany(x => x)
                .FirstOrDefault();

            if (player.Item2 != null)
            {
                int currentScore = player.Item2.Value;
                _world.SetComponent(player.Item1, new Score(currentScore + amount));
            }
        }

        return ValueTask.CompletedTask;
    }
}
