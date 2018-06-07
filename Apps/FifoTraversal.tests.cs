using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WayneKing.Practice.Abstractions;

namespace WayneKing.Practice.Apps.Tests
{
    [TestClass]
    public class FifoTraversalTests
    {
        private static TransformationNode CreateNode()
        {
            return new TransformationNode("test");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EmbarkWithNullNodeThrows()
        {
            new FifoTraversal().Embark(null);
        }

        [TestMethod]
        public void InstanceIsInitiallyEmpty()
        {
            Assert.IsTrue(new FifoTraversal().IsEmpty);
        }

        [TestMethod]
        public void IsNotEmptyAfterNodeEmbarked()
        {
            var ft = new FifoTraversal();
            ft.Embark(CreateNode());
            Assert.IsFalse(ft.IsEmpty);
        }

        [TestMethod]
        public void IsEmptyAfterNodeEmbarkedAndDisembarked()
        {
            var ft = new FifoTraversal();
            ft.Embark(CreateNode());
            ft.Disembark();
            Assert.IsTrue(ft.IsEmpty);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DisembarkWhenEmptyThrows()
        {
            var ft = new FifoTraversal();
            ft.Embark(CreateNode());
            ft.Disembark();
            ft.Disembark();
        }

        [TestMethod]
        public void CanStillBeUsedAfterEmptied()
        {
            // ability to reuse is incidental; capturing in unit test just so
            // we know if behavior ever changes
            var ft = new FifoTraversal();
            ft.Embark(CreateNode());
            ft.Disembark();
            ft.Embark(CreateNode());
            Assert.IsFalse(ft.IsEmpty);
        }

        [TestMethod]
        public void DuplicateNodesAllowed()
        {
            // duplicate nodes may not be a good idea, but is allowed, at least incidentally
            var ft = new FifoTraversal();
            var node = CreateNode();
            ft.Embark(node);
            ft.Embark(node);            
            ft.Disembark();
            ft.Disembark();
            Assert.IsTrue(ft.IsEmpty);
        }
    }
}