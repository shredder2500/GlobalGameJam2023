using GameJam.Components;
using GameJam.Engine.Components;
using GameJam.Engine.Rendering;
using GameJam.Engine.Rendering.Components;
using GameJam.Engine.Resources;
using JasperFx.Core;
using Microsoft.Extensions.Logging;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using SixLabors.ImageSharp.ColorSpaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameJam.Systems;

internal class AddRootSystem : ISystem, IDisposable
{
    private readonly ILogger _logger;
    private readonly IResourceManager _resources;
    private readonly IInputContext _inputContext;
    private readonly SpriteSheet _spriteSheet;
    private readonly IWorld _world;
    private readonly Sprite[] _rootSprites;
    private bool _isPressed;

    public AddRootSystem(IWorld world, IResourceManager resources, IInputContext inputContext, ILogger<AddRootSystem> logger)
    {
        _logger = logger;
        _world = world;
        _resources = resources;
        _inputContext = inputContext;
        _spriteSheet = new(resources.Load<Texture>("sprite.stumpy-tileset"), new(320, 128),
         new(16, 16));
        _rootSprites = new[]{ _spriteSheet.GetSprite(104, new(2, 1)), 
                              _spriteSheet.GetSprite(124, new(2, 1)),
                              _spriteSheet.GetSprite(144, new(2, 1))};
    }

    public void Dispose()
    {
        _resources.Unload<Texture>("sprite.stumpy-tileset");
    }

    public ValueTask Execute(CancellationToken cancellationToken)
    {
        // Check to add root at active location
        var spacePressed = _inputContext.Keyboards[0].IsKeyPressed(Key.Space);
        if (spacePressed && !_isPressed)
        {
            var playerStatus = _world.GetEntityBuckets()
                .Where(x => x.HasComponent<EnergyManagement>())
                .Select(x => x.GetIndices().Select(i => (x.GetEntity(i), x.GetComponent<EnergyManagement>(i))))
                .SelectMany(x => x)
                .FirstOrDefault();

            if (playerStatus.Item2 == null) return ValueTask.CompletedTask;

            // Get active node's position
            var activeNode = _world.GetEntityBuckets()
                .Where(x => x.HasComponent<Active>() && x.HasComponent<Node>())
                .Select(x => x.GetIndices().Select(i => x.GetComponent<Position>(i)))
                .SelectMany(x => x)
                .FirstOrDefault();

            if (activeNode == null) return ValueTask.CompletedTask;

            // Check if there is a root already at active node location
            var rootPositionCheck = _world.GetEntityBuckets()
                .Where(x => x.HasComponent<Root>() || x.HasComponent<Stone>())
                .Select(x => x.GetIndices().Select(i => x.GetComponent<Position>(i)))
                .SelectMany(x => x);

            foreach (Position? pos in rootPositionCheck)
            {
                if (pos!.Value.Equals(activeNode.Value.Value))
                {
                    return ValueTask.CompletedTask;
                }
            }

            // Get number of roots (For sprite layering / order)
            var numRoots = _world.GetEntityBuckets()
                .Where(x => x.HasComponent<Root>())
                .Select(x => x.GetIndices())
                .SelectMany(x => x).Count();

            var rootLocations = _world.GetEntityBuckets()
                .Where(x => x.HasComponent<Root>() && x.HasComponent<Position>())
                .Select(x => x.GetIndices().Select(i => (x.GetComponent<Position>(i))))
                .SelectMany(x => x)
                .Where(x => CheckAdjacency(activeNode.Value, x!.Value))
                .ToArray();

            if (!rootLocations.Any()) return ValueTask.CompletedTask;

            var newRoot = _world.CreateEntity();
            _world.SetComponent(newRoot, new Root());
            _world.SetComponent(newRoot, new Position(activeNode.Value));
            _world.SetComponent(newRoot, _rootSprites[new Random().Next(0, _rootSprites.Length)]);
            _world.SetComponent(newRoot, new Size(new(2 * 16, 16)));
            _world.SetComponent(newRoot, new SpriteLayer(0, numRoots));
            _world.SetComponent(newRoot, new Pivot(new(0.25f, 0.5f)));
            switch (CheckSpecificAdjacency(activeNode.Value, rootLocations[new Random().Next(0, rootLocations.Length)]!.Value))
            {
                case 1:
                    // Right
                    _world.SetComponent(newRoot, new Rotation(180));
                    break;
                case 2:
                    // Down
                    _world.SetComponent(newRoot, new Rotation(90));
                    break;
                case 3:
                    // Up
                    _world.SetComponent(newRoot, new Rotation(270));
                    break;
                default:
                    // Default to left
                    break;
            }
            return ValueTask.CompletedTask;
        }

        _isPressed = spacePressed;

        // Checks if two vectors are adjacent (does not count diagonals)
        bool CheckAdjacency(Vector2D<int> root1, Vector2D<int> root2)
        {
            int xDist = Math.Abs(root1.X - root2.X);
            int yDist = Math.Abs(root1.Y - root2.Y);
            return (xDist + yDist) == 16;
        }

        // Checks distance to see where adjacent root is
        int CheckSpecificAdjacency(Vector2D<int> root1, Vector2D<int> root2) 
        {
            int xDist = root1.X - root2.X;
            int yDist = root1.Y - root2.Y;

            if (xDist == -16)
                return 0; // Left
            else if (xDist == 16)
                return 1; // Right
            else if (yDist == -16)
                return 2; // Down
            else if (yDist == 16)
                return 3; // Up
            else
                return 0; // Default to left in case something happens
        }

        void SubtractEnergy(int amount)
        {

        }

        return ValueTask.CompletedTask;
    }
}
