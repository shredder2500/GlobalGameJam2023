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

namespace GameJam.Systems.Init;

public class UndergroundSetupSystem : ISystem, IDisposable
{
    public GamePhase Phase => GamePhase.Init;
    private readonly ILogger _logger;
    private readonly IResourceManager _resources;
    private readonly IWorld _world;
    private readonly SpriteSheet _spriteSheet;
    private readonly Sprite[] _undergroundSprites;
    private int spawnCount = 15;

    public UndergroundSetupSystem(IResourceManager resources, IWorld world, ILogger<UndergroundSetupSystem> logger)
    {
        _logger = logger;
        _resources = resources;
        _world = world;
        _spriteSheet = new(resources.Load<Texture>("sprite.stumpy-tileset"), new(320, 128),
        new(16, 16));
        _undergroundSprites = new[] {_spriteSheet.GetSprite(4), 
                                     _spriteSheet.GetSprite(5), 
                                     _spriteSheet.GetSprite(7), 
                                     _spriteSheet.GetSprite(9)};
    }

    public void Dispose()
    {
        _resources.Unload<Texture>("sprite.stumpy-tileset");
    }

    public ValueTask Execute(CancellationToken cancellationToken)
    {
        // Randomly spawn a series of underground elements 
        while (spawnCount > 0)
        {
            int spawnX = new Random().Next(-6, 7) * 16;
            int spawnY = new Random().Next(-5, 3) * 16;
            Vector2D<int> spawnLocation = new Vector2D<int>(spawnX, spawnY);

            var search = _world.GetEntityBuckets()
                .Where(x => (x.HasComponent<Root>() || x.HasComponent<Water>() || x.HasComponent<Bug>() || x.HasComponent<RichSoil>()) && x.HasComponent<Position>())
                .Select(x => x.GetIndices().Select(i => x.GetComponent<Position>(i)))
                .SelectMany(x => x)
                .Where(x => x!.Value.Equals(spawnLocation)).ToArray();

            if (search.Length <= 0)
            {
                int arrayIndex = new Random().Next(0, _undergroundSprites.Length);
                var underground = _world.CreateEntity();
                _world.SetComponent(underground, new Position(spawnLocation));
                _world.SetComponent(underground, _undergroundSprites[arrayIndex]);
                _world.SetComponent(underground, new SpriteLayer(-1, 1));

                // Hard code check what we spawned and add appropriate component
                if (arrayIndex == 0) {
                    _world.SetComponent(underground, new RichSoil());
                } else if (arrayIndex == 1) {
                    _world.SetComponent(underground, new Water());
                } else if (arrayIndex == 2 || arrayIndex == 3) {
                    _world.SetComponent(underground, new Bug());
                } else {

                }

                spawnCount--;
            }
        }

        return ValueTask.CompletedTask;
    }
}
