using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using TaffyScript.MonoGame.Collisions;

namespace TaffyScript.MonoGame
{
    public class SpatialHash
    {
        private int _cellSize;
        private float _inverseCellSize;
        private Box _box = new Box(0, 0);
        private SpatialStore _hash = new SpatialStore();
        private HashSet<GameObject> _cache = new HashSet<GameObject>();

        public SpatialHash(int cellSize)
        {
            _cellSize = cellSize;
            _inverseCellSize = 1f / cellSize;
        }

        public void Add(GameObject item)
        {
            foreach (var set in GetOverlappingSets(item.Collider.BoundingBox))
            {
                set.Add(item);
            }
        }

        public void Clear()
        {
            _hash.Clear();
        }

        public bool Contains(GameObject item)
        {
            foreach (var set in GetOverlappingSets(item.Collider.BoundingBox))
            {
                if (set.Contains(item))
                    return true;
            }

            return false;
        }

        public HashSet<GameObject> GetAllObjects()
        {
            return _hash.GetAllObjects();
        }

        public IEnumerator<GameObject> GetEnumerator()
        {
            foreach (var item in _hash.GetAllObjects())
                yield return item;
        }

        public void Remove(GameObject item)
        {
            foreach (var set in GetOverlappingSets(item.Collider.BoundingBox))
                set.Remove(item);
        }

        public void RemoveWithBruteForce(GameObject item)
        {
            _hash.Remove(item);
        }

        public void PrepForMove(GameObject item, RectangleF bounds)
        {
            foreach (var set in GetOverlappingSets(bounds))
                set.Remove(item);
        }

        public void Move(GameObject item)
        {
            Add(item);
        }

        public bool Any(ref RectangleF bounds)
        {
            _box.Size = bounds.Size;
            foreach (var set in GetOverlappingSets(bounds))
                foreach (var item in set)
                    if (_box.Overlaps(item.Collider))
                        return true;

            return false;
        }

        public bool Any(ref RectangleF bounds, GameObject excludeMe)
        {
            _box.Size = bounds.Size;
            foreach (var set in GetOverlappingSets(bounds))
                foreach (var item in set)
                    if (item != excludeMe && _box.Overlaps(item.Collider))
                        return true;

            return false;
        }

        public bool Any(GameObject gameObject)
        {
            foreach (var set in GetOverlappingSets(gameObject.Collider.BoundingBox))
                foreach (var item in set)
                    if (item != gameObject && gameObject.Collider.Overlaps(item.Collider))
                        return true;

            return false;
        }

        public GameObject FirstOrDefault(ref RectangleF bounds)
        {
            _box.Size = bounds.Size;
            foreach (var set in GetOverlappingSets(bounds))
                foreach (var item in set)
                    if (_box.Overlaps(item.Collider))
                        return item;

            return default(GameObject);
        }

        public GameObject FirstOrDefault(ref RectangleF bounds, GameObject excludeMe)
        {
            _box.Size = bounds.Size;
            foreach (var set in GetOverlappingSets(bounds))
                foreach (var item in set)
                    if (item != excludeMe && _box.Overlaps(item.Collider))
                        return item;

            return default(GameObject);
        }

        public HashSet<GameObject> Broadphase(ref RectangleF bounds)
        {
            var collect = new HashSet<GameObject>();
            foreach (var set in GetOverlappingSets(bounds))
                collect.UnionWith(set);

            return collect;
        }

        public HashSet<GameObject> Broadphase(ref RectangleF bounds, GameObject excludeMe)
        {
            var collect = new HashSet<GameObject>();
            foreach (var set in GetOverlappingSets(bounds))
                collect.UnionWith(set);

            collect.Remove(excludeMe);

            return collect;
        }

        public HashSet<GameObject> Broadphase(GameObject gameObject)
        {
            _cache.Clear();
            foreach (var set in GetOverlappingSets(gameObject.Collider.BoundingBox))
                _cache.UnionWith(set);

            _cache.Remove(gameObject);
            return _cache;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IEnumerable<HashSet<GameObject>> GetOverlappingSets(RectangleF box)
        {
            var minX = MathF.FloorToInt(box.Left * _inverseCellSize);
            var minY = MathF.FloorToInt(box.Top * _inverseCellSize);
            var maxX = MathF.FloorToInt(box.Right * _inverseCellSize) + 1;
            var maxY = MathF.FloorToInt(box.Bottom * _inverseCellSize) + 1;

            for (var w = minX; w < maxX; ++w)
            {
                for (var h = minY; h < maxY; ++h)
                {
                    if (!_hash.TryGetValue(w, h, out var set))
                    {
                        set = new HashSet<GameObject>();
                        _hash.Add(w, h, set);
                    }
                    yield return set;
                }
            }
        }
    }

    internal class SpatialStore
    {
        private Dictionary<long, HashSet<GameObject>> _wrapper = new Dictionary<long, HashSet<GameObject>>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private long GetKey(int x, int y)
        {
            return (long)x << 32 | (uint)y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(int x, int y, HashSet<GameObject> set)
        {
            _wrapper.Add(GetKey(x, y), set);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove(GameObject entity)
        {
            foreach (var set in _wrapper.Values)
            {
                if (set.Contains(entity))
                    set.Remove(entity);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetValue(int x, int y, out HashSet<GameObject> set)
        {
            return _wrapper.TryGetValue(GetKey(x, y), out set);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public HashSet<GameObject> GetAllObjects()
        {
            var all = new HashSet<GameObject>();

            foreach (var set in _wrapper.Values)
                all.UnionWith(set);

            return all;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            _wrapper.Clear();
        }
    }
}
