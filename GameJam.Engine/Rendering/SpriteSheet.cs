using System.Drawing;
using System.Numerics;
using GameJam.Engine.Rendering.Components;
using Silk.NET.OpenGL;

namespace GameJam.Engine.Rendering;

public record SpriteSheet(Texture Texture, Size TextureSize, Size CellSize, int RowSize)
{
    public Sprite GetSprite(int idx)
    {
        var row = idx / RowSize;
        var col = idx % RowSize;
        
        var unitX = CellSize.Width / (float)TextureSize.Width;
        var unitY = CellSize.Height / (float)TextureSize.Height;
        var x = unitX * col;
        var y = unitY * row;
        var uv = new Vector4(x, y, x + unitX, y + unitY);

        return new Sprite(Texture, uv);
    }
}