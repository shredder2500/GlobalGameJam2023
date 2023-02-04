using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace GameJam.Engine.Resources.Loaders; 

public class TextureLoader : IResourceLoader<Texture> {
  private readonly GL _gl;

  public TextureLoader(IWindow window) {
    _gl = GL.GetApi(window);
  }

  public unsafe Texture Load(Stream stream) {
    var config = Configuration.Default.Clone();
    config.PreferContiguousImageBuffers = true;
    var img = Image.Load<Rgba32>(stream);

    var handle = _gl.GenTexture();
    _gl.ActiveTexture(TextureUnit.Texture0);
    _gl.BindTexture(TextureTarget.Texture2D, handle);
    
    if (!img.DangerousTryGetSinglePixelMemory(out Memory<Rgba32> memory))
    {
      throw new ($"Image to large.");
    }
    
    using var pinHandle = memory.Pin();

    _gl.TexImage2D(TextureTarget.Texture2D, 0, (int) InternalFormat.Rgba, (uint)img.Width, (uint)img.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pinHandle.Pointer);
    _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) GLEnum.ClampToEdge);
    _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) GLEnum.ClampToEdge);
    _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) GLEnum.Nearest);
    _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) GLEnum.Nearest);

    return new(handle);
  }

  public void Unload(Texture resource) {
    _gl.DeleteTexture(resource.Handle);
  }
}