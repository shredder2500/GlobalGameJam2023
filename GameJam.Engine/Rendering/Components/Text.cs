using System.Runtime.InteropServices;

namespace GameJam.Engine.Rendering.Components;

public record struct Text(nint Value)
{
    public static implicit operator string(Text text)
    {
        return Marshal.PtrToStringAuto(text.Value)!;
    }

    public static implicit operator Text(string value) => new(Marshal.StringToHGlobalAuto(value));
}