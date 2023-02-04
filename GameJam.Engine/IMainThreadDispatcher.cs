namespace GameJam.Engine;

public interface IMainThreadDispatcher
{
    public void Enqueue(Action action);
    internal void Execute();
}