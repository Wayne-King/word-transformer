using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WayneKing.Practice.Trees.Tests
{
    [TestClass]
    public class TrieTreeTests
    {
        private TrieTree tree;

        public TrieTreeTests()
        {
            tree = new TrieTree();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddThrowsWhenNullWord()
        {
            tree.AddWord(null);
        }

        [TestMethod]
        [DataTestMethod]
            [DataRow("")][DataRow("9")][DataRow("a b")][DataRow("z%a")]
        public void AddEmptyOrNonLetterWordReturnsFalse(string word)
        {
            Assert.IsFalse(tree.AddWord(word));
            Assert.AreEqual(0, tree.Count, "count");
        }

        [TestMethod]
        public void SingleWordViaAddWord()
        {
            tree.AddWord("hello");
        }

        [TestMethod]
        public void SingleWordViaAddLetter()
        {
            AddLetters("hello");
            Assert.IsTrue(tree.IsWord("hello"));
        }

        private void AddLetters(string letters, bool appendSeparator = true)
        {
            foreach (char letter in letters)
                tree.AddLetter(letter);

            if (appendSeparator)
                tree.AddLetter(' ');
        }

        [TestMethod]
        public void IsWordThrowsWhenNullParam()
        {
            Assert.ThrowsException<ArgumentNullException>(() => tree.IsWord((string)null));
            Assert.ThrowsException<ArgumentNullException>(() => tree.IsWord((char[])null));
        }

        [TestMethod]
        public void IsWordWhenEmptyString()
        {
            Assert.IsFalse(tree.IsWord(string.Empty));
        }

        [TestMethod]
        public void IsWordWhenEmptyChars()
        {
            Assert.IsFalse(tree.IsWord(new char[0]));
        }

        [TestMethod]
        public void IsWordWhenUnknownWord()
        {
            Assert.IsFalse(tree.IsWord("bob"));
        }

        [TestMethod]
        public void IsWordWhenKnownWord()
        {
            tree.AddWord("bob");
            Assert.IsTrue(tree.IsWord("bob"));
        }

        // assuming as given that IsWord(string) and IsWord(char[]) simply
        //  defer directly to IsWordCore(), and just this single sanity test
        //  to explicitly exercise IsWord(char[])
        [TestMethod]
        public void IsWordViaCharArray()
        {
            tree.AddWord("bob");
            Assert.IsTrue(tree.IsWord("bob".ToCharArray()));
        }

        [TestMethod]
        public void IsNotWordWhenPrefixOfKnownWord()
        {
            tree.AddWord("aabb");
            Assert.IsFalse(tree.IsWord("aa"), "via AddWord");

            AddLetters("xxyy");
            Assert.IsFalse(tree.IsWord("xx"), "via AddLetter");
        }

        [TestMethod]
        public void IsNotWordWhenSuffixOfKnown()
        {
            tree.AddWord("aa");
            Assert.IsFalse(tree.IsWord("aaa"), "via AddWord");

            AddLetters("xx");
            Assert.IsFalse(tree.IsWord("xxx"), "via AddLetter");
        }

        [TestMethod]
        public void WordThenFollowOnWordViaAddWord()
        {
            tree.AddWord("aa");
            tree.AddWord("aabb");
            Assert.IsTrue(tree.IsWord("aa"));
            Assert.IsFalse(tree.IsWord("aab"));
            Assert.IsTrue(tree.IsWord("aabb"));
        }

        [TestMethod]
        public void WordThenFollowOnWordViaAddLetter()
        {
            AddLetters("aa aabb");
            Assert.IsTrue(tree.IsWord("aa"));
            Assert.IsFalse(tree.IsWord("aab"));
            Assert.IsTrue(tree.IsWord("aabb"));
        }

        [TestMethod]
        public void WordThenPrefixWordViaAddWord()
        {
            tree.AddWord("aabb");
            tree.AddWord("aa");
            Assert.IsTrue(tree.IsWord("aa"));
            Assert.IsFalse(tree.IsWord("aab"));
            Assert.IsTrue(tree.IsWord("aabb"));
        }

        [TestMethod]
        public void WordThenPrefixWordViaAddLetter()
        {
            AddLetters("aabb aa");
            Assert.IsTrue(tree.IsWord("aa"));
            Assert.IsFalse(tree.IsWord("aab"));
            Assert.IsTrue(tree.IsWord("aabb"));
        }

        [TestMethod]
        public void WordsIgnoreCaseViaAddWord()
        {
            tree.AddWord("aa");
            Assert.IsTrue(tree.IsWord("AA"), "aa == AA");

            int count = tree.Count;
            tree.AddWord("AA");
            Assert.AreEqual(count, tree.Count, "count aa/AA");

            tree.AddWord("AAbb");
            Assert.IsTrue(tree.IsWord("aaBB"), "AAbb == aaBB");
        }

        [TestMethod]
        public void WordsIgnoreCaseViaAddLetter()
        {
            AddLetters("aa");
            Assert.IsTrue(tree.IsWord("AA"), "aa == AA");

            int count = tree.Count;
            AddLetters("AA");
            Assert.AreEqual(count, tree.Count, "count aa/AA");

            AddLetters("AAbb");
            Assert.IsTrue(tree.IsWord("aaBB"), "AAbb == aaBB");
        }

        [TestMethod]
        public void DuplicateWordCountedOnceViaAddWord()
        {
            Assert.AreEqual(0, tree.Count, "empty");

            tree.AddWord("aa");
            Assert.AreEqual(1, tree.Count, "one word");
            tree.AddWord("aa");
            Assert.AreEqual(1, tree.Count, "dup word");

            tree.AddWord("aaa");
            Assert.AreEqual(2, tree.Count, "follow-on word");
        }

        [TestMethod]
        public void DuplicateWordCountedOnceViaAddLetter()
        {
            Assert.AreEqual(0, tree.Count, "empty");

            AddLetters("aa");
            Assert.AreEqual(1, tree.Count, "one word");
            AddLetters("aa");
            Assert.AreEqual(1, tree.Count, "dup word");

            AddLetters("aaa");
            Assert.AreEqual(2, tree.Count, "follow-on word");
        }

        [TestMethod]
        public void AddDuplicateWordReturnsTrue()
        {
            Assert.IsTrue(tree.AddWord("aa"), "initial");
            Assert.IsTrue(tree.AddWord("aa"), "duplicate");
        }

        [TestMethod]
        [DataTestMethod]
            [DataRow("aa bb ")][DataRow("aa\nbb\n")][DataRow("aa9bb9")][DataRow("aa-bb-")]
            [DataRow("aa  bb  ")][DataRow("aa\r\nbb\r\n")][DataRow("aa§bb§")]
        public void AddLetterNonLettersAreWordSeparators(string letters)
        {
            AddLetters(letters, appendSeparator: false);
            Assert.AreEqual(2, tree.Count, "count");
            Assert.IsTrue(tree.IsWord("aa"), "aa");
            Assert.IsTrue(tree.IsWord("bb"), "bb");
        }

        [TestMethod]
        public void OptimizeMarksFinalLetterAsEndOfWord()
        {
            AddLetters("aa", appendSeparator: false);
            tree.Optimize();
            Assert.IsTrue(tree.IsWord("aa"));
        }
    }

    [TestClass]
    public class NodeTests
    {
        [TestMethod]
        public void ConstructWithLetter()
        {
            var node = new TrieTree.Node('a');
            Assert.AreEqual('a', node.Letter, "Letter");
            Assert.IsFalse(node.IsEndOfWord, "IsEndOfWord");
        }

        [TestMethod]
        public void NodesCompareByLetterOnly()
        {
            var nodeA1 = new TrieTree.Node('a');
            var nodeA2 = new TrieTree.Node('a');
            var nodeB = new TrieTree.Node('b');
            var nodeC = nodeA2.GetOrCreateDescendant('c');

            nodeA1.IsEndOfWord = false;
            nodeA2.IsEndOfWord = true;

            Assert.AreEqual(0, nodeA1.CompareTo(nodeA2), "a == a'");
            Assert.IsTrue(nodeA1.CompareTo(nodeB) < 0, "a < b");
            Assert.IsTrue(nodeB.CompareTo(nodeA1) > 0, "b > a");
            Assert.IsTrue(nodeB.CompareTo(nodeC) < 0, "b < c");
        }

        [TestMethod]
        public void NodeNeverComparesWithNull()
        {
            Assert.AreNotEqual(0, new TrieTree.Node('a').CompareTo(null));
        }

        [TestMethod]
        public void AddDescendantLetter()
        {
            var node1 = new TrieTree.Node('a');
            var node2 = node1.GetOrCreateDescendant('b');

            Assert.IsNotNull(node2);
            Assert.AreNotSame(node1, node2);

            var node3 = node1.Children.GetOrCreateNode('b');
            Assert.AreEqual(node2, node3);
        }

        [TestMethod]
        public void LettersComparedCaseSensitively()
        {
            var node1 = new TrieTree.Node('.');
            var node2 = node1.GetOrCreateDescendant('a');
            var node3 = node1.GetOrCreateDescendant('A');

            Assert.AreNotSame(node2, node3);
        }
    }

    [TestClass]
    public class NodesListTests
    {
        [TestMethod]
        public void GetOrCreateWithNewLetter()
        {
            var node = new TrieTree.NodesList().GetOrCreateNode('a');

            Assert.IsNotNull(node);
            Assert.AreEqual('a', node.Letter);
        }

        [TestMethod]
        public void GetOrCreateTwoLetters()
        {
            var list = new TrieTree.NodesList();
            var initialCount = list.Count;
            var node1 = list.GetOrCreateNode('a');
            var count1 = list.Count;
            var node2 = list.GetOrCreateNode('b');
            var count2 = list.Count;

            Assert.IsNotNull(node1);
            Assert.IsNotNull(node2);
            Assert.AreNotSame(node1, node2);
            Assert.AreEqual('a', node1.Letter);
            Assert.AreEqual('b', node2.Letter);
            Assert.AreEqual(0, initialCount, "initial count");
            Assert.AreEqual(1, count1, "count 1");
            Assert.AreEqual(2, count2, "count 2");
        }

        [TestMethod]
        public void GetOrCreateWithKnownLetter()
        {
            var list = new TrieTree.NodesList();
            var node1 = list.GetOrCreateNode('a');
            var node2 = list.GetOrCreateNode('a');

            Assert.AreSame(node1, node2);
            Assert.AreEqual(1, list.Count, "Count");
        }
    }

    [TestClass]
    public class NodeWalkTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WalkThrowsWhenNullStart()
        {
            TrieTree.NodeWalk.Walk(null, n => {});
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WalkThrowsWhenNullAction()
        {
            TrieTree.NodeWalk.Walk(new TrieTree.Node('a'), null);
        }

        [TestMethod]
        public void WalkIsDepthFirst()
        {
            var nodeA = new TrieTree.Node('a');
            var nodeB = nodeA.Children.GetOrCreateNode('b');
            var nodeC = nodeA.Children.GetOrCreateNode('c');
            var nodeD = nodeC.Children.GetOrCreateNode('d');
            var nodeE = nodeC.Children.GetOrCreateNode('e');

            string walk = string.Empty;
            TrieTree.NodeWalk.Walk(nodeA, node => walk += node.Node.Letter);

            // DFS because typical tree is shallow & very wide
            Assert.AreEqual("acedb", walk);
        }

        [TestMethod]
        public void WalkStartsAtDepthZero()
        {
            TrieTree.NodeWalk.Walk(
                    new TrieTree.Node('a'),
                    node => Assert.AreEqual(0, node.Depth));
        }
    }

    [TestClass]
    public class ActuaryTests
    {
        [TestMethod]
        public void NoStatsUntilOptimize()
        {
            var tree = new TrieTree();
            Assert.IsNull(tree.Statistics);

            tree.AddWord("aa");
            Assert.IsNull(tree.Statistics);

            tree.Optimize();
            Assert.IsNotNull(tree.Statistics);            
        }

        [TestMethod]
        public void StatsWhenEmptyTree()
        {
            var tree = new TrieTree();
            tree.Optimize();
            var stats = tree.Statistics;

            Assert.AreEqual(0, stats.AverageChildCount, "AverageChildCount");
            Assert.AreEqual(0, stats.AverageWordLength, "AverageWordLength");
            Assert.AreEqual(0, stats.CummulativeWordLength, "CummulativeWordLength");
            Assert.AreEqual(0, stats.MaxWordLength, "MaxWordLength");
            Assert.AreEqual(1, stats.NodeCount, "NodeCount");
            Assert.AreEqual(0, stats.WordCount, "WordCount");
        }

        [TestMethod]
        public void StatsWhenSomeWords()
        {
            var tree = new TrieTree();
            tree.AddWord("aaa");
            tree.AddWord("abba");
            tree.AddWord("aba");
            tree.AddWord("abb");
            tree.AddWord("abc");
            tree.Optimize();
            var stats = tree.Statistics;

            Assert.AreEqual(8/5, stats.AverageChildCount, "AverageChildCount");
            Assert.AreEqual(16/5, stats.AverageWordLength, "AverageWordLength");
            Assert.AreEqual(16, stats.CummulativeWordLength, "CummulativeWordLength");
            Assert.AreEqual(4, stats.MaxWordLength, "MaxWordLength");
            Assert.AreEqual(9, stats.NodeCount, "NodeCount");
            Assert.AreEqual(5, stats.WordCount, "WordCount");
        }
    }
}
