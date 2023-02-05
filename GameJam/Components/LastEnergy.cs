using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameJam.Components;

public record struct LastEnergy(int Value)
{
    public static implicit operator int(LastEnergy score) => score.Value;
    public static implicit operator LastEnergy(int value) => new(value);
}
