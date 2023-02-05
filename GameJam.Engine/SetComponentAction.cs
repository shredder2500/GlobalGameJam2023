namespace GameJam.Engine;

internal class SetComponentAction<T> : IEntityAction where T : unmanaged
{
    private readonly Entity _entity;
    private readonly T _value;
    private readonly IList<IEntityBucket> _entityBuckets;
    private readonly IDictionary<Entity, EntityRecord> _entityRecords;

    public SetComponentAction(Entity entity, T value, IList<IEntityBucket> entityBuckets, IDictionary<Entity, EntityRecord> entityRecords)
    {
        _entity = entity;
        _value = value;
        _entityBuckets = entityBuckets;
        _entityRecords = entityRecords;
    }

    public void Execute()
    {
        var entityRecord = _entityRecords[_entity];
        // Entity is not yet part of a bucket and need to find one or create one for it
        // future optimization: convert Types to bit flag for faster searching
        if (entityRecord.Bucket == null)
        {
            var bucket = _entityBuckets.Where(x => x.ComponentTypes.Count() == 1)
                             .FirstOrDefault(x => x.ComponentTypes.First() == typeof(T))
                         ?? NewBucket(new[] { typeof(T) });

            var idx = bucket.AddEntity(entityRecord.Entity);
            bucket.SetComponent(idx, _value);
            _entityRecords[entityRecord.Entity] = entityRecord with
            {
                Bucket = bucket,
                entityIdx = idx
            };
        }
        // if the entity is already in a bucket with the component
        // then we just need to update the value
        else if (entityRecord.Bucket.ComponentTypes.Contains(typeof(T)))
        {
            entityRecord.Bucket.SetComponent(entityRecord.entityIdx, _value);
        }
        // This is a new component to this entity so we need to move it from one bucket to another
        else
        {
            var archetype = entityRecord.Bucket.ComponentTypes.Concat(new[] { typeof(T) })
                .OrderBy(x => x.Name);

            var bucket = _entityBuckets.FirstOrDefault(x => x.ComponentTypes.SequenceEqual(archetype))
                         ?? NewBucket(archetype);
            
            var newEntityIdx = entityRecord.Bucket.MoveTo(bucket, entityRecord.entityIdx);
            bucket.SetComponent(newEntityIdx, _value);
            _entityRecords[entityRecord.Entity] = entityRecord with
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