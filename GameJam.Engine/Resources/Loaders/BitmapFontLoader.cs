using GameJam.Engine.Rendering;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace GameJam.Engine.Resources.Loaders;

public class BitmapFontLoader : IResourceLoader<BitmapFont> {
  private readonly GL _gl;

  public BitmapFontLoader(IWindow window)
  {
    _gl = GL.GetApi(window);
  }

  public unsafe BitmapFont Load(Stream stream) {
    var width = ReadInt(2);
    var height = ReadInt(6);
    var cellWidth = ReadInt(10);
    var cellHeight = ReadInt(14);
    var bpp = ReadByte(18);
    var charOffset = ReadByte(19);
    var handle = LoadImage(ReadImageData());
    return new() {
      Width = width,
      Height = height,
      CellWidth = cellWidth,
      CellHeight = cellHeight,
      CharOffset = charOffset,
      CharWidths = GetCharWidths(),
      Handle = handle
    };

    byte[] ReadImageData() {
      stream.Seek(276, SeekOrigin.Begin);
      var buffer = new byte[width * height * bpp];
      var read = stream.Read(buffer);
      return buffer;
    }
    
    byte[] GetCharWidths() {
      stream.Seek(20, SeekOrigin.Begin);
      var buffer = new byte[256];
      var read = stream.Read(buffer);
      return buffer;
    }

    byte ReadByte(int offset) {
      stream.Seek(offset, SeekOrigin.Begin);
      return (byte)stream.ReadByte();
    }

    int ReadInt(int offset) {
      stream.Seek(offset, SeekOrigin.Begin);
      var buffer = new byte[sizeof(int)];
      var read = stream.Read(buffer);
      return BitConverter.ToInt32(buffer);
    }

    uint LoadImage(byte[] data) {
      Memory<byte> memory = data;
      var h = _gl.GenTexture();
      _gl.BindTexture(TextureTarget.Texture2D, h);

      using var pinHandle = memory.Pin();

      _gl.TexImage2D(TextureTarget.Texture2D, 0, (int) InternalFormat.Rgba, (uint)width, (uint)height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pinHandle.Pointer);
      // _gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, 256, 256, 0, PixelFormat.Rgba, GLEnum.UnsignedByte, pinHandle.Pointer);
      _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) GLEnum.ClampToEdge);
      _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) GLEnum.ClampToEdge);
      _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) GLEnum.Nearest);
      _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) GLEnum.Nearest);
      

      return h;
    }
  }

  public void Unload(BitmapFont resource) {
    _gl.DeleteTexture(resource.Handle);
  }
}