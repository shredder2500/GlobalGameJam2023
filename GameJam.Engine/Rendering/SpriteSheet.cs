using System.Drawing;
using System.Numerics;
using GameJam.Engine.Rendering.Components;
using Silk.NET.OpenGL;

namespace GameJam.Engine.Rendering;

public record struct SpriteSheet(Texture Texture, Size TextureSize, Size CellSize)
{
    public Sprite GetSprite(int idx) => GetSprite(idx, new(1, 1));
    public Sprite GetSprite(int idx, Size size)
    {
        var rowSize = TextureSize.Width / CellSize.Width;
        var col = idx / rowSize;
        var row = idx % rowSize;
        
        var unitX = CellSize.Width / (float)TextureSize.Width;
        var unitY = CellSize.Height / (float)TextureSize.Height;
        var x = unitX * row;
        var y = unitY * col;
        var uv = new Vector4(x, y, x + unitX * size.Width, y + unitY * size.Height);

        return new Sprite(Texture, uv);
    }
}