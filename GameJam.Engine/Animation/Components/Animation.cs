using GameJam.Engine.Rendering;

namespace GameJam.Engine.Animation.Components;

public record struct Animation(SpriteSheet Sheet, int StartIdx, int EndIdx);