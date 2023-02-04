namespace GameJam.Engine;

internal class DestroyEntityAction : IEntityAction
{
    private readonly EntityRecord _entityRecord;
    private readonly IDictionary<Entity, EntityRecord> _entityMap;

    public DestroyEntityAction(EntityRecord entityRecord, IDictionary<Entity, EntityRecord> entityMap)
    {
        _entityRecord = entityRecord;
        _entityMap = entityMap;
    }

    public void Execute()
    {
        _entityMap.Remove(_entityRecord.Entity);
        _entityRecord.Bucket?.RemoveEntity(_entityRecord.entityIdx);
    }
}