using GameJam.Components;
using GameJam.Engine.Components;
using GameJam.Engine.Rendering.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameJam.Systems;

internal class RootSystem : ISystem
{
    private readonly IWorld _world;

    public RootSystem(IWorld world)
    {
        _world = world;
    }

    public ValueTask Execute(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
