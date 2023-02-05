using GameJam.Components;
using GameJam.Engine.Components;
using GameJam.Engine.Rendering.Components;
using Silk.NET.Input;
using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameJam.Systems;

internal class AddRootSystem : ISystem
{
    private readonly IInputContext _inputContext;
    private IWorld _world;

    public AddRootSystem(IInputContext inputContext)
    {
        _inputContext = inputContext;
    }

    public ValueTask Execute(CancellationToken cancellationToken)
    {
        // Check to add root at active location
        if (_inputContext.Keyboards[0].IsKeyPressed(Key.Space))
        {
            // Get active node's position
            var activeNode = _world.GetEntityBuckets()
                .Where(x => x.HasComponent<Active>() && x.HasComponent<Node>())
                .Select(x => x.GetIndices().Select(i => x.GetComponent<Position>(i)))
                .SelectMany(x => x)
                .FirstOrDefault();

            if (activeNode == null) return ValueTask.CompletedTask;

            // Compare active node's position to all other nodes with roots on them
            var roots = _world.GetEntityBuckets()
                .Where(x => x.HasComponent<Root>() && x.HasComponent<Position>())
                .Select(x => x.GetIndices().Select(i => (x.GetComponent<Position>(i))))
                .SelectMany(x => x);

            var activeNodeX = activeNode.Value.Value.X;
            var activeNodeY = activeNode.Value.Value.Y;
            foreach(Position? location in roots)
            {
                if (CheckAdjacency(activeNode.Value, location!.Value))
                {
                    // Found adjacent root -> Can place root here
                    var newRoot = _world.CreateEntity();
                    _world.SetComponent(newRoot, new Root());
                    _world.SetComponent(newRoot, new Position(activeNode.Value));
                    //_world.SetComponent(newRoot, new Sprite());
                    if (CheckSpecificAdjacency(activeNode.Value, location!.Value))
                    {
                        _world.SetComponent(newRoot, new Rotation(0));
                    }
                    else
                    {
                        _world.SetComponent(newRoot, new Rotation(90));
                    }
                    break;
                }
            }
        }

        // Checks if two vectors are adjacent (does not count diagonals)
        bool CheckAdjacency(Vector2D<int> root1, Vector2D<int> root2)
        {
            int xDist = Math.Abs(root1.X - root2.X);
            int yDist = Math.Abs(root1.Y - root2.Y);
            return (xDist + yDist) == 1;
        }

        // True = Horizontal Adjacency, False = Vertical Adjacency
        bool CheckSpecificAdjacency(Vector2D<int> root1, Vector2D<int> root2)
        {
            return Math.Abs(root1.X - root2.X) == 1;
        }

        return ValueTask.CompletedTask;
    }
}
