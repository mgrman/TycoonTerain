using Votyra.Core.Models;

namespace Votyra.Core.TerrainMeshes
{
    public interface ITerrainMesh2f : IMesh
    {
        void Reset(Area3f area);

        void AddCell(Vector2i cellInGroup, Vector2i subCell, SampledData2f data, SampledMask2e maskData);

        void FinalizeMesh();

        float Raycast(Vector2f point);

        Vector3f Raycast(Ray3f ray);
    }
}
