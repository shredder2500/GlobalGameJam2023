namespace GameJam.Engine;

public interface IWorldManager
{
    public IWorld CreateWorld();
    internal ValueTask Update(CancellationToken token);
    internal ValueTask Sim(CancellationToken token);
    internal ValueTask Render(CancellationToken token);
}