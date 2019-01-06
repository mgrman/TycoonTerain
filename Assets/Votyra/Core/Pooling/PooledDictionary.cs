using System;
using System.Collections.Generic;
using Votyra.Core.Models.ObjectPool;

namespace Votyra.Core.Pooling
{
    public class PooledDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IReadOnlyPooledDictionary<TKey, TValue>
    {
        private static readonly bool IsDisposable = typeof(IDisposable).IsAssignableFrom(typeof(TValue));

        private static readonly ConcurentObjectPool<PooledDictionary<TKey, TValue>> Pool = new ConcurentObjectPool<PooledDictionary<TKey, TValue>>(5, () => new PooledDictionary<TKey, TValue>());

        private PooledDictionary()
        {
        }

        public void Dispose()
        {
            if (IsDisposable)
                foreach (var item in Values)
                {
                    (item as IDisposable)?.Dispose();
                }

            Pool.ReturnObject(this);
        }

        public static PooledDictionary<TKey, TValue> Create()
        {
            var obj = Pool.GetObject();
            obj.Clear();
            return obj;
        }
    }
}