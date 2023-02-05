using GameJam.Engine.Components;
using GameJam.Engine.Rendering.Components;
using GameJam.Engine;
using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameJam.Components;

namespace GameJam.Systems;

internal class NodeSystem : ISystem
{
    private readonly IWorld _world;

    public NodeSystem(IWorld world)
    {
        _world = world;
    }

    public ValueTask Execute(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
