using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameJam.Components;

public record struct LastScore(int Value)
{
    public static implicit operator int(LastScore score) => score.Value;
    public static implicit operator LastScore(int value) => new(value);
}
