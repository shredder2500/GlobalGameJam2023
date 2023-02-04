namespace GameJam.Engine;

internal class SetComponentAction<T> : IEntityAction where T : unmanaged
{
    private readonly EntityRecord _entityRecord;
    private readonly T _value;
    private readonly IList<IEntityBucket> _entityBuckets;
    private readonly IDictionary<Entity, EntityRecord> _entityRecords;

    public SetComponentAction(EntityRecord entityRecord, T value, IList<IEntityBucket> entityBuckets, IDictionary<Entity, EntityRecord> entityRecords)
    {
        _entityRecord = entityRecord;
        _value = value;
        _entityBuckets = entityBuckets;
        _entityRecords = entityRecords;
    }

    public void Execute()
    {
        // Entity is not yet part of a bucket and need to find one or create one for it
        // future optimization: convert Types to bit flag for faster searching
        if (_entityRecord.Bucket == null)
        {
            var bucket = _entityBuckets.Where(x => x.ComponentTypes.Count() == 1)
                             .FirstOrDefault(x => x.ComponentTypes.First() == typeof(T))
                         ?? NewBucket(new[] { typeof(T) });

            var idx = bucket.AddEntity(_entityRecord.Entity);
            bucket.SetComponent(idx, _value);
            _entityRecords[_entityRecord.Entity] = _entityRecord with
            {
                Bucket = bucket,
                entityIdx = idx
            };
        }
        // if the entity is already in a bucket with the component
        // then we just need to update the value
        else if (_entityRecord.Bucket.ComponentTypes.Contains(typeof(T)))
        {
            _entityRecord.Bucket.SetComponent(_entityRecord.entityIdx, _value);
        }
        // This is a new component to this entity so we need to move it from one bucket to another
        else
        {
            var archetype = _entityRecord.Bucket.ComponentTypes.Concat(new[] { typeof(T) })
                .OrderBy(x => x.Name);

            var bucket = _entityBuckets.FirstOrDefault(x => x.ComponentTypes.SequenceEqual(archetype))
                         ?? NewBucket(archetype);
            
            var newEntityIdx = _entityRecord.Bucket.MoveTo(bucket, _entityRecord.entityIdx);
            _entityRecords[_entityRecord.Entity] = _entityRecord with
            {
                Bucket = bucket,
                entityIdx = newEntityIdx
            };
        }
    }

    private IEntityBucket NewBucket(IEnumerable<Type> archetype)
    {
        var bucket = new EntityBucket(archetype.ToArray());
        _entityBuckets.Add(bucket);
        return bucket;
    }
}