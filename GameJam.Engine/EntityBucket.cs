using System.Buffers;
using System.Runtime.InteropServices;

namespace GameJam.Engine;

internal unsafe class EntityBucket : IEntityBucket, IDisposable
{
    public const int InitSize = 4;
    private readonly Type[] _componentTypes;
    private readonly nuint[] _componentSizes;
    private readonly LinkedList<int> _indices;
    private readonly PriorityQueue<int, int> _nextIdx;
    private Entity[] _entities;
    private void*[] _components;

    public IEnumerable<Type> ComponentTypes => _componentTypes;

    public EntityBucket(Type[] componentTypes)
    {
        _componentTypes = componentTypes;
        _indices = new();
        _entities = new Entity[InitSize];
        _components = new void*[componentTypes.Length];
        _componentSizes = new nuint[componentTypes.Length];
        _nextIdx = new();

        for (var i = 0; i < componentTypes.Length; i++)
        {
            var componentSize = (nuint)Marshal.SizeOf(componentTypes[i]);
            _componentSizes[i] = componentSize;
            _components[i] = NativeMemory.Alloc(InitSize, componentSize);
        }
        
        _nextIdx.EnqueueRange(Enumerable.Range(0, InitSize).Select(i => (i, i)));
    }

    public bool HasComponent<T>() where T : unmanaged =>
        _componentTypes.Contains(typeof(T));

    public IEnumerable<int> GetIndices() => _indices;

    public T GetComponent<T>(int entityIdx) where T : unmanaged
    {
        var typeIdx = GetTypeIdx<T>();
        var ptr = (T*)_components[typeIdx];

        return ptr[entityIdx];
    }

    public Entity GetEntity(int entityIdx) => _entities[entityIdx];

    public int AddEntity(Entity entity)
    {
        var idx = _nextIdx.Dequeue();
        _entities[idx] = entity;
        _indices.AddLast(idx);
        
        EnsureCapacity();

        return idx;
    }

    public void SetComponent<T>(int entityIdx, T value) where T : unmanaged
    {
        var typeIdx = GetTypeIdx<T>();
        var ptr = (T*)_components[typeIdx];
        ptr[entityIdx] = value;
    }

    public int MoveTo(IEntityBucket otherBucket, int entityIdx)
    {
        var targetIdx = otherBucket.AddEntity(GetEntity(entityIdx));

        for (var i = 0; i < _componentTypes.Length; i++)
        {
            if (!otherBucket.ComponentTypes.Contains(_componentTypes[i])) continue;
            
            var ptr = (byte*)_components[i] + _componentSizes[i] * (nuint)entityIdx;
            CopyComponent(ptr, _componentTypes[i], targetIdx);
        }
        
        RemoveEntity(entityIdx);

        return targetIdx;
    }

    public void CopyComponent(void* componentPtr, Type componentType, int entityIdx)
    {
        var typeIdx = GetTypeIdx(componentType);
        var typeSize = _componentSizes[typeIdx];
        var targetPtr = (byte*)_components[typeIdx] + typeSize * (nuint)entityIdx;
        
        NativeMemory.Copy(componentPtr, targetPtr, typeSize);
    }

    public void RemoveEntity(Entity entity)
    {
        var idx = Array.IndexOf(_entities, entity);
        RemoveEntity(idx);
    }

    public void RemoveEntity(int idx)
    {
        _indices.Remove(idx);
        _nextIdx.Enqueue(idx, idx);
    }

    // Future optimization: figure out how to cache this instead of looking it up every time
    // Current thought is I would offload tracking the type idx to some querying logic. but that is out of scope for the jam
    // for the jam the I am planning to query using Linq
    private int GetTypeIdx<T>() => GetTypeIdx(typeof(T));

    public int GetTypeIdx(Type type)
    {
        var idx = Array.IndexOf(_componentTypes, type);
        if (idx == -1) throw new InvalidOperationException($"Bucket does not contain {type}");
        return idx;
    }

    // Future optimization: Use array pool for Entities array and create a pool to reuse component Pointers to reduce allocations
    private void EnsureCapacity()
    {
        if (_indices.Count < _entities.Length) return;

        var targetSize = _entities.Length * 2;
        var currSize = _entities.Length;
        var newEntityArray = new Entity[targetSize];
        Array.Copy(_entities, newEntityArray, currSize);
        _entities = newEntityArray;

        for (var i = 0; i < _componentTypes.Length; i++)
        {
            _components[i] = NativeMemory.Realloc(_components[i], _componentSizes[i] * (nuint)targetSize);
        }
        
        _nextIdx.EnqueueRange(Enumerable.Range(currSize, targetSize - currSize).Select(i => (i, i)));
    }

    public void Dispose()
    {
        for (var i = 0; i < _componentTypes.Length; i++)
        {
            NativeMemory.Free(_components[i]);
        }
    }
}