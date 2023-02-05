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

using static GameJam.Config;

namespace GameJam.Systems;

public class MoveActiveNodeSystem : ISystem, IDisposable
{
    private readonly IResourceManager _resources;
    private readonly SpriteSheet _spriteSheet;
    private readonly ILogger _logger;
    private readonly IInputContext _inputContext;
    private readonly IWorld _world;

    private bool _isPressed;

    public MoveActiveNodeSystem(IWorld world, IResourceManager resources, ILogger<MoveActiveNodeSystem> logger, IInputContext inputContext)
    {
        _resources= resources;
        _spriteSheet = new(resources.Load<Texture>("sprite.stumpy-tileset"), StumpyTileSheetSize, new(PPU, PPU));
        _logger = logger;
        _world = world;
        _inputContext = inputContext;
    }

    public void Dispose()
    {
        throw new NotImplementedException();
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
            UpdateTreeEyes(2);
        }
        else if (dPressed && !_isPressed)
        {
            // Move active to right
            MoveActiveHorizontally(1);
            UpdateTreeEyes(3);
        }
        else if (wPressed && !_isPressed)
        {
            // Move active to up
            MoveActiveVertically(1);
            UpdateTreeEyes(0);
        }
        else if (sPressed && !_isPressed)
        {
            // Move active to down
            MoveActiveVertically(-1);
            UpdateTreeEyes(1);
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
                    X = activeNode.Value.Value.X + offset * PPU
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

            var (newActiveNode, something) = _world.GetEntityBuckets()
                .Where(x => x.HasComponent<Node>() && x.HasComponent<Position>())
                .Select(x => x.GetIndices().Select(i => (x.GetEntity(i), x.GetComponent<Position>(i))))
                .SelectMany(x => x)
                .FirstOrDefault(x => x.Item2!.Value.Value.Equals(activeNode.Value.Value with
                {
                    Y = activeNode.Value.Value.Y + offset * PPU
                }));

            if (something == null) return;

            _world.RemoveComponent<Active>(entity);
            _world.SetComponent<Active>(newActiveNode, new());
        }

        // 0 = Up, 1 = Down, 2 = Left, 3 = Right
        void UpdateTreeEyes(int dir)
        {
            var treeEyes = _world.GetEntityBuckets()
                .Where(x => x.HasComponent<Eye>())
                .Select(x => x.GetIndices().Select(i => x.GetEntity(i)))
                .SelectMany(x => x).FirstOrDefault();

            // 46 = Down, 214 = Left, 234 = Right, 49 = Up
            if (dir == 0)
            {
                // Up
                _world.SetComponent(treeEyes, _spriteSheet.GetSprite(49));
            }
            else if (dir == 1)
            {
                // Down
                _world.SetComponent(treeEyes, _spriteSheet.GetSprite(46));
            }
            else if (dir == 2)
            {
                // Left
                _world.SetComponent(treeEyes, _spriteSheet.GetSprite(214));
            }
            else if (dir == 3)
            {
                // Right
                _world.SetComponent(treeEyes, _spriteSheet.GetSprite(234));
            }
            else
            {
                // Default to Down
                _world.SetComponent(treeEyes, _spriteSheet.GetSprite(49));
            }
        }

        return ValueTask.CompletedTask;
    }
}
