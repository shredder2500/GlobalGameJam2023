using Microsoft.Extensions.Logging;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace GameJam.Engine.Resources.Loaders; 

internal class ShaderLoader : IResourceLoader<Shader> {
  private readonly GL _gl;
  private readonly ILogger _logger;

  public ShaderLoader(IWindow window, ILogger<ShaderLoader> logger) {
    _gl = GL.GetApi(window);
    _logger = logger;
  }

  public Shader Load(Stream resourceStream) {
    using var reader = new StreamReader(resourceStream);
    var shaders = GetSources(reader.ReadToEnd())
      .Select(x => LoadShader(x.Key, x.Value))
      .ToArray();

    var handle = _gl.CreateProgram();
    
    AttachShaders(shaders);

    // var uniformBlockIndex = _gl.GetUniformBlockIndex(handle, "Matrices");
    // _gl.UniformBlockBinding(handle, uniformBlockIndex, 0);

    return new(handle);

    uint LoadShader(ShaderType type, string src)
    {
      var shaderHandle = _gl.CreateShader(type);
      _gl.ShaderSource(shaderHandle, src);
      _gl.CompileShader(shaderHandle);

      var infoLog = _gl.GetShaderInfoLog(shaderHandle);
      if (!string.IsNullOrWhiteSpace(infoLog))
      {
        throw new($"Error compiling shader of type {type}, failed with error {infoLog}");
      }

      return shaderHandle;
    }
    
    void AttachShaders(params uint[] handles)
    {
      foreach (var shader in handles)
      {
        _gl.AttachShader(handle, shader);
      }
      _gl.LinkProgram(handle);
      CheckProgramStatus();
      foreach (var shader in handles)
      {
        _gl.DetachShader(handle, shader);
        _gl.DeleteShader(shader);
      }
    }
    
    void CheckProgramStatus()
    {
      _gl.GetProgram(handle, GLEnum.LinkStatus, out var status);
      if (status == 0)
      {
        _logger.LogCritical("Shader failed with status {Status}", _gl.GetProgramInfoLog(handle));
        _gl.DeleteShader(handle);
        throw new($"Program failed to link with error: {_gl.GetProgramInfoLog(handle)}");
      }
    }
    
    Dictionary<ShaderType, string> GetSources(string source) {
      return source.Split("#type ")
        .Where(x => !string.IsNullOrEmpty(x))
        .Select(x => {
          var stringReader = new StringReader(x);
          var type = stringReader.ReadLine()!.Trim();
          var s = stringReader.ReadToEnd();
          return (type, source: s);
        })
        .ToDictionary(x => GetShaderType(x.type), x => x.source);

      ShaderType GetShaderType(string type) => type switch {
        "vertex" => ShaderType.VertexShader,
        "fragment" => ShaderType.FragmentShader,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
      };
    }
  }

  public void Unload(Shader resource)
  {
    _gl.DeleteProgram(resource.Handle);
  }
}