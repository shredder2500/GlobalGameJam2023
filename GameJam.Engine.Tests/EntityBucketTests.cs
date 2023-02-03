namespace GameJam.Engine.Tests;

[TestFixture]
public class EntityBucketTests
{
    [Test]
    public void HasComponent_WillReturnTrue_IfHasComponent()
    {
        using var bucket = new EntityBucket(new[] { typeof(TestComponentA) });
        var result = bucket.HasComponent<TestComponentA>();
        Assert.That(result, Is.True);
    }
    
    [Test]
    public void HasComponent_WillReturnFalse_IfDoesNotContainTypet()
    {
        using var bucket = new EntityBucket(new[] { typeof(TestComponentA) });
        var result = bucket.HasComponent<TestComponentB>();
        Assert.That(result, Is.False);
    }

    [Test]
    public void GetIndices_WillReturnNothing_ByDefault()
    {
        using var bucket = new EntityBucket(new[] { typeof(TestComponentA) });
        var result = bucket.GetIndices();
        
        Assert.That(result.ToArray(), Has.Length.EqualTo(0));
    }

    [Test]
    public void AddEntity_WillReturnEntityIdx()
    {
        using var bucket = new EntityBucket(new[] { typeof(TestComponentA) });
        var a = bucket.AddEntity(new (2));
        var b = bucket.AddEntity(new (4));
        Assert.Multiple(() =>
        {
            Assert.That(a, Is.EqualTo(0));
            Assert.That(b, Is.EqualTo(1));
        });
    }

    [Test]
    public void GetIndices_WillReturnIdx_AfterEntitiesAreAdded()
    {
        using var bucket = new EntityBucket(new[] { typeof(TestComponentA) });
        bucket.AddEntity(new (2));
        bucket.AddEntity(new (4));
        var result = bucket.GetIndices().ToArray();
        
        Assert.That(result, Has.Length.EqualTo(2));
        Assert.That(result, Contains.Item(0)); // first entity inserted in index 0
        Assert.That(result, Contains.Item(1)); // second entity inserted in index 1
    }

    [Test]
    public void GetComponent_WillReturn_ComponentDataThatWasAdded()
    {
        using var bucket = new EntityBucket(new[] { typeof(TestComponentA) });
        var entityIdx = bucket.AddEntity(new (2));
        bucket.SetComponent(entityIdx, new TestComponentA(5));

        var result = bucket.GetComponent<TestComponentA>(entityIdx);
        
        Assert.That(result.Value, Is.EqualTo(5));
    }

    [Test]
    public void GetEntity_WillReturnEntity_ByIdx()
    {
        using var bucket = new EntityBucket(new[] { typeof(TestComponentA) });
        bucket.AddEntity(new(3));
        var entityIdx = bucket.AddEntity(new (23));

        var result = bucket.GetEntity(entityIdx);
        
        Assert.That(result, Is.EqualTo(new Entity(23)));
    }

    [Test]
    public void GetComponent_ThrowsException_IfBucketDoesNotContainType()
    {
        using var bucket = new EntityBucket(new[] { typeof(TestComponentA) });
        var entityIdx = bucket.AddEntity(new(23));

        Assert.Throws<InvalidOperationException>(() => bucket.GetComponent<TestComponentB>(entityIdx));
    }

    [Test]
    public void CanAddMoreEntitiesThanInitSize()
    {
        using var bucket = new EntityBucket(new[] { typeof(TestComponentA) });
    
        for (var i = 0; i < EntityBucket.InitSize; i++)
        {
            bucket.AddEntity(new(i));
        }
        
        Assert.That(bucket.AddEntity(new(EntityBucket.InitSize)), Is.EqualTo(EntityBucket.InitSize));
    }

    [Test]
    public void DataIsTheSameAfterResizeOfCapacity()
    {
        using var bucket = new EntityBucket(new[] { typeof(TestComponentA) });
        var entityIdx = bucket.AddEntity(new(30));
        bucket.SetComponent(entityIdx, new TestComponentA(20));

        for (var i = 0; i < EntityBucket.InitSize; i++)
        {
            bucket.AddEntity(new(i));
        }

        Assert.Multiple(() =>
        {
            Assert.That(bucket.GetEntity(entityIdx), Is.EqualTo(new Entity(30)));
            Assert.That(bucket.GetComponent<TestComponentA>(entityIdx), Is.EqualTo(new TestComponentA(20)));
        });
    }

    public record struct TestComponentA(int Value);

    public record struct TestComponentB;
}