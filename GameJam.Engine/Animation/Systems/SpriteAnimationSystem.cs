using GameJam.Engine.Animation.Components;

namespace GameJam.Engine.Animation.Systems;

public class SpriteAnimationSystem : ISystem
{
    private readonly IWorld _world;
    private readonly IGameTime _time;

    public SpriteAnimationSystem(IWorld world, IGameTime time)
    {
        _world = world;
        _time = time;
    }

    public async ValueTask Execute(CancellationToken cancellationToken)
    {
        var result = _world.GetEntityBuckets()
            .Where(x => x.HasComponent<Components.Animation>())
            .Select(x => x.GetIndices().Select(i => (
                x.GetEntity(i),
                x.GetComponent<Components.Animation>(i),
                x.GetComponent<AnimationSpeed>(i) ?? new AnimationSpeed(.25),
                x.GetComponent<AnimationIdx>(i) ?? new AnimationIdx(x.GetComponent<Components.Animation>(i)!.Value.StartIdx),
                x.GetComponent<AnimationTime>(i) ?? new AnimationTime(0),
                x.HasComponent<LoopAnimation>()
            ))).SelectMany(x => x);

        await Parallel.ForEachAsync(result, cancellationToken, (x, _) =>
        {
            var (entity, animation, speed, idx, time, loop) = x;
            
            if (time.Value < speed.Value)
                _world.SetComponent(entity, new AnimationTime(time.Value += _time.DeltaTime));
            else
            {
                _world.SetComponent(entity, new AnimationTime(Math.Min(.1, time.Value - speed.Value)));

                if (animation!.Value.EndIdx == idx.Value && loop)
                {
                    _world.SetComponent(entity, new AnimationIdx(animation!.Value.StartIdx));
                }
                else if (animation!.Value.EndIdx != idx.Value)
                {
                    _world.SetComponent(entity, new AnimationIdx(idx.Value + 1));
                }
            }
            
            _world.SetComponent(entity, animation!.Value.Sheet.GetSprite(idx.Value));
            
            return ValueTask.CompletedTask;
        });
    }
}