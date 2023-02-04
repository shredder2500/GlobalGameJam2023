namespace GameJam.Engine;

public interface IGameTime
{
    public double DeltaTime { get; }

    internal void Update();
}