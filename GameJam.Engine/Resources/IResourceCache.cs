namespace GameJam.Engine.Resources; 

internal interface IResourceCache<T> {
  public bool TryGet(string name, out T? resource);
  public void Add(string name, T value);
  public void Remove(string name);
}