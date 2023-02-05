using GameJam.Components;
using GameJam.Engine.Components;
using GameJam.Engine.Rendering.Components;
using JasperFx.Core;
using Silk.NET.Input;
using Silk.NET.Maths;
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
        if (_inputContext.Keyboards[0].IsKeyPressed(Key.A))
        {
            // Move active to left
            var activeNode = _world.GetEntityBuckets()
                .Where(x => x.HasComponent<Active>() && x.HasComponent<Node>())
                .Select(x => x.GetIndices().Select(i => x.GetComponent<Position>(i)))
                .SelectMany(x => x)
                .FirstOrDefault();
            
            if (activeNode == null) return ValueTask.CompletedTask;


            var adjNodes = _world.GetEntityBuckets()
                .Where(x => x.HasComponent<Node>())
                .Select(x => x.GetIndices().Select(i => x.GetComponent<Position>(i)))
                .SelectMany(x => x);

            Vector2D<int> newPosition = new(0, 0);
            foreach (Position node in adjNodes)
            {
                
            }

            

            var selector = _world.GetEntityBuckets()
                .Where(x => x.HasComponent<Selector>())
                .Select(x => x.GetIndices().Select<int, Entity?>(i => x.GetEntity(i)))
                .SelectMany(x => x)
                .FirstOrDefault();

            if (selector == null) return ValueTask.CompletedTask;

            _world.SetComponent(selector!.Value, activeNode!.Value);
        }

        throw new NotImplementedException();
    }
}
