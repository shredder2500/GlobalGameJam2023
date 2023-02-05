﻿using GameJam.Components;
using GameJam.Engine.Components;
using GameJam.Engine.Rendering;
using GameJam.Engine.Resources;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameJam.Systems;

public class WaterConsumption : ISystem, IDisposable
{
    private readonly IResourceManager _resources;
    private readonly IWorld _world;
    private readonly SpriteSheet _spriteSheet;

    public WaterConsumption(IResourceManager resources, IWorld world)
    {
        _resources = resources;
        _world = world;
        _spriteSheet = new(resources.Load<Texture>("sprite.stumpy-tileset"), new(320, 128), new(16, 16));
    }

    public void Dispose()
    {
        _resources.Unload<Texture>("sprite.stumpy-tileset");
    }

    public ValueTask Execute(CancellationToken cancellationToken)
    {
        // Remove water component if there is a root at same location
        var water = _world.GetEntityBuckets()
            .Where(x => x.HasComponent<Water>() && x.HasComponent<Position>())
            .Select(x => x.GetIndices().Select(i => x.GetComponent<Position>(i)))
            .SelectMany(x => x).ToArray();

        var search = _world.GetEntityBuckets()
            .Where(x => x.HasComponent<Root>() && x.HasComponent<Position>())
            .Select(x => x.GetIndices().Select(i => x.GetComponent<Position>(i)))
            .SelectMany(x => x).ToArray();

        for (int i = 0; i < water.Length; i++)
        {

        }

        return ValueTask.CompletedTask; 
    }
}