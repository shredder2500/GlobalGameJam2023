using GameJam.Components;
using GameJam.Engine.Animation.Components;
using GameJam.Engine.Components;
using GameJam.Engine.Rendering;
using GameJam.Engine.Rendering.Components;
using GameJam.Engine.Resources;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using static GameJam.Config;

namespace GameJam.Systems;

public class CanPlaceIndicatorSystem : ISystem
{
    private readonly IWorld _world;
    private readonly Sprite _canPlaceSprite;
    private readonly Sprite _cannotPlaceSprite;
    private readonly Animation _canPlaceAnimation;
    private readonly Animation _cannotPlaceAnimation;

    public CanPlaceIndicatorSystem(IWorld world, IResourceManager resources)
    {
        _world = world;
        var spriteSheet = new SpriteSheet(resources.Load<Texture>("sprite.stumpy-tileset"), StumpyTileSheetSize,
            new(PPU, PPU));

        _canPlaceSprite = spriteSheet.GetSprite(87);
        _cannotPlaceSprite = spriteSheet.GetSprite(85);
        _cannotPlaceAnimation = new Animation(spriteSheet, 85, 86);
        _canPlaceAnimation = new Animation(spriteSheet, 87, 88);
    }

    public ValueTask Execute(CancellationToken cancellationToken)
    {
        var (selector, position, wasPlaceable) = _world.GetEntityBuckets()
            .Where(x => x.HasComponent<Selector>() && x.HasComponent<Position>())
            .Select(x => x.GetIndices().Select(i => (x.GetEntity(i), x.GetComponent<Position>(i)!.Value, x.HasComponent<Placeable>())))
            .SelectMany(x => x)
            .FirstOrDefault();

        var isNotPlaceableTile = _world.GetEntityBuckets()
            .Where(x => (x.HasComponent<Root>() || x.HasComponent<Stone>()) && x.HasComponent<Position>())
            .Select(x => x.GetIndices().Select(i => x.GetComponent<Position>(i)!.Value))
            .SelectMany(x => x)
            .Any(x => x == position);

        var nextToRoot = _world
            .GetEntityBuckets()
            .Where(x => x.HasComponent<Root>() && x.HasComponent<Position>())
            .Select(x => x.GetIndices().Select(i => x.GetComponent<Position>(i)!.Value))
            .SelectMany(x => x)
            .Any(x => CheckAdjacency(x, position));

        if (wasPlaceable && (!nextToRoot || isNotPlaceableTile))
        {
            _world.RemoveComponent<Placeable>(selector);
            _world.SetComponent(selector, _cannotPlaceSprite);
            _world.SetComponent(selector, _cannotPlaceAnimation);
            _world.SetComponent(selector, new AnimationIdx(_cannotPlaceAnimation.StartIdx));
            _world.SetComponent(selector, new AnimationTime(0));
        }
        else if (!wasPlaceable && (nextToRoot && !isNotPlaceableTile))
        {
            _world.SetComponent(selector, new Placeable());
            _world.SetComponent(selector, _canPlaceSprite);
            _world.SetComponent(selector, _canPlaceAnimation);
            _world.SetComponent(selector, new AnimationIdx(_canPlaceAnimation.StartIdx));
            _world.SetComponent(selector, new AnimationTime(0));
        }
        
        return ValueTask.CompletedTask;
            
        bool CheckAdjacency(Vector2D<int> root1, Vector2D<int> root2)
        {
            var xDist = Math.Abs(root1.X - root2.X);
            var yDist = Math.Abs(root1.Y - root2.Y);
            return (xDist + yDist) == PPU;
        }
    }
}