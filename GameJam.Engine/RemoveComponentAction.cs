namespace GameJam.Engine;

internal class RemoveComponentAction<T> : IEntityAction where T : unmanaged
{
    private readonly EntityRecord _entityRecord;
    private readonly IList<IEntityBucket> _entityBuckets;
    private readonly IDictionary<Entity, EntityRecord> _entityRecords;

    public RemoveComponentAction(EntityRecord entityRecord, IList<IEntityBucket> entityBuckets, IDictionary<Entity, EntityRecord> entityRecords)
    {
        _entityRecord = entityRecord;
        _entityBuckets = entityBuckets;
        _entityRecords = entityRecords;
    }

    public void Execute()
    {
        if (_entityRecord.Bucket == null) return;
        
        var archetype = _entityRecord.Bucket.ComponentTypes
            .Where(x => x != typeof(T))
            .OrderBy(x => x.Name)
            .ToArray();

        var bucket = _entityBuckets.FirstOrDefault(x => x.ComponentTypes.SequenceEqual(archetype))
                     ?? NewBucket(archetype);

        var newEntityIdx = _entityRecord.Bucket.MoveTo(bucket, _entityRecord.entityIdx);

        _entityRecords[_entityRecord.Entity] = _entityRecord with
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