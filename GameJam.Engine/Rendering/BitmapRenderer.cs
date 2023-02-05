using System.Drawing;
using System.Numerics;
using GameJam.Engine.Resources;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace GameJam.Engine.Rendering;

public class BitmapRenderer : IDisposable, IBitmapRenderer {
    private readonly IResourceManager _resources;
    private readonly Shader _shader;
    private readonly GL _gl;
  
    private readonly BufferObject<float> _vbo;
    private readonly BufferObject<uint> _ebo;
    private readonly VertexArrayObject<float, uint> _vao;
  
    private readonly int _uTextureLocation;
    private readonly int _uProjectionLocation;
    private readonly int _uColorLocation;

    //Vertex data, uploaded to the VBO.
    private static float[] Vertices(float x, float y, float width, float height, Vector4 uv) => new []
    {
        //X    Y
        x, y, uv.X, uv.W,
        x + width, y, uv.Z, uv.W,
        x, y + height, uv.X, uv.Y,
        x + width, y + height, uv.Z, uv.Y
    };

    //Index data, uploaded to the EBO.
    // private static readonly uint[] Indices =
    // {
    //   0, 2, 1,
    //   2, 3, 1
    // };

    public BitmapRenderer(IResourceManager resources, IWindow window) {
        _gl = GL.GetApi(window);
        _resources = resources;
        _shader = resources.Load<Shader>("shader.font");
    
        _ebo = new (_gl, Array.Empty<uint>(), BufferTargetARB.ElementArrayBuffer);
        _vbo = new (_gl, Array.Empty<float>(), BufferTargetARB.ArrayBuffer);
        _vao = new (_gl, _vbo, _ebo);

        _vao.VertexAttributePointer(0, 2, VertexAttribPointerType.Float, 4, 0);
        _vao.VertexAttributePointer(1, 2, VertexAttribPointerType.Float, 4, 2);
        _vao.UnBind();
    
        _uTextureLocation = GetLocation("uTexture0");
        _uProjectionLocation = GetLocation("uProjection");
        _uColorLocation = GetLocation("uColor");
    
        int GetLocation(string name)
        {
            var location = _gl.GetUniformLocation(_shader.Handle, name);
            if (location == -1)
            {
                throw new($"{name} uniform not found on shader.");
            }
    
            return location;
        }
    }
  
    public unsafe void RenderText(Vector2 position, string text, BitmapFont font, Color color) {
        var numOfColumns = font.Width / font.CellWidth;
        _gl.UseProgram(_shader.Handle);
        _vao.Bind();
        var vertInfo = new List<float>();
        var indices = new List<uint>();
        var widths = font.CharWidths.Span;

        var offset = 0;
        for (var i = 0; i < text.Length; i++) {
            var index = (int)text[i] - font.CharOffset;
            var col = index / numOfColumns;
            var row = index % numOfColumns;
      
            var uv = new Vector4(row / 16f, col / 16f, (row / 16f) + (1 / 16f), (col / 16f) + (1 / 16f));
            vertInfo.AddRange(Vertices(position.X + offset, position.Y, font.CellWidth, font.CellHeight, uv));
            offset += widths[(int)text[i]];
            indices.AddRange(new uint[] {
                0, 2, 1,
                2, 3, 1
            }.Select(x => x + (uint)(i * 4)));
        }
    
        _vbo.Upload(vertInfo.ToArray());
        _ebo.Upload(indices.ToArray());
        _gl.ActiveTexture(TextureUnit.Texture0);
        _gl.BindTexture(TextureTarget.Texture2D, font.Handle);
        _gl.Uniform1(_uTextureLocation, 0);
        _gl.Uniform4(_uColorLocation, color.R / 256f, color.G / 256f, color.B / 256f, color.A / 256f);
        SetMatrix(_uProjectionLocation, Matrix4x4.CreateOrthographicOffCenter(0, 160, 0, 144, -1, 1));
    
        _gl.DrawElements(PrimitiveType.Triangles, (uint) indices.Count, DrawElementsType.UnsignedInt, null);
        _vao.UnBind();
    
        void SetMatrix(int location, Matrix4x4 value) =>
            _gl.UniformMatrix4(location, 1, false, (float*)&value);
    }

    public void Dispose() {
        _resources.Unload<Shader>("shader.defaultFont");
        _vao.Dispose();
        _vbo.Dispose();
        _ebo.Dispose();
    }
}