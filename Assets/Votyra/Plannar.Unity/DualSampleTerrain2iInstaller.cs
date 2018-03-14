using UnityEngine;
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
using Votyra.Core.Images.Constraints;

namespace Votyra.Plannar.Unity
{
    public class DualSampleTerrain2iInstaller : MonoInstaller
    {

        public override void InstallBindings()
        {
            Container.Rebind<IImageSampler2i>().To<DualImageSampler2i>().AsSingle();
            Container.Rebind<IImageConstraint2i>().To<DualSampledTycoonTileConstraint2i>().AsSingle();
            Container.Rebind<ITerrainMesher2i>().To<TerrainMesherWithWalls2i>().AsSingle();
        }

    }
}