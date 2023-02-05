namespace GameJam.Engine;

internal class World : IWorld
{
    private uint _nextId;
    private readonly List<IEntityBucket> _entityBuckets = new();
    private readonly Dictionary<Entity, EntityRecord> _entityBucketMap = new();
    private readonly PriorityQueue<IEntityAction, int> _actionQueue = new();

    public Entity CreateEntity()
    {
        lock(_entityBucketMap){
            Entity newEntity = _nextId++;
            _entityBucketMap.Add(newEntity, new(newEntity, null, 0));
            return newEntity;
        }
    }

    public IEnumerable<IEntityBucket> GetEntityBuckets() => _entityBuckets;

    public void SetComponent<T>(Entity entity, T value) where T : unmanaged
    {
        lock (_actionQueue)
        {
            _actionQueue.Enqueue(new SetComponentAction<T>(entity, value, _entityBuckets, _entityBucketMap), 1);
        }
    }

    public void RemoveComponent<T>(Entity entity) where T : unmanaged
    {
        lock (_actionQueue)
        {
            _actionQueue.Enqueue(new RemoveComponentAction<T>(entity, _entityBuckets, _entityBucketMap), 2);
        }
    }

    public void DestroyEntity(Entity entity)
    {
        lock (_actionQueue)
        {
            var record = _entityBucketMap[entity];
            _actionQueue.Enqueue(new DestroyEntityAction(record, _entityBucketMap), 3);
        }
    }

    public T? GetComponent<T>(Entity entity) where T : unmanaged
    {
        var record = _entityBucketMap[entity];
        return record.Bucket?.GetComponent<T>(record.entityIdx);
    }

    public void Sync()
    {
        while (_actionQueue.TryDequeue(out var action, out _))
        {
            action.Execute();
        }

        _entityBuckets.RemoveAll(x => !x.GetIndices().Any());
    }
}