namespace GameJam.Engine;

internal class RemoveComponentAction<T> : IEntityAction where T : unmanaged
{
    private readonly Entity _entity;
    private readonly IList<IEntityBucket> _entityBuckets;
    private readonly IDictionary<Entity, EntityRecord> _entityRecords;

    public RemoveComponentAction(Entity entity, IList<IEntityBucket> entityBuckets, IDictionary<Entity, EntityRecord> entityRecords)
    {
        _entity = entity;
        _entityBuckets = entityBuckets;
        _entityRecords = entityRecords;
    }

    public void Execute()
    {
        var entityRecord = _entityRecords[_entity];
        if (entityRecord.Bucket == null) return;
        
        var archetype = entityRecord.Bucket.ComponentTypes
            .Where(x => x != typeof(T))
            .OrderBy(x => x.Name)
            .ToArray();

        var bucket = _entityBuckets.FirstOrDefault(x => x.ComponentTypes.SequenceEqual(archetype))
                     ?? NewBucket(archetype);

        var newEntityIdx = entityRecord.Bucket.MoveTo(bucket, entityRecord.entityIdx);

        _entityRecords[entityRecord.Entity] = entityRecord with
        {
            Bucket = bucket,
            entityIdx = newEntityIdx
        };
    }
    
    private IEntityBucket NewBucket(IEnumerable<Type> archetype)
    {
        var bucket = new EntityBucket(archetype.ToArray());
        _entityBuckets.Add(bucket);
        return bucket;
    }
}