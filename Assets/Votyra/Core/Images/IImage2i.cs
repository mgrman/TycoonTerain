using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public interface IImage2i
    {
        Range1i RangeZ { get; }

        int? Sample(Vector2i point);

        bool AnyData(Range2i range);
    }
}