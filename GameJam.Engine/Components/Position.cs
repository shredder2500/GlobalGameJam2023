using Silk.NET.Maths;

namespace GameJam.Engine.Components;

public record struct Position(Vector2D<int> Value) {
  public static implicit operator Vector2D<int>(Position pos) => pos.Value;
  public static implicit operator Position(Vector2D<int> value) => new(value);
}