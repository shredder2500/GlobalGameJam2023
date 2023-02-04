using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace GameJam.Engine.Resources; 

internal class ResourceManager : IResourceManager
{
  private readonly IServiceProvider _services;

  public ResourceManager(IServiceProvider services)
  {
    _services = services;
  }

  public T? Load<T>(string resourceName) {
    var assembly = AppDomain.CurrentDomain.GetAssemblies()
      .FirstOrDefault(x => x.GetManifestResourceNames().Contains(resourceName));

    if (assembly == null)
      throw new InvalidOperationException($"Resource does not Exist {resourceName}");
    
    var cache = _services.GetRequiredService<IResourceCache<T>>();

    if (cache.TryGet(resourceName, out var resource)) return resource;

    var loader = _services.GetRequiredService<IResourceLoader<T>>();
    resource = loader.Load(assembly.GetManifestResourceStream(resourceName)!);
    cache.Add(resourceName, resource);
        
    return resource;
  }

  public void Unload<T>(string resourceName)
  {
    var cache = _services.GetRequiredService<IResourceCache<T>>();
    var loader = _services.GetRequiredService<IResourceLoader<T>>();

    if (cache.TryGet(resourceName, out var resource))
    {
      cache.Remove(resourceName);
      loader.Unload(resource!);
    }
  }
}