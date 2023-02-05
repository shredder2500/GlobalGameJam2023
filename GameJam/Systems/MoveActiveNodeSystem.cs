using GameJam.Components;
using GameJam.Engine.Components;
using GameJam.Engine.Rendering.Components;
using JasperFx.Core;
using Microsoft.Extensions.Logging;
using Silk.NET.Input;
using Silk.NET.Maths;
using SixLabors.ImageSharp.ColorSpaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameJam.Systems;

public class MoveActiveNodeSystem : ISystem
{
    private readonly ILogger _logger;
    private readonly IInputContext _inputContext;
    private readonly IWorld _world;

    private bool _isPressed;

    public MoveActiveNodeSystem(IWorld world, ILogger<MoveActiveNodeSystem> logger, IInputContext inputContext)
    {
        _logger = logger;
        _world = world;
        _inputContext = inputContext;
    }

    public ValueTask Execute(CancellationToken cancellationToken)
    {
        
        // Key Inputs to Move Active Node 
        var aPressed = _inputContext.Keyboards[0].IsKeyPressed(Key.A);
        var dPressed = _inputContext.Keyboards[0].IsKeyPressed(Key.D);
        var wPressed = _inputContext.Keyboards[0].IsKeyPressed(Key.W);
        var sPressed = _inputContext.Keyboards[0].IsKeyPressed(Key.S);
        if (aPressed && !_isPressed)
        {
            // Move active to left
            MoveActiveHorizontally(-1);
        }
        else if (dPressed && !_isPressed)
        {
            // Move active to right
            MoveActiveHorizontally(1);
        }
        else if (wPressed && !_isPressed)
        {
            // Move active to up
            MoveActiveVertically(1);
        }
        else if (sPressed && !_isPressed)
        {
            // Move active to down
            MoveActiveVertically(-1);
        }

        _isPressed = aPressed || dPressed || wPressed || sPressed;
        

        // param1 -> -1 if left, +1 if right
        void MoveActiveHorizontally(int offset)
        {
            var (entity, activeNode) = _world.GetEntityBuckets()
               .Where(x => x.HasComponent<Active>() && x.HasComponent<Node>())
               .Select(x => x.GetIndices().Select(i => (x.GetEntity(i), x.GetComponent<Position>(i))))
               .SelectMany(x => x)
               .FirstOrDefault();

            if (activeNode == null) return ;

            var (newActiveNode, something) = _world.GetEntityBuckets()
                .Where(x => x.HasComponent<Node>() && x.HasComponent<Position>())
                .Select(x => x.GetIndices().Select(i => (x.GetEntity(i), x.GetComponent<Position>(i))))
                .SelectMany(x => x)
                .FirstOrDefault(x => x.Item2!.Value.Value.Equals(activeNode.Value.Value with
                {
                    X = activeNode.Value.Value.X + offset * 16
                }));
            
            if (something == null) return;

            _world.RemoveComponent<Active>(entity);
            _world.SetComponent<Active>(newActiveNode, new());
        }

        // param1 -> -1 if down, +1 if up
        void MoveActiveVertically(int offset)
        {
            var (entity, activeNode) = _world.GetEntityBuckets()
               .Where(x => x.HasComponent<Active>() && x.HasComponent<Node>())
               .Select(x => x.GetIndices().Select(i => (x.GetEntity(i), x.GetComponent<Position>(i))))
               .SelectMany(x => x)
               .FirstOrDefault();

            if (activeNode == null) return;

            _logger.LogInformation("found active node");

            var (newActiveNode, something) = _world.GetEntityBuckets()
                .Where(x => x.HasComponent<Node>() && x.HasComponent<Position>())
                .Select(x => x.GetIndices().Select(i => (x.GetEntity(i), x.GetComponent<Position>(i))))
                .SelectMany(x => x)
                .FirstOrDefault(x => x.Item2!.Value.Value.Equals(activeNode.Value.Value with
                {
                    Y = activeNode.Value.Value.Y + offset * 16
                }));

            if (something == null) return;

            _world.RemoveComponent<Active>(entity);
            _world.SetComponent<Active>(newActiveNode, new());

            _logger.LogInformation("new active node set");
        }

        return ValueTask.CompletedTask;
    }
}
