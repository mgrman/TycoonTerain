using System.Linq;
using NUnit.Framework;
using Votyra.Core.Models;

namespace Votyra.Cubical.Tests.Editor.SampledData3bTests
{
    public class ParseCubeTests
    {
        [Test]
        public void FromStringToString()
        {
            var cubeString = @"
              0-----0
             /|    /|
            0-+---0 |
            | 0---+-0
            |/    |/
            1-----0
            ";

            var cube = SampledData3b.ParseCube(cubeString);

            Assert.AreEqual(string.Join("\n", cubeString.Split('\n')
                .Select(o => o.Trim())), string.Join("\n", cube.ToCubeString()
                .Split('\n')
                .Select(o => o.Trim())));
        }

        [Test]
        public void x0y0z0()
        {
            var cubeString = @"
              0-----0
             /|    /|
            0-+---0 |
            | 0---+-0
            |/    |/
            1-----0
            ";
            var cube = SampledData3b.ParseCube(cubeString);

            Assert.IsTrue(cube.Data_x0y0z0);
            Assert.IsFalse(cube.Data_x0y0z1);
            Assert.IsFalse(cube.Data_x0y1z0);
            Assert.IsFalse(cube.Data_x0y1z1);
            Assert.IsFalse(cube.Data_x1y0z0);
            Assert.IsFalse(cube.Data_x1y0z1);
            Assert.IsFalse(cube.Data_x1y1z0);
            Assert.IsFalse(cube.Data_x1y1z1);
        }

        [Test]
        public void x0y0z1()
        {
            var cubeString = @"
              0-----0
             /|    /|
            1-+---0 |
            | 0---+-0
            |/    |/
            0-----0
            ";
            var cube = SampledData3b.ParseCube(cubeString);

            Assert.IsFalse(cube.Data_x0y0z0);
            Assert.IsTrue(cube.Data_x0y0z1);
            Assert.IsFalse(cube.Data_x0y1z0);
            Assert.IsFalse(cube.Data_x0y1z1);
            Assert.IsFalse(cube.Data_x1y0z0);
            Assert.IsFalse(cube.Data_x1y0z1);
            Assert.IsFalse(cube.Data_x1y1z0);
            Assert.IsFalse(cube.Data_x1y1z1);
        }

        [Test]
        public void x0y1z0()
        {
            var cubeString = @"
              0-----0
             /|    /|
            0-+---0 |
            | 1---+-0
            |/    |/
            0-----0
            ";
            var cube = SampledData3b.ParseCube(cubeString);

            Assert.IsFalse(cube.Data_x0y0z0);
            Assert.IsFalse(cube.Data_x0y0z1);
            Assert.IsTrue(cube.Data_x0y1z0);
            Assert.IsFalse(cube.Data_x0y1z1);
            Assert.IsFalse(cube.Data_x1y0z0);
            Assert.IsFalse(cube.Data_x1y0z1);
            Assert.IsFalse(cube.Data_x1y1z0);
            Assert.IsFalse(cube.Data_x1y1z1);
        }

        [Test]
        public void x0y1z1()
        {
            var cubeString = @"
              1-----0
             /|    /|
            0-+---0 |
            | 0---+-0
            |/    |/
            0-----0
            ";
            var cube = SampledData3b.ParseCube(cubeString);

            Assert.IsFalse(cube.Data_x0y0z0);
            Assert.IsFalse(cube.Data_x0y0z1);
            Assert.IsFalse(cube.Data_x0y1z0);
            Assert.IsTrue(cube.Data_x0y1z1);
            Assert.IsFalse(cube.Data_x1y0z0);
            Assert.IsFalse(cube.Data_x1y0z1);
            Assert.IsFalse(cube.Data_x1y1z0);
            Assert.IsFalse(cube.Data_x1y1z1);
        }

        [Test]
        public void x1y0z0()
        {
            var cubeString = @"
              0-----0
             /|    /|
            0-+---0 |
            | 0---+-0
            |/    |/
            0-----1
            ";
            var cube = SampledData3b.ParseCube(cubeString);

            Assert.IsFalse(cube.Data_x0y0z0);
            Assert.IsFalse(cube.Data_x0y0z1);
            Assert.IsFalse(cube.Data_x0y1z0);
            Assert.IsFalse(cube.Data_x0y1z1);
            Assert.IsTrue(cube.Data_x1y0z0);
            Assert.IsFalse(cube.Data_x1y0z1);
            Assert.IsFalse(cube.Data_x1y1z0);
            Assert.IsFalse(cube.Data_x1y1z1);
        }

        [Test]
        public void x1y0z1()
        {
            var cubeString = @"
              0-----0
             /|    /|
            0-+---1 |
            | 0---+-0
            |/    |/
            0-----0
            ";
            var cube = SampledData3b.ParseCube(cubeString);

            Assert.IsFalse(cube.Data_x0y0z0);
            Assert.IsFalse(cube.Data_x0y0z1);
            Assert.IsFalse(cube.Data_x0y1z0);
            Assert.IsFalse(cube.Data_x0y1z1);
            Assert.IsFalse(cube.Data_x1y0z0);
            Assert.IsTrue(cube.Data_x1y0z1);
            Assert.IsFalse(cube.Data_x1y1z0);
            Assert.IsFalse(cube.Data_x1y1z1);
        }

        [Test]
        public void x1y1z0()
        {
            var cubeString = @"
              0-----0
             /|    /|
            0-+---0 |
            | 0---+-1
            |/    |/
            0-----0
            ";
            var cube = SampledData3b.ParseCube(cubeString);

            Assert.IsFalse(cube.Data_x0y0z0);
            Assert.IsFalse(cube.Data_x0y0z1);
            Assert.IsFalse(cube.Data_x0y1z0);
            Assert.IsFalse(cube.Data_x0y1z1);
            Assert.IsFalse(cube.Data_x1y0z0);
            Assert.IsFalse(cube.Data_x1y0z1);
            Assert.IsTrue(cube.Data_x1y1z0);
            Assert.IsFalse(cube.Data_x1y1z1);
        }

        [Test]
        public void x1y1z1()
        {
            var cubeString = @"
              0-----1
             /|    /|
            0-+---0 |
            | 0---+-0
            |/    |/
            0-----0
            ";
            var cube = SampledData3b.ParseCube(cubeString);

            Assert.IsFalse(cube.Data_x0y0z0);
            Assert.IsFalse(cube.Data_x0y0z1);
            Assert.IsFalse(cube.Data_x0y1z0);
            Assert.IsFalse(cube.Data_x0y1z1);
            Assert.IsFalse(cube.Data_x1y0z0);
            Assert.IsFalse(cube.Data_x1y0z1);
            Assert.IsFalse(cube.Data_x1y1z0);
            Assert.IsTrue(cube.Data_x1y1z1);
        }
    }
}