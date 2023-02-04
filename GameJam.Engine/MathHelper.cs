using System.Numerics;

using static System.MathF;

namespace GameJam.Engine;

public static class MathHelper
{
    public static float DegreesToRadians(float degrees)
    {
        return MathF.PI / 180f * degrees;
    }

    public static Vector2 RotateVector(Vector2 vector, float radians) => new(
        Cos(radians) * vector.X - Sin(radians) * vector.Y,
        Sin(radians) * vector.X + Cos(radians) * vector.Y);

    public static Vector2 RotateAround(Vector2 point, float radians, Vector2 origin) => new(
        Cos(radians) * (point.X - origin.X) - Sin(radians) * (point.Y - origin.Y) + origin.X,
        Sin(radians) * (point.X - origin.X) + Cos(radians) * (point.Y - origin.Y) + origin.Y);

    public static void Deconstruct(this Vector2 vector, out float x, out float y) => (x, y) = (vector.X, vector.Y);
}