using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public interface IImage2f
    {
        Range1hf RangeZ { get; }

        Height1f Sample(Vector2i point);
    }
}