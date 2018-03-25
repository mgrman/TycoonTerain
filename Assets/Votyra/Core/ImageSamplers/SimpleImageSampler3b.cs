using Votyra.Core.Images;
using Votyra.Core.Models;

namespace Votyra.Core.ImageSamplers
{
    public class SimpleImageSampler3b : IImageSampler3
    {
        public Vector3i WorldToImage(Vector3f pos)
        {
            return pos.RoundToVector3i();
        }

        public Vector3f ImageToWorld(Vector3i pos)
        {
            return pos.ToVector3f();
        }

        public Vector3i CellToX0Y0Z0(Vector3i pos)
        {
            return new Vector3i(pos.x + 0, pos.y + 0, pos.z + 0);
        }

        public Vector3i CellToX0Y0Z1(Vector3i pos)
        {
            return new Vector3i(pos.x + 0, pos.y + 0, pos.z + 1);
        }

        public Vector3i CellToX0Y1Z0(Vector3i pos)
        {
            return new Vector3i(pos.x + 0, pos.y + 1, pos.z + 0);
        }

        public Vector3i CellToX0Y1Z1(Vector3i pos)
        {
            return new Vector3i(pos.x + 0, pos.y + 1, pos.z + 1);
        }

        public Vector3i CellToX1Y0Z0(Vector3i pos)
        {
            return new Vector3i(pos.x + 1, pos.y + 0, pos.z + 0);
        }

        public Vector3i CellToX1Y0Z1(Vector3i pos)
        {
            return new Vector3i(pos.x + 1, pos.y + 0, pos.z + 1);
        }

        public Vector3i CellToX1Y1Z0(Vector3i pos)
        {
            return new Vector3i(pos.x + 1, pos.y + 1, pos.z + 0);
        }

        public Vector3i CellToX1Y1Z1(Vector3i pos)
        {
            return new Vector3i(pos.x + 1, pos.y + 1, pos.z + 1);
        }
    }
}