namespace GameJam.Engine.Resources; 

public interface IResourceManager {
  public T? Load<T>(string resourceName);
  public void Unload<T>(string resourceName);
}