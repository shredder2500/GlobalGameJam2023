using Silk.NET.Maths;

namespace GameJam.Engine.Components;

public record struct Size(Vector2D<int> Value) {
  public static implicit operator Vector2D<int>(Size pos) => pos.Value;
  public static implicit operator Size(Vector2D<int> value) => new(value);
}