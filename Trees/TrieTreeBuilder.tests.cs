using System;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WayneKing.Practice.Trees.Tests
{
    [TestClass]
    public class TrieTreeBuilderTests
    {
        [TestMethod]
        [DataTestMethod]
            [DataRow("aa\nbb\n")][DataRow("aa\r\nbb\r\n")]
            [DataRow("aa\nbb")][DataRow("aa\r\nbb")]
        public void LoadStreamWithLineBreaks(string lines)
        {
            var builder = new TrieTreeBuilder();
            using (Stream stream = ToStream(lines))
                builder.LoadWordListFromStream(stream);
            var tree = builder.ToTree();

            Assert.AreEqual(2, tree.Count, "count");
            Assert.IsTrue(tree.IsWord("aa"), "aa");
            Assert.IsTrue(tree.IsWord("bb"), "bb");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        [DataTestMethod]
            [DataRow(null)][DataRow("")]
        public void LoadFromUrlThrowsWhenNullOrEmptyUrl(string url)
        {
            new TrieTreeBuilder().LoadWordListFromUrl(url);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        [DataTestMethod]
            [DataRow(null)][DataRow("")]
        public void LoadFromFileThrowsWhenNullOrEmptyUrl(string path)
        {
            new TrieTreeBuilder().LoadWordListFromFile(path);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        [DataTestMethod]
            [DataRow(null)][DataRow("")]
        public void LoadWordsFromFileThrowsWhenNullOrEmptyUrl(string path)
        {
            new TrieTreeBuilder().LoadWordsFromFile(path);
        }

        [TestMethod]
        public void LoadStreamByLetter()
        {
            // we don't need to test various word separators (e.g.,
            // LoadStreamWithLineBreaks does) because when loading
            // by letter, the TrieTree class handles that (not
            // TrieTreeBuilder)
            var builder = new TrieTreeBuilder();
            using (Stream stream = ToStream("aa bb"))
                builder.LoadWordsFromStream(stream);
            var tree = builder.ToTree();

            Assert.AreEqual(2, tree.Count, "count");
            Assert.IsTrue(tree.IsWord("aa"), "aa");
            Assert.IsTrue(tree.IsWord("bb"), "bb");
        }

        [TestMethod]
        public void LoadListFromUrl()
        {
            LoadFromX((builder, file) => builder.LoadWordListFromUrl("file://" + file));
        }

        [TestMethod]
        public void LoadListFromFile()
        {
            LoadFromX((builder, file) => builder.LoadWordListFromUrl(file));
        }

        [TestMethod]
        public void LoadWordsFromFile()
        {
            LoadFromX((builder, file) => builder.LoadWordsFromFile(file));
        }

        private void LoadFromX(Action<TrieTreeBuilder, string> loadAction)
        {
            string file = Path.GetTempFileName();
            File.WriteAllLines(file, new[] { "aa", "bb" });

            var builder = new TrieTreeBuilder();
            loadAction(builder, file);
            var tree = builder.ToTree();

            Assert.AreEqual(2, tree.Count, "count");
            Assert.IsTrue(tree.IsWord("aa"), "aa");
            Assert.IsTrue(tree.IsWord("bb"), "bb");

            File.Delete(file);
        }

        [TestMethod]
        public void LoadTimeIsInitiallyZero()
        {
            Assert.AreEqual(TimeSpan.Zero, new TrieTreeBuilder().LoadTime);
        }

        [TestMethod]
        public void LoadTimeIncreasesAfterLoadList()
        {
            var builder = new TrieTreeBuilder();
            using (Stream stream = ToStream("aa"))
                builder.LoadWordListFromStream(stream);

            Assert.IsTrue(builder.LoadTime > TimeSpan.Zero);
        }

        [TestMethod]
        public void LoadTimeIncreasesAfterLoadWords()
        {
            var builder = new TrieTreeBuilder();
            using (Stream stream = ToStream("aa"))
                builder.LoadWordsFromStream(stream);

            Assert.IsTrue(builder.LoadTime > TimeSpan.Zero);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AnInstanceCanOnlyBeLoadedOnce_ListList()
        {
            var builder = new TrieTreeBuilder();
            using (Stream stream = ToStream("aa"))
                builder.LoadWordListFromStream(stream);
            using (Stream stream = ToStream("bb"))
                builder.LoadWordListFromStream(stream);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AnInstanceCanOnlyBeLoadedOnce_WordsWords()
        {
            var builder = new TrieTreeBuilder();
            using (Stream stream = ToStream("aa"))
                builder.LoadWordsFromStream(stream);
            using (Stream stream = ToStream("bb"))
                builder.LoadWordsFromStream(stream);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AnInstanceCanOnlyBeLoadedOnce_ListWords()
        {
            var builder = new TrieTreeBuilder();
            using (Stream stream = ToStream("aa"))
                builder.LoadWordListFromStream(stream);
            using (Stream stream = ToStream("bb"))
                builder.LoadWordsFromStream(stream);
        }

        private static Stream ToStream(string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            return new MemoryStream(Encoding.ASCII.GetBytes(value));
        }
    }
}