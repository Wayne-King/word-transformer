using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WayneKing.Practice.Apps.Tests
{
    [TestClass]
    public class TransformationNodeFactoryTests
    {
        private TransformationNodeFactory factory;

        public TransformationNodeFactoryTests()
        {
            factory = new TransformationNodeFactory();
        }

        [TestMethod]
        public void CreateNodeWithNoPrecursor()
        {
            var node = factory.CreateNode("word");
            Assert.IsNotNull(node);
            Assert.AreEqual("word", node.Word, "word");
            Assert.IsNull(node.Parent, "parent");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateWithNullPrecursorThrows()
        {
            factory.CreateNode(null, "word");
        }

        [TestMethod]
        public void CreateNodeWithPrecursor()
        {
            var precursor = factory.CreateNode("precursor");
            var node = factory.CreateNode(precursor, "word");
            Assert.AreSame(precursor, node.Parent, "parent");
            Assert.AreEqual("word", node.Word, "word");
        }
    }
}