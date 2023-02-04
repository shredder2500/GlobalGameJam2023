namespace GameJam.Engine;

public interface IEntityBucket
{
    public IEnumerable<Type> ComponentTypes { get; }
    public bool HasComponent<T>() where T : unmanaged;
    public T? GetComponent<T>(int entityIdx) where T : unmanaged;
    public IEnumerable<int> GetIndices();
    public Entity GetEntity(int entityIdx);

    internal int AddEntity(Entity entity);
    internal void SetComponent<T>(int entityIdx, T value) where T : unmanaged;
    internal void RemoveEntity(Entity entity);
    internal unsafe void CopyComponent(void* componentPtr, Type componentType, int entityIdx);
    internal int MoveTo(IEntityBucket otherBucket, int entityIdx);
    internal void RemoveEntity(int idx);
}