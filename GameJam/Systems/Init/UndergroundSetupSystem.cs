using GameJam.Components;
using GameJam.Engine.Components;
using GameJam.Engine.Rendering;
using GameJam.Engine.Rendering.Components;
using GameJam.Engine.Resources;
using Microsoft.Extensions.Logging;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static GameJam.Config;

namespace GameJam.Systems.Init;

public class UndergroundSetupSystem : ISystem, IDisposable
{
    public GamePhase Phase => GamePhase.Init;
    private readonly ILogger _logger;
    private readonly IResourceManager _resources;
    private readonly IWorld _world;
    private readonly SpriteSheet _spriteSheet;
    private readonly (Sprite sprite, int chance, Action<Entity> onCreate)[] _undergroundSprites;
    private int spawnCount = 15;

    public UndergroundSetupSystem(IResourceManager resources, IWorld world, ILogger<UndergroundSetupSystem> logger)
    {
        _logger = logger;
        _resources = resources;
        _world = world;
        _spriteSheet = new(resources.Load<Texture>("sprite.stumpy-tileset"), StumpyTileSheetSize,
            new(PPU, PPU));
        _undergroundSprites = new (Sprite sprite, int chance, Action<Entity> onCreate)[] {
                (_spriteSheet.GetSprite(4), 10, x => world.SetComponent(x, new RichSoil())), 
                 (_spriteSheet.GetSprite(5), 40, x => world.SetComponent(x, new Water())), 
                 (_spriteSheet.GetSprite(7), 10, x => world.SetComponent(x, new Bug())), 
                 (_spriteSheet.GetSprite(9), 20, x => world.SetComponent(x, new Bug())),
                 (_spriteSheet.GetSprite(146), 20, x => world.SetComponent(x, new Stone()))
             }
            .OrderBy(x => x.Item2).ToArray();
    }

    public void Dispose()
    {
        _resources.Unload<Texture>("sprite.stumpy-tileset");
    }

    public ValueTask Execute(CancellationToken cancellationToken)
    {
        // Randomly spawn a series of underground elements 

        var random = new Random();

        var points = new HashSet<Vector2D<int>>();

        while (points.Count < spawnCount)
        {
            var spawnX = random.Next(-(GridSize.X / 2), GridSize.X / 2) * PPU;
            var spawnY = random.Next(-(GridSize.Y / 2), (GridSize.Y - 1) / 2) * PPU;
            
            points.Add(new(spawnX, spawnY));
        }

        foreach (var point in points)
        {
            var rndValue = random.Next(0, 101);
            Console.WriteLine(rndValue);
            var (sprite, onCreate) = GetRandomSprite();
            
            var underground = _world.CreateEntity();
            _world.SetComponent(underground, new Position(point));
            _world.SetComponent(underground, sprite);
            _world.SetComponent(underground, new SpriteLayer(-1, 1));
            onCreate(underground);
        }

        (Sprite, Action<Entity>) GetRandomSprite()
        {
            var rndValue = random.Next(0, 101);
            foreach (var (sprite, chance, onCreate) in _undergroundSprites)
            {
                rndValue -= chance;
                if (rndValue <= 0)
                    return (sprite, onCreate);
            }

            throw new Exception("Couldn't get rnd sprite, check if chances add up to 100");
        }

        return ValueTask.CompletedTask;
    }
}
