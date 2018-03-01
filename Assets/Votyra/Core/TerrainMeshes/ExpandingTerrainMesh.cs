using System.Collections.Generic;
using Votyra.Core.Models;

namespace Votyra.Core.TerrainMeshes
{
    public class ExpandingTerrainMesh : ITerrainMesh
    {
        public Rect3f MeshBounds { get; private set; }
        public List<Vector3f> Vertices { get; }
        public List<Vector3f> Normals { get; }
        public List<Vector2f> UV { get; }
        public List<int> Indices { get; }

        public int TriangleCount { get; private set; }
        public int PointCount { get; private set; }

        public ExpandingTerrainMesh()
        {
            Vertices = new List<Vector3f>();
            UV = new List<Vector2f>();
            Indices = new List<int>();
            Normals = new List<Vector3f>();
        }

        public void Clear(Rect3f meshBounds)
        {
            MeshBounds = meshBounds;
            TriangleCount = 0;
            PointCount = 0;
            Vertices.Clear();
            UV.Clear();
            Indices.Clear();
            Normals.Clear();
        }

        public void AddTriangle(Vector3f posA, Vector3f posB, Vector3f posC)
        {
            var side1 = posB - posA;
            var side2 = posC - posA;
            var normal = Vector3f.Cross(side1, side2).normalized;

            Indices.Add(PointCount);
            Vertices.Add(posA);
            UV.Add(new Vector2f(posA.x, posA.y));
            Normals.Add(normal);
            PointCount++;

            Indices.Add(PointCount);
            Vertices.Add(posB);
            UV.Add(new Vector2f(posB.x, posB.y));
            Normals.Add(normal);
            PointCount++;

            Indices.Add(PointCount);
            Vertices.Add(posC);
            UV.Add(new Vector2f(posC.x, posC.y));
            Normals.Add(normal);
            PointCount++;

            TriangleCount++;
        }
    }
}