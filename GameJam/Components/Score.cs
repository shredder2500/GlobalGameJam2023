using GameJam.Engine.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameJam.Components;

public record struct Score(int Value)
{
    public static implicit operator int(Score score) => score.Value;
    public static implicit operator Score(int value) => new(value);
};
