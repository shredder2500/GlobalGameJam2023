using Silk.NET.OpenGL;

namespace GameJam.Engine.Rendering;

public class BufferObject<TDataType> : IDisposable
    where TDataType : unmanaged
{
    private uint _handle;
    private BufferTargetARB _bufferType;
    private GL _gl;

    public uint Handle => _handle;

    public unsafe BufferObject(GL gl, Span<TDataType> data, BufferTargetARB bufferType)
    {
        _gl = gl;
        _bufferType = bufferType;

        _handle = _gl.GenBuffer();
        Bind();
        fixed (void* d = data)
        {
            _gl.BufferData(bufferType, (nuint) (data.Length * sizeof(TDataType)), d, BufferUsageARB.StaticDraw);
        }
    }

    public unsafe void Upload(Span<TDataType> data) {
        Bind();
        fixed (void* d = data)
        {
            _gl.BufferData(_bufferType, (nuint) (data.Length * sizeof(TDataType)), d, BufferUsageARB.DynamicDraw);
        }
    }

    public void Bind()
    {
        _gl.BindBuffer(_bufferType, _handle);
    }

    public void Dispose()
    {
        _gl.DeleteBuffer(_handle);
    }
}