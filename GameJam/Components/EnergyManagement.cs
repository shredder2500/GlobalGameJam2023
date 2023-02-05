using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GameJam.Components;

public record struct EnergyManagement(int Value)
{
    public static implicit operator int(EnergyManagement energy) => energy.Value;
    public static implicit operator EnergyManagement(int value) => new(value);
};
