namespace GameJam.Engine;

public readonly record struct Entity(int Id)
{
    public static implicit operator int(Entity entity) => entity.Id;
    public static implicit operator Entity(int id) => new(id);
}