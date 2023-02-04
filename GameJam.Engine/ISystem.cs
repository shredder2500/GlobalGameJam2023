namespace GameJam.Engine;

public interface ISystem
{
    public virtual GamePhase Phase => GamePhase.Update;
    
    public ValueTask Execute(CancellationToken cancellationToken);
}

public enum GamePhase
{
    Update,
    Simulation,
    Presentation
}