using GameJam.Components;
using GameJam.Engine.Animation.Components;
using GameJam.Engine.Components;
using GameJam.Engine.Rendering;
using GameJam.Engine.Rendering.Components;
using GameJam.Engine.Resources;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using static GameJam.Config;

namespace GameJam.Systems;

public class FogOfWar : ISystem
{
    private readonly IWorld _world;

    public FogOfWar(IWorld world)
    {
        _world = world;
    }

    public ValueTask Execute(CancellationToken cancellationToken)
    {
        var results = _world.GetEntityBuckets()
            .Where(x => x.HasComponent<UndergroundElement>() && x.HasComponent<Position>() && x.HasComponent<Hidden>())
            .Select(x =>
                x.GetIndices().Select(i =>
                    (x.GetEntity(i), x.GetComponent<Position>(i)!.Value)))
            .SelectMany(x => x);

        foreach (var (element, position) in results)
        {
            var nextToRoot = _world
                .GetEntityBuckets()
                .Where(x => x.HasComponent<Root>() && x.HasComponent<Position>())
                .Select(x => x.GetIndices().Select(i => x.GetComponent<Position>(i)!.Value))
                .SelectMany(x => x)
                .Any(x => CheckAdjacency(x, position));

            if (nextToRoot)
            {
                _world.RemoveComponent<Hidden>(element);
            }
        }
        
        return ValueTask.CompletedTask;
            
        bool CheckAdjacency(Vector2D<int> root1, Vector2D<int> root2)
        {
            var xDist = Math.Abs(root1.X - root2.X);
            var yDist = Math.Abs(root1.Y - root2.Y);
            return (xDist + yDist) == PPU * FogOfWarDistance;
        }
    }
}