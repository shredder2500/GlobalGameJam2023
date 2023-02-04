namespace GameJam.Engine;

internal class MainThreadDispatcher : IMainThreadDispatcher
{
    private readonly Queue<Action> _actions = new();

    public void Enqueue(Action action)
    {
        _actions.Enqueue(action);
    }

    public void Execute()
    {
        Console.WriteLine($"Executing {_actions.Count} actions");
        while (_actions.TryDequeue(out var action))
            action();
    }
}