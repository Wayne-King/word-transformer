using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WayneKing.Practice.Abstractions.Tests
{
    [TestClass]
    public class TransformationNodeTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructWithNullWordThrows()
        {
            new TransformationNode(null);
        }

        [TestMethod]
        public void ConstructWithWordStoresWord()
        {
            Assert.AreEqual("word", new TransformationNode("word").Word);
        }

        [TestMethod]
        public void ParentIsInitiallyNull()
        {
            Assert.IsNull(new TransformationNode("word").Parent);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AttachToNullParentThrows()
        {
            new TransformationNode("word").AttachToParent(null);
        }

        [TestMethod]
        public void AttachToParentStoresParent()
        {
            var parent = new TransformationNode("parent");
            var child = new TransformationNode("child");
            child.AttachToParent(parent);
            Assert.AreSame(parent, child.Parent);
        }

        [TestMethod]
        public void ReattachToParentThrows()
        {
            var parent = new TransformationNode("parent");
            var newParent = new TransformationNode("newParent");
            var child = new TransformationNode("child");
            child.AttachToParent(parent);

            Assert.ThrowsException<InvalidOperationException>(() => child.AttachToParent(parent), "original parent");
            Assert.ThrowsException<InvalidOperationException>(() => child.AttachToParent(newParent), "new parent");
        }
    }
}