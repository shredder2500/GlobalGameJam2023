using System.Numerics;
using GameJam.Engine.Rendering.Components;
using GameJam.Engine.Resources;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

using static GameJam.Engine.MathHelper;

namespace GameJam.Engine.Rendering;

internal class RenderQueue : IRenderQueue, IDisposable
{
    private readonly PriorityQueue<(Sprite, Vector2D<int>, Vector2D<int>, float), SpriteLayer> _renderQueue;
    private readonly IWindow _window;
    private readonly GL _gl;
    private readonly Shader _shader;
    private readonly IMainThreadDispatcher _dispatcher;
    private readonly IResourceManager _resources;
    
    private readonly int _uTextureLocation;
    private readonly int _uViewLocation;
    private readonly int _uProjectionLocation;
    private readonly int _uModelLocation;
    
    private readonly BufferObject<float> _vbo;
    private readonly BufferObject<uint> _ebo;
    private readonly VertexArrayObject<float, uint> _vao;
    
    private const float Min = -.5f;
    private const float Max = .5f;

    //Vertex data, uploaded to the VBO.
    private static float[] Vertices(Vector4 uv) => new []
    {
        //X    Y
        Min, Min, uv.X, uv.W,
        Max, Min, uv.Z, uv.W,
        Min, Max, uv.X, uv.Y,
        Max, Max, uv.Z, uv.Y
    };

    //Index data, uploaded to the EBO.
    private static readonly uint[] Indices =
    {
        0, 2, 1,
        2, 3, 1
    };

    public RenderQueue(IWindow window, IResourceManager resources, IMainThreadDispatcher dispatcher)
    {
        _window = window;
        _resources = resources;
        _dispatcher = dispatcher;
        _renderQueue = new(new SpriteLayerComparer());
        _shader = resources.Load<Shader>("shader.sprite");

        _gl = GL.GetApi(window);
        
        _ebo = new (_gl, Indices, BufferTargetARB.ElementArrayBuffer);
        _vbo = new (_gl, Vertices(new (0, 0, 1, 1)), BufferTargetARB.ArrayBuffer);
        _vao = new (_gl, _vbo, _ebo);

        _vao.VertexAttributePointer(0, 2, VertexAttribPointerType.Float, 4, 0);
        _vao.VertexAttributePointer(1, 2, VertexAttribPointerType.Float, 4, 2);
        _vao.UnBind();
    
        _uTextureLocation = GetLocation("uTexture0");
        _uViewLocation = GetLocation("uView");
        _uProjectionLocation = GetLocation("uProjection");
        _uModelLocation = GetLocation("uModel");
    
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

    public void Enqueue(Sprite sprite, SpriteLayer layer, Vector2D<int> pos, Vector2D<int> size, float rotation)
    {
        lock (_renderQueue) {
            _renderQueue.Enqueue((sprite, pos, size, rotation), layer);
        }
    }

    public unsafe void Render(Camera camera, Vector2D<int> position)
    {
        _dispatcher.Enqueue(() =>
        {
            lock (_renderQueue)
            {
                var camSize = camera.Size;

                var aspectRatio = (float)_window.Size.X / _window.Size.Y;
                var width = aspectRatio * camSize;

                var right = width / 2f;
                var left = -right;
                var top = camSize / 2f;
                var bottom = -top;

                var view = Matrix4x4.CreateLookAt(new(position.X, position.Y, 1), new(position.X, position.Y, -1),
                    Vector3.UnitY) * Matrix4x4.CreateRotationZ(DegreesToRadians(0));
                var projection = Matrix4x4.CreateOrthographicOffCenter(left, right, bottom, top, -1, 1);
                _gl.UseProgram(_shader.Handle);
                _vao.Bind();
                while (_renderQueue.TryDequeue(out var x, out _))
                {
                    var (sprite, pos, size, rot) = x;
                    _vbo.Upload(Vertices(sprite.Uv));
                    _gl.Uniform1(_uTextureLocation, 0);
                    _gl.BindTexture(TextureTarget.Texture2D, sprite.Texture.Handle);
                    _gl.ActiveTexture(TextureUnit.Texture0);

                    SetMatrix(_uViewLocation, view);
                    SetMatrix(_uProjectionLocation, projection);

                    var model = Matrix4x4.Identity * Matrix4x4.CreateScale(size.X, size.Y, 1) *
                                Matrix4x4.CreateRotationZ(DegreesToRadians(rot)) *
                                Matrix4x4.CreateTranslation(pos.X, pos.Y, 0);
                    SetMatrix(_uModelLocation, model);

                    _gl.DrawElements(PrimitiveType.Triangles, (uint)Indices.Length, DrawElementsType.UnsignedInt, null);

                    void SetMatrix(int location, Matrix4x4 value) =>
                        _gl.UniformMatrix4(location, 1, false, (float*)&value);
                }
                _vao.UnBind();
            }
        });
    }

    public void Dispose() {
        _vao.Dispose();
        _vbo.Dispose();
        _ebo.Dispose();
        _resources.Unload<Shader>("shader.default-sprite");
    }
}