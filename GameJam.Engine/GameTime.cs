using System.Diagnostics;

namespace GameJam.Engine;

public class GameTime : IGameTime
{
    public double DeltaTime { get; private set; }
    private Stopwatch _timer;

    public GameTime()
    {
        _timer = new Stopwatch();
        _timer.Start();
    }

    public void Update()
    {
        DeltaTime = _timer.Elapsed.TotalMilliseconds;
        _timer.Restart();
    }
}