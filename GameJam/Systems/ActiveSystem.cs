using GameJam.Components;
using GameJam.Engine.Components;
using GameJam.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameJam.Systems;

public class ActiveSystem : ISystem
{
    private IWorld _world;

    public ValueTask Execute(CancellationToken cancellationToken)
    {
        var activeNode = _world.GetEntityBuckets()
                .Where(x => x.HasComponent<Active>() && x.HasComponent<Node>())
                .Select(x => x.GetIndices().Select(i => x.GetComponent<Position>(i)))
                .SelectMany(x => x)
                .FirstOrDefault();

        if (activeNode == null) return ValueTask.CompletedTask;

        var selector = _world.GetEntityBuckets()
            .Where(x => x.HasComponent<Selector>())
            .Select(x => x.GetIndices().Select<int, Entity?>(i => x.GetEntity(i)))
            .SelectMany(x => x)
            .FirstOrDefault();

        if (selector == null) return ValueTask.CompletedTask;

        _world.SetComponent(selector!.Value, activeNode!.Value);
        return ValueTask.CompletedTask;
    }
}
