using System;
using Votyra.Core.Models;
using Votyra.Core.Models.ObjectPool;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core.Pooling
{
    public class PooledTerrainMesh2iContainer<T> : IPooledTerrainMesh2i
        where T : ITerrainMesh2i, new()
    {
        public T Mesh { get; }

        public Vector2i CellInGroupCount => Mesh.CellInGroupCount;
        public int TriangleCount => Mesh.TriangleCount;

        ITerrainMesh2i IPooledTerrainMesh2i.Mesh => Mesh;
        ITerrainMesh IPooledTerrainMesh.Mesh => Mesh;

        private static readonly bool IsDisposable = typeof(IDisposable).IsAssignableFrom(typeof(T));

        private static readonly ConcurentObjectDictionaryPool<PooledTerrainMesh2iContainer<T>, Vector2i> Pool = new ConcurentObjectDictionaryPool<PooledTerrainMesh2iContainer<T>, Vector2i>(5, (CellInGroupCount) => new PooledTerrainMesh2iContainer<T>(CellInGroupCount));

        private PooledTerrainMesh2iContainer(Vector2i CellInGroupCount)
        {
            Mesh = new T();
            Mesh.Initialize(CellInGroupCount);
        }

        public static PooledTerrainMesh2iContainer<T> CreateDirty(Vector2i CellInGroupCount)
        {
            var obj = Pool.GetObject(CellInGroupCount);
            return obj;
        }

        public void Dispose()
        {
            if (IsDisposable)
            {
                (Mesh as IDisposable)?.Dispose();
            }
            Pool.ReturnObject(this, this.CellInGroupCount);
        }

        public void Clear(Range3f meshBounds)
        {
            Mesh.Clear(meshBounds);
        }

        public void AddTriangle(Vector3f a, Vector3f b, Vector3f c)
        {
            Mesh.AddTriangle(a, b, c);
        }

        public void Initialize(Vector2i CellInGroupCount)
        {
            throw new InvalidOperationException("Cannot initialize pooled mesh.");
        }
    }
}