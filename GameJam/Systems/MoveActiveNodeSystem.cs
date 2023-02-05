﻿using GameJam.Components;
using GameJam.Engine.Components;
using GameJam.Engine.Rendering.Components;
using JasperFx.Core;
using Silk.NET.Input;
using Silk.NET.Maths;
using SixLabors.ImageSharp.ColorSpaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameJam.Systems;

public class InputSystem : ISystem
{
    private readonly IInputContext _inputContext;
    private IWorld _world;

    public InputSystem(IInputContext inputContext)
    {
        _inputContext = inputContext;
    }

    public ValueTask Execute(CancellationToken cancellationToken)
    {
        // Key Inputs to Move Active Node 
        if (_inputContext.Keyboards[0].IsKeyPressed(Key.A))
        {
            // Move active to left
            MoveActiveHorizontally(-1);
        }
        else if (_inputContext.Keyboards[0].IsKeyPressed(Key.D))
        {
            // Move active to right
            MoveActiveHorizontally(1);
        }
        else if (_inputContext.Keyboards[0].IsKeyPressed(Key.W))
        {
            // Move active to up
            MoveActiveVertically(1);
        }
        else if (_inputContext.Keyboards[0].IsKeyPressed(Key.S))
        {
            // Move active to down
            MoveActiveVertically(-1);
        }

        // Key Input to Change Selector to Active
        if (_inputContext.Keyboards[0].IsKeyPressed(Key.Space))
        {
            var activeNode = _world.GetEntityBuckets()
                .Where(x => x.HasComponent<Active>() && x.HasComponent<Node>())
                .Select(x => x.GetIndices().Select(i => x.GetComponent<Position>(i)))
        }



        // param1 -> -1 if left, +1 if right
        ValueTask MoveActiveHorizontally(int offset)
        {
            var (entity, activeNode) = _world.GetEntityBuckets()
               .Where(x => x.HasComponent<Active>() && x.HasComponent<Node>())
               .Select(x => x.GetIndices().Select(i => (x.GetEntity(i), x.GetComponent<Position>(i))))
               .SelectMany(x => x)
               .FirstOrDefault();

            if (activeNode == null) return ValueTask.CompletedTask;

            var (newActiveNode, _) = _world.GetEntityBuckets()
                .Where(x => x.HasComponent<Node>() && x.HasComponent<Position>())
                .Select(x => x.GetIndices().Select(i => (x.GetEntity(i), x.GetComponent<Position>(i))))
                .SelectMany(x => x)
                .FirstOrDefault(x => x.Item2!.Value.Value.Equals(activeNode.Value.Value with
                {
                    X = activeNode.Value.Value.X + offset
                }));

            _world.RemoveComponent<Active>(entity);
            _world.SetComponent<Active>(newActiveNode, new());

            return ValueTask.CompletedTask;
        }

        // param1 -> -1 if down, +1 if up
        ValueTask MoveActiveVertically(int offset)
        {
            var (entity, activeNode) = _world.GetEntityBuckets()
               .Where(x => x.HasComponent<Active>() && x.HasComponent<Node>())
               .Select(x => x.GetIndices().Select(i => (x.GetEntity(i), x.GetComponent<Position>(i))))
               .SelectMany(x => x)
               .FirstOrDefault();

            if (activeNode == null) return ValueTask.CompletedTask;

            var (newActiveNode, _) = _world.GetEntityBuckets()
                .Where(x => x.HasComponent<Node>() && x.HasComponent<Position>())
                .Select(x => x.GetIndices().Select(i => (x.GetEntity(i), x.GetComponent<Position>(i))))
                .SelectMany(x => x)
                .FirstOrDefault(x => x.Item2!.Value.Value.Equals(activeNode.Value.Value with
                {
                    Y = activeNode.Value.Value.Y + offset
                }));

            _world.RemoveComponent<Active>(entity);
            _world.SetComponent<Active>(newActiveNode, new());

            return ValueTask.CompletedTask;
        }

        return ValueTask.CompletedTask;
    }
}