using System.Numerics;
using Silk.NET.OpenGL;

namespace GameJam.Engine.Rendering.Components; 

public record struct Sprite(Texture Texture, Vector4 Uv);