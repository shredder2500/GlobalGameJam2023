using GameJam.Components;
using GameJam.Engine.Components;
using GameJam.Engine.Rendering.Components;
using GameJam.Engine.Rendering;
using GameJam.Engine.Resources;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Silk.NET.OpenGL;
using static GameJam.Config;

namespace GameJam.Systems.UI;


public class ScoreDisplaySystem : ISystem, IDisposable
{
    private readonly ILogger _logger;
    private readonly IResourceManager _resources;
    private readonly SpriteSheet _spriteSheet;
    private readonly IWorld _world;

    public ScoreDisplaySystem(IWorld world, IResourceManager resources, ILogger<WaterDisplaySystem> logger)
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
        // Get 
        var scoreTracker = _world.GetEntityBuckets()
            .Where(x => x.HasComponent<Score>())
            .Select(x => x.GetIndices().Select(i => x.GetComponent<Score>(i)?.Value))
            .SelectMany(x => x)
            .FirstOrDefault();
            
        if (scoreTracker == null) return ValueTask.CompletedTask;

        var scoreText = _world.GetEntityBuckets()
            .Where(x => x.HasComponent<Text>())
            .Select(x => x.GetIndices().Select(i => (x.GetEntity(i), x.GetComponent<ScoreText>(i))))
            .SelectMany(x => x)
            .FirstOrDefault();

        if (scoreText.Item2 == null) return ValueTask.CompletedTask;

        _world.SetComponent<Text>(scoreText.Item1, "Score: " + scoreTracker.Value.ToString());

        return ValueTask.CompletedTask;
    }
}
