using System.Collections.Generic;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Models;

namespace Votyra.Core.GroupSelectors
{
    public class GroupsByCameraVisibilitySelector3B : IGroupSelector3b
    {
        private readonly Vector3i _cellInGroupCount;
        private readonly IImageSampler3 _imageSampler;

        private readonly HashSet<Vector3i> _skippedAreas = new HashSet<Vector3i>();

        public GroupsByCameraVisibilitySelector3B(ITerrainConfig terrainConfig, IImageSampler3 imageSampler)
        {
            _imageSampler = imageSampler;
            _cellInGroupCount = terrainConfig.CellInGroupCount;
        }

        public GroupActions<Vector3i> GetGroupsToUpdate(IFrameData3b options)
        {
            if (options == null)
            {
                return null;
            }

            var planes = options.CameraPlanes;
            var frustumCorners = options.CameraFrustumCorners;
            var cameraPosition = options.CameraRay.Origin;
            var invalidatedArea = _imageSampler.ImageToWorld(options.InvalidatedArea_imageSpace)
                .RoundToContain();

            var localCameraBounds = Area3f.FromMinAndSize(cameraPosition, new Vector3f());
            foreach (var frustumCorner in frustumCorners)
            {
                var vector = frustumCorner;
                localCameraBounds = localCameraBounds.Encapsulate(cameraPosition + vector);
            }

            var cameraBoundsGroups = (localCameraBounds / _cellInGroupCount.ToVector3f()).RoundToContain();

            var groupsToRecompute = new List<Vector3i>();
            var groupsToKeep = new List<Vector3i>();

            var min = cameraBoundsGroups.Min;
            var max = cameraBoundsGroups.Max;
            for (var ix = min.X; ix <= max.X; ix++)
            {
                for (var iy = min.Y; iy <= max.Y; iy++)
                {
                    for (var iz = min.Z; iz <= max.Z; iz++)
                    {
                        var group = new Vector3i(ix, iy, iz);
                        var groupBounds = Area3f.FromMinAndSize((group * _cellInGroupCount).ToVector3f(), _cellInGroupCount.ToVector3f());

                        var isInside = planes.TestPlanesAABB(groupBounds);
                        if (isInside)
                        {
                            var groupArea = Range3i.FromMinAndSize(group * _cellInGroupCount, _cellInGroupCount);

                            var isInvalidated = groupArea.Overlaps(invalidatedArea);

                            if (isInvalidated)
                            {
                                groupsToRecompute.Add(group);
                                _skippedAreas.Remove(group);
                            }
                            else
                            {
                                if (!options.ExistingGroups.Contains(group))
                                {
                                    var groupBounds_image = _imageSampler.WorldToImage(groupBounds);
                                    var noData = _skippedAreas.Contains(group) || !options.Image.AnyData(groupBounds_image);
                                    if (noData)
                                    {
                                        groupsToKeep.Add(group);
                                        _skippedAreas.Add(group);
                                    }
                                    else
                                    {
                                        groupsToRecompute.Add(group);
                                    }
                                }
                                else
                                {
                                    groupsToKeep.Add(group);
                                }
                            }
                        }
                    }
                }
            }

            return new GroupActions<Vector3i>(groupsToRecompute, groupsToKeep);
        }

        private bool TestPlanesAABB(IEnumerable<Plane3f> planes, Area3f bounds)
        {
            var min = bounds.Min;
            var max = bounds.Max;

            var isInside = true;
            foreach (var plane in planes)
            {
                isInside = isInside && TestPlaneAABB(plane, min, max);
            }

            return isInside;
        }

        private bool TestPlaneAABB(Plane3f plane, Vector3f boundsMin, Vector3f boundsMax) => TestPlanePoint(plane, new Vector3f(boundsMin.X, boundsMin.Y, boundsMin.Z)) || TestPlanePoint(plane, new Vector3f(boundsMin.X, boundsMin.Y, boundsMax.Z)) || TestPlanePoint(plane, new Vector3f(boundsMin.X, boundsMax.Y, boundsMin.Z)) || TestPlanePoint(plane, new Vector3f(boundsMin.X, boundsMax.Y, boundsMax.Z)) || TestPlanePoint(plane, new Vector3f(boundsMax.X, boundsMin.Y, boundsMin.Z)) || TestPlanePoint(plane, new Vector3f(boundsMax.X, boundsMin.Y, boundsMax.Z)) || TestPlanePoint(plane, new Vector3f(boundsMax.X, boundsMax.Y, boundsMin.Z)) || TestPlanePoint(plane, new Vector3f(boundsMax.X, boundsMax.Y, boundsMax.Z));

        private bool TestPlanePoint(Plane3f plane, Vector3f point) => plane.GetDistanceToPoint(point) > 0;
    }
}
