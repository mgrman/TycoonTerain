﻿using System;
using System.Collections.Generic;
using System.Threading;
using Votyra.Models;
using Votyra.Utils;
using Votyra.Images;
using Votyra.ImageSamplers;
using Votyra.TerrainAlgorithms;
using Votyra.TerrainMeshGenerators;
using Votyra.TerrainTileGenerators;
using Votyra.TerrainMeshers;
using Votyra.TerrainMeshers.TriangleMesh;
using Votyra.Unity.Behaviours;
using Votyra.Unity.GroupSelectors;
using Votyra.Unity.Images;
using Votyra.Unity.Logging;
using Votyra.Unity.MeshUpdaters;
using Votyra.Unity.Models;
using Votyra.Unity.Profiling;
using Votyra.Unity.Utils;
using UnityEngine;
using UnityEngine.Profiling;
using System.Linq;
using System.Threading.Tasks;
using Votyra.Profiling;
using Votyra.Unity.Assets.Votyra.Pooling;
using Votyra.Images.EditableImages;
using Votyra.Logging;
using Votyra.TerrainTileGenerators.TerrainGroups;
using Votyra.TerrainMeshGenerators.TerrainMeshers;

namespace Votyra.Unity
{

    //TODO: move to floats
    public class TerrainGeneratorBehaviour : MonoBehaviour
    {
        public UI_Vector2i CellInGroupCount = new UI_Vector2i(10, 10);
        public bool FlipTriangles = false;
        public bool DrawBounds = false;
        public bool Async = true;
        public Material Material = null;
        public Material MaterialWalls = null;
        public Texture2D InitialTexture = null;
        public float InitialTextureScale = 1;

        public IEditableImage EditableImage { get { return _imageProvider as IEditableImage; } }

        private IImage2iProvider _imageProvider;
        private ITerrainAlgorithm _terrainAlgorithm;
        private ITerrainAlgorithm _onEditTerrainAlgorithm;
        private IImageSampler _sampler;
        private IGroupSelector _groupsSelector;
        private ITerrainMeshGenerator _terrainMeshGenerator;
        private ITerrainTileGenerator _terrainTileGenerator;
        private IMeshUpdater _meshUpdater;

        private Task _updateTask = null;
        private CancellationTokenSource _onDestroyCts = new CancellationTokenSource();

        private void Start()
        {
            this.gameObject.DestroyAllChildren();
            Initialize();
        }

        private void LateUpdate()
        {
            if (_updateTask == null || _updateTask.IsCompleted)
            {
                _updateTask?.Dispose();
                _updateTask = null;

                var context = GetSceneContext();
                _updateTask = UpdateTerrain(context, Async, _onDestroyCts.Token);
            }

            if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            {
                ProcessMouseClick();
            }
        }

        private void ProcessMouseClick()
        {
            // Debug.LogFormat("OnMouseDown on tile.");

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            // Casts the ray and get the first game object hit
            Physics.Raycast(ray, out hit);

            var position = this.transform.worldToLocalMatrix.MultiplyPoint(hit.point).XY();
            position = _sampler.WorldToImage(position);

            this.SendMessage("OnCellClick", position, SendMessageOptions.DontRequireReceiver);
        }

        private async Task UpdateTerrain(SceneContext context, bool async, CancellationToken token)
        {
            GroupActions groupActions = null;
            IReadOnlyPooledDictionary<Vector2i, ITerrainMesh> results = null;
            try
            {
                Func<IReadOnlyPooledDictionary<Vector2i, ITerrainMesh>> computeAction = () =>
                    {
                        using (this.CreateProfiler("Creating visible groups"))
                        {
                            groupActions = context.GroupSelector.GetGroupsToUpdate(context);
                        }
                        var toRecompute = groupActions?.ToRecompute ?? Enumerable.Empty<Vector2i>();
                        if (toRecompute.Any())
                        {
                            IReadOnlyPooledCollection<ITerrainGroup> tiles;
                            using (this.CreateProfiler("TerrainTileGenerator"))
                            {
                                tiles = context.TerrainTileGenerator.Generate(context, toRecompute);
                            }
                            using (this.CreateProfiler("TerrainMeshGenerator"))
                            {
                                return context.TerrainMeshGenerator.Generate(context, tiles);
                            }
                        }
                        else
                        {
                            return null;
                        }
                    };

                if (async)
                {
                    results = await Task.Run(computeAction);
                }
                else
                {
                    results = computeAction();
                }

                if (token.IsCancellationRequested)
                {
                    return;
                }

                if (results != null)
                {
                    using (this.CreateProfiler("Applying mesh"))
                    {
                        var toKeep = groupActions?.ToKeep ?? Enumerable.Empty<Vector2i>();
                        context.MeshUpdater.UpdateMesh(context, results, toKeep);
                    }
                }

            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            finally
            {
                context?.Dispose();
                groupActions?.Dispose();
                results?.Dispose();
            }
        }

        private SceneContext GetSceneContext()
        {
            var camera = Camera.main;
            var container = this.gameObject;

            var existingGroups = _meshUpdater.ExistingGroups;

            var image = _imageProvider.CreateImage();

            Vector2i cellInGroupCount = new Vector2i(this.CellInGroupCount.x, this.CellInGroupCount.y);

            var localToProjection = camera.projectionMatrix * camera.worldToCameraMatrix * this.transform.localToWorldMatrix;

            var planes = GeometryUtility.CalculateFrustumPlanes(localToProjection);

            var frustumCorners = PooledArrayContainer<Vector3>.CreateDirty(4);

            camera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), camera.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, frustumCorners.Array);

            return new SceneContext
            (
                _groupsSelector,
                _terrainMeshGenerator,
                _terrainTileGenerator,
                _meshUpdater,
                camera.transform.position,
                planes,
                frustumCorners,
                camera.transform.localToWorldMatrix,
                container.transform.worldToLocalMatrix,
                existingGroups,
                cellInGroupCount,
                image,
                _sampler,
                _terrainAlgorithm,
                () => this.GameObjectFactory()
            );
        }

        private GameObject GameObjectFactory()
        {
            var go = new GameObject();
            go.transform.SetParent(this.transform, false);
            if (DrawBounds)
            {
                go.AddComponent<DrawBounds>();
            }
            var meshRenderer = go.GetOrAddComponent<MeshRenderer>();
            meshRenderer.material = Material;
            meshRenderer.materials = new Material[] { Material, MaterialWalls };
            return go;
        }

        private void Initialize()
        {
            if (LoggerFactory.Factory == null)
                LoggerFactory.Factory = (name, owner) => new UnityLogger(name, owner);
            if (ProfilerFactory.Factory == null)
                ProfilerFactory.Factory = (name, owner) => new UnityProfiler(name, owner);

            _terrainAlgorithm = new SimpleTerrainAlgorithm();
            _onEditTerrainAlgorithm = new TileSelectTerrainAlgorithm();
            _sampler = new DualImageSampler();

            _terrainMeshGenerator = new TerrainMeshGenerator<TerrainMesher>();
            _terrainTileGenerator = new TerrainTileGenerator();
            _meshUpdater = new TerrainMeshUpdater();
            _groupsSelector = new GroupsByCameraVisibilitySelector();
            _imageProvider = new EditableMatrixImage(InitialTexture, InitialTextureScale, _sampler, _onEditTerrainAlgorithm);
        }

        private void DisposeService()
        {
            (_imageProvider as IDisposable)?.Dispose();
            _imageProvider = null;

            (_onEditTerrainAlgorithm as IDisposable)?.Dispose();
            _onEditTerrainAlgorithm = null;

            (_terrainAlgorithm as IDisposable)?.Dispose();
            _terrainAlgorithm = null;

            (_sampler as IDisposable)?.Dispose();
            _sampler = null;

            (_terrainMeshGenerator as IDisposable)?.Dispose();
            _terrainMeshGenerator = null;

            (_terrainTileGenerator as IDisposable)?.Dispose();
            _terrainTileGenerator = null;

            (_meshUpdater as IDisposable)?.Dispose();
            _meshUpdater = null;

            (_groupsSelector as IDisposable)?.Dispose();
            _groupsSelector = null;
        }

        private void OnDestroy()
        {
            _onDestroyCts.Cancel();
            DisposeService();
        }
    }
}