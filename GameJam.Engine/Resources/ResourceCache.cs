namespace GameJam.Engine.Resources; 

internal class ResourceCache<T> : IResourceCache<T>, IDisposable
{
  private readonly Dictionary<string, T> _cache = new();
  private readonly IResourceLoader<T> _loader;

  public ResourceCache(IResourceLoader<T> loader)
  {
    _loader = loader;
  }

  public bool TryGet(string name, out T? resource) => _cache.TryGetValue(name, out resource);

  public void Add(string name, T value) => _cache.Add(name, value);

  public void Remove(string name) => _cache.Remove(name);

  public void Dispose()
  {
    foreach (var value in _cache.Values) {
      _loader.Unload(value);
    }
    _cache.Clear();
  }
}