using UnityEngine;
using Votyra.Core.Images;
using Votyra.Core.MeshUpdaters;
using Votyra.Core.Models;
using Votyra.Core.Utils;
using Votyra.Core.GroupSelectors;
using Votyra.Plannar.Images;
using Votyra.Plannar.Images.Constraints;
using Votyra.Core.ImageSamplers;
using Votyra.Core.TerrainGenerators;
using Votyra.Core.TerrainGenerators.TerrainMeshers;
using Zenject;
using Votyra.Core;
using System;
using Votyra.Core.Behaviours;

namespace Votyra.Plannar.Unity
{
    public class Terrain2iInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<TerrainGenerator2i>().AsSingle();
            Container.BindInterfacesAndSelfTo<TerrainMeshUpdater<Vector2i>>().AsSingle();
            Container.BindInterfacesAndSelfTo<GroupsByCameraVisibilitySelector2i>().AsSingle();
            Container.BindInterfacesAndSelfTo<InitialStateSetter2f>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<EditableMatrixImage2f>().AsSingle();
            Container.BindInstance<GameObject>(this.gameObject).WithId("root").AsSingle();
            Container.BindInterfacesAndSelfTo<ClickToPaint2i>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<TerrainGeneratorManager<IFrameData2i, Vector2i>>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<FrameData2iProvider>().AsSingle();

            Container.Bind<Func<GameObject>>()
                .FromMethod(context =>
                {
                    var root = context.Container.ResolveId<GameObject>("root");
                    var terrainConfig = context.Container.Resolve<ITerrainConfig>();
                    var materialConfig = context.Container.Resolve<IMaterialConfig>();
                    Func<GameObject> factory = () => CreateNewGameObject(root, terrainConfig, materialConfig);
                    return factory;

                }).AsSingle();
        }

        private GameObject CreateNewGameObject(GameObject root, ITerrainConfig terrainConfig, IMaterialConfig materialConfig)
        {
            var go = new GameObject();
            go.transform.SetParent(root.transform, false);
            if (terrainConfig.DrawBounds)
            {
                go.AddComponent<DrawBounds>();
            }
            var meshRenderer = go.GetOrAddComponent<MeshRenderer>();
            meshRenderer.materials = ArrayUtils.CreateNonNull(materialConfig.Material, materialConfig.MaterialWalls);
            return go;
        }
    }
}