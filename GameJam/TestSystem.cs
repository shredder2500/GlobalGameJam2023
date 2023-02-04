namespace GameJam;

public class TestSystem : ISystem
{
    public ValueTask Execute(CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }
}