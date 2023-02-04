namespace GameJam.Engine.Resources; 

public interface IResourceLoader<T> {
  public T Load(Stream stream);
  public void Unload(T resource);
}