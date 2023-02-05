using GameJam.Engine.Components;
using GameJam.Engine.Rendering.Components;
using Silk.NET.Maths;

namespace GameJam.Engine.Rendering.Systems;

internal class Render2DSystem : ISystem
{
    private readonly IWorld _world;
    private readonly IRenderQueue _renderQueue;

    public Render2DSystem(IWorld world, IRenderQueue renderQueue)
    {
        _world = world;
        _renderQueue = renderQueue;
    }

    public GamePhase Phase => GamePhase.Presentation;

    public async ValueTask Execute(CancellationToken cancellationToken)
    {
        await QueueSpritesForRender(cancellationToken);
        await QueueTextForRender(cancellationToken);

        var bucket = _world.GetEntityBuckets()
            .FirstOrDefault(x => x.HasComponent<Camera>());

        var camera = bucket != null ? bucket.GetComponent<Camera>(0) ?? new(5) : new(5);
        Vector2D<int> position = bucket != null
            ? bucket.GetComponent<Position>(0) ?? new Vector2D<int>(0, 0)
            : new Vector2D<int>(0, 0);
        _renderQueue.Render(camera, position);
    }

    private async Task QueueTextForRender(CancellationToken cancellationToken)
    {
        var result = _world.GetEntityBuckets()
            .Where(x => x.HasComponent<Position>() &&
                        x.HasComponent<Text>())
            .Select(x => x.GetIndices().Select(i => (
                x.GetComponent<Text>(i)!.Value,
                x.GetComponent<Position>(i),
                x.GetComponent<Size>(i) ?? new Vector2D<int>(16, 16),
                x.GetComponent<Rotation>(i) ?? 0,
                x.GetComponent<SpriteLayer>(i) ?? new(0, 0),
                x.GetComponent<Pivot>(i) ?? new(new(.5f, .5f)))))
            .SelectMany(x => x);

        await Parallel.ForEachAsync(result, cancellationToken, (x, _) =>
        {
            var (text, pos, size, rot, layer, pivot) = x;

            _renderQueue.Enqueue(text, layer, pos!.Value, size, rot, pivot);

            return ValueTask.CompletedTask;
        });
    }

    private async Task QueueSpritesForRender(CancellationToken cancellationToken)
    {
        var result = _world.GetEntityBuckets()
            .Where(x => x.HasComponent<Position>() &&
                        x.HasComponent<Sprite>())
            .Select(x => x.GetIndices().Select(i => (
                x.GetComponent<Sprite>(i),
                x.GetComponent<Position>(i),
                x.GetComponent<Size>(i) ?? new Vector2D<int>(16, 16),
                x.GetComponent<Rotation>(i) ?? 0,
                x.GetComponent<SpriteLayer>(i) ?? new(0, 0),
                x.GetComponent<Pivot>(i) ?? new(new(.5f, .5f)))))
            .SelectMany(x => x);

        await Parallel.ForEachAsync(result, cancellationToken, (x, _) =>
        {
            var (sprite, pos, size, rot, layer, pivot) = x;

            _renderQueue.Enqueue(sprite!.Value, layer, pos!.Value, size, rot, pivot);

            return ValueTask.CompletedTask;
        });
    }
}