using GameJam.Engine.Components;
using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameJam.Components;

// This value is location of the next water droplet you can place (in UI)
public record struct WaterUI(Vector2D<int> Value)
{
    public static implicit operator Vector2D<int>(WaterUI pos) => pos.Value;
    public static implicit operator WaterUI(Vector2D<int> value) => new(value);
}
