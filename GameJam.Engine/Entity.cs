namespace GameJam.Engine;

public readonly record struct Entity(uint Id)
{
    public static implicit operator uint(Entity entity) => entity.Id;
    public static implicit operator Entity(uint id) => new(id);
}