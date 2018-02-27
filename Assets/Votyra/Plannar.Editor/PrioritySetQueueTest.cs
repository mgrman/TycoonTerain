using System;
using System.Collections.Generic;
using NUnit.Framework;
using Votyra.Core.Models;
using Votyra.Plannar.Images.Constraints;

namespace Votyra.Plannar.Editor
{
    [TestFixture]
    public class PrioritySetQueueTest
    {
        [Test]
        public void New_FloatAscending()
        {
            var initialValues = new Dictionary<Vector2i, float>()
                { { new Vector2i(1, 1), 1 }, { new Vector2i(0, 0), 0 }, { new Vector2i(7, 7), 7 }
                };

            var queue = new PrioritySetQueue<Vector2i, float>(initialValues.Keys, EqualityComparer<Vector2i>.Default, o => initialValues[o], Comparer<float>.Create((a, b) => a.CompareTo(b)));

            var first = queue.GetFirst();
            var second = queue.GetFirst();
            var third = queue.GetFirst();

            Assert.AreEqual(new Vector2i(0, 0), first);
            Assert.AreEqual(new Vector2i(1, 1), second);
            Assert.AreEqual(new Vector2i(7, 7), third);
        }

        [Test]
        public void New_FloatAscending_AddedToCorrectPosition()
        {
            var initialValues = new Dictionary<Vector2i, float>()
                { { new Vector2i(1, 1), 1 }, { new Vector2i(0, 0), 0 }, { new Vector2i(7, 7), 7 }
                };

            var queue = new PrioritySetQueue<Vector2i, float>(initialValues.Keys, EqualityComparer<Vector2i>.Default, o => initialValues[o], Comparer<float>.Create((a, b) => a.CompareTo(b)));

            var first = queue.GetFirst();

            queue.Add(new Vector2i(2, 2), 2);

            var second = queue.GetFirst();
            var third = queue.GetFirst();
            var fourth = queue.GetFirst();
            Assert.AreEqual(new Vector2i(0, 0), first);
            Assert.AreEqual(new Vector2i(1, 1), second);
            Assert.AreEqual(new Vector2i(2, 2), third);
            Assert.AreEqual(new Vector2i(7, 7), fourth);
        }

        [Test]
        public void New_FloatDescending()
        {
            var initialValues = new Dictionary<Vector2i, float>()
                { { new Vector2i(1, 1), 1 }, { new Vector2i(0, 0), 0 }, { new Vector2i(7, 7), 7 }
                };

            var queue = new PrioritySetQueue<Vector2i, float>(initialValues.Keys, EqualityComparer<Vector2i>.Default, o => initialValues[o], Comparer<float>.Create((a, b) => -a.CompareTo(b)));

            var first = queue.GetFirst();
            var second = queue.GetFirst();
            var third = queue.GetFirst();

            Assert.AreEqual(new Vector2i(7, 7), first);
            Assert.AreEqual(new Vector2i(1, 1), second);
            Assert.AreEqual(new Vector2i(0, 0), third);
        }

        [Test]
        public void New_FloatDescending_AddedToCorrectPosition()
        {
            var initialValues = new Dictionary<Vector2i, float>()
                { { new Vector2i(1, 1), 1 }, { new Vector2i(0, 0), 0 }, { new Vector2i(7, 7), 7 }
                };

            var queue = new PrioritySetQueue<Vector2i, float>(initialValues.Keys, EqualityComparer<Vector2i>.Default, o => initialValues[o], Comparer<float>.Create((a, b) => -a.CompareTo(b)));

            var first = queue.GetFirst();
            queue.Add(new Vector2i(2, 2), 2);

            var second = queue.GetFirst();
            var third = queue.GetFirst();
            var fourth = queue.GetFirst();
            Assert.AreEqual(new Vector2i(7, 7), first);
            Assert.AreEqual(new Vector2i(2, 2), second);
            Assert.AreEqual(new Vector2i(1, 1), third);
            Assert.AreEqual(new Vector2i(0, 0), fourth);
        }
    }
}