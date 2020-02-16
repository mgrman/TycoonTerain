using System;
using System.Collections.Generic;
using System.Threading;
using Votyra.Core.GroupSelectors;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.Queueing;
using Votyra.Core.TerrainGenerators.TerrainMeshers;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core
{
    public class TerrainGeneratorManager2i : IDisposable
    {
        private readonly ITerrainRepository2i _meshRepository;

        private readonly IFrameDataProvider2i _frameDataProvider;
        private readonly int _meshTopologyDistance;

        private readonly CancellationTokenSource _onDestroyCts = new CancellationTokenSource();

        private readonly IGroupsByCameraVisibilitySelector2i _groupsByCameraVisibilitySelector2I;
        private readonly ITerrainMesh2iPool _terrainMeshPool;
        private readonly ITerrainMesher2f _terrainMesher;

        private readonly IWorkQueue<ArcResource<IFrameData2i>> _frameWorkQueue;
        private readonly IWorkQueue<GroupUpdateData> _groupWorkQueue;

        private readonly Action<Vector2i, ArcResource<IFrameData2i>> _onGroupBecameVisibleDelegate;
        private readonly Action<Vector2i> _onGroupStoppedBeingVisibleDelegate;
        private readonly List<GroupUpdateData> _updateDateCache = new List<GroupUpdateData>();
        private readonly Vector2i _cellInGroupCount;

        public TerrainGeneratorManager2i(ITerrainConfig terrainConfig,IFrameDataProvider2i frameDataProvider, IGroupsByCameraVisibilitySelector2i groupsByCameraVisibilitySelector2I, ITerrainMesh2iPool terrainMeshPool, ITerrainMesher2f terrainMesher, ITerrainRepository2i repository, IWorkQueue<ArcResource<IFrameData2i>> frameWorkQueue, IWorkQueue<GroupUpdateData> groupWorkQueue)
        {
            _cellInGroupCount = terrainConfig.CellInGroupCount.XY();
            _frameDataProvider = frameDataProvider;
            _groupsByCameraVisibilitySelector2I = groupsByCameraVisibilitySelector2I;
            _terrainMeshPool = terrainMeshPool;
            _terrainMesher = terrainMesher;
            _meshRepository = repository;
            _meshRepository.TerrainChange += MeshRepositoryOnTerrainChange;

            _onGroupBecameVisibleDelegate = OnGroupBecameVisible;
            _onGroupStoppedBeingVisibleDelegate = OnGroupStoppedBeingVisible;

            _frameWorkQueue = frameWorkQueue;
            _frameWorkQueue.DoWork += EnqueueTerrainUpdates;
            _groupWorkQueue = groupWorkQueue;
            _groupWorkQueue.DoWork += UpdateGroup;

            _frameDataProvider.FrameData += _frameWorkQueue.QueueNew;
        }

        private void MeshRepositoryOnTerrainChange(RepositoryChange<Vector2i, ITerrainMesh2f> obj)
        {
            switch (obj.Action)
            {
                case RepositorActionType.Removed:
                    _terrainMeshPool.ReturnRaw(obj.Mesh);
                    break;
            }
        }

        public void Dispose()
        {
            _onDestroyCts.Cancel();
            _frameWorkQueue.DoWork -= EnqueueTerrainUpdates;
            _groupWorkQueue.DoWork -= UpdateGroup;

            _frameDataProvider.FrameData -= _frameWorkQueue.QueueNew;
        }

        private void EnqueueTerrainUpdates(ArcResource<IFrameData2i> context)
        {
            _groupsByCameraVisibilitySelector2I.UpdateGroupsVisibility(context, _meshRepository.ContainsKeyFunc, _onGroupBecameVisibleDelegate, _onGroupStoppedBeingVisibleDelegate);

            _meshRepository.Select((group, mesh) => new GroupUpdateData(group, context, mesh, false), _updateDateCache);
            foreach (var activeGroup in _updateDateCache)
            {
                context.Activate();
                _groupWorkQueue.QueueNew(activeGroup);
            }
        }

        private void OnGroupBecameVisible(Vector2i group, ArcResource<IFrameData2i> data)
        {
            var terrainMesh = _terrainMeshPool.GetRaw();
            _meshRepository.Add(group, terrainMesh);
            _groupWorkQueue.QueueNew(new GroupUpdateData(group, data, terrainMesh, true));
        }

        private void UpdateGroup(GroupUpdateData data)
        {
            var context = data.Context;
            bool locked = false;
            try
            {
                // TODO problem here, since by the time the method finishes the group can go out of sight.
                // maybe need to be able to "lock" the group while updating, but also the terrainMesh should not be returned to pool until this method finishes.
                var terrainMesh = data.Mesh;
                var forceUpdate = data.ForceUpdate;
                var rangeXY = Range2i.FromMinAndSize(data.Group * _cellInGroupCount, _cellInGroupCount);
                if (!forceUpdate && !context.Value.InvalidatedArea.Overlaps(rangeXY))
                {
                    return;
                }

                locked = _meshRepository.Lock(data.Group);
                if (!locked)
                {
                    return;
                }

                _terrainMesher.GetResultingMesh(terrainMesh, data.Group, context.Value.Image, context.Value.Mask);
                terrainMesh.FinalizeMesh();
            }
            finally
            {
                context.Dispose();
                if (locked)
                {
                    _meshRepository.Unlock(data.Group);
                }
            }
        }

        private void OnGroupStoppedBeingVisible(Vector2i group)
        {
            _meshRepository.Remove(group);
        }
    }
}