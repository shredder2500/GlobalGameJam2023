namespace GameJam.Engine;

public interface IWorld
{
    public Entity CreateEntity();
    public IEnumerable<IEntityBucket> GetEntityBuckets();
    public void SetComponent<T>(Entity entity, T value) where T : unmanaged;
    public T? GetComponent<T>(Entity entity) where T : unmanaged;
    public void RemoveComponent<T>(Entity entity, T value) where T : unmanaged;
    public void DestroyEntity(Entity entity);
    internal void Sync();
}