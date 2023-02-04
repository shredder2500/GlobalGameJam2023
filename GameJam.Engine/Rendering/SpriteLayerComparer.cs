using GameJam.Engine.Rendering.Components;

namespace GameJam.Engine.Rendering;

public class SpriteLayerComparer : IComparer<SpriteLayer>
{
    public int Compare(SpriteLayer x, SpriteLayer y)
    {
        var layerComparison = x.Layer.CompareTo(y.Layer);
        if (layerComparison != 0) return layerComparison;
        return x.Order.CompareTo(y.Order);
    }
}