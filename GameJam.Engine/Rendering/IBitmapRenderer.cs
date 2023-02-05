using System.Drawing;
using System.Numerics;

namespace GameJam.Engine.Rendering;

public interface IBitmapRenderer {
    unsafe void RenderText(Vector2 position, string text, BitmapFont font, Color color);
}