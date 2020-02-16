using System;
using System.Collections.Generic;

namespace Votyra.Core
{
    public interface ITerrainRepository<TKey, TValue> where TKey : struct
    {
        event Action<RepositoryChange<TKey, TValue>> TerrainChange;
        void Add(TKey key, TValue value);
        void Remove(TKey key);
        void Unlock(TKey key);
        TValue TryGetValue(TKey key);
        void Select<TResult>(Func<TKey, TValue, TResult> func,List<TResult> cache);
        bool Contains(TKey key);
        Func<TKey, bool> ContainsKeyFunc { get; }
        bool Lock(TKey key);
    }
}