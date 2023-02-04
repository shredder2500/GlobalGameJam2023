namespace GameJam.Engine.Components;

public record struct Rotation(float Value) {
  public static implicit operator float(Rotation rot) => rot.Value;
  public static implicit operator Rotation(float value) => new(value);
}