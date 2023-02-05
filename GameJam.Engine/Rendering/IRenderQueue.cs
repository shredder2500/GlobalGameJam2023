using GameJam.Engine.Rendering.Components;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace GameJam.Engine.Rendering;

internal interface IRenderQueue
{
    public void Enqueue(Sprite sprite, SpriteLayer layer, Vector2D<int> pos, Vector2D<int> size, float rotation,
        Pivot pivot);
    public void Enqueue(Text text, SpriteLayer layer, Vector2D<int> pos, Vector2D<int> size, float rotation,
        Pivot pivot);
    public void Render(Camera camera, Vector2D<int> pos);
}