using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace WayneKing.Practice.Trees
{
    public class TrieTree : WayneKing.Practice.Abstractions.IWordDictionary
    {
        internal class Node : IComparable<Node>
        {
            public char Letter { get; private set; }
            public bool IsEndOfWord { get; set; }
            public NodesList Children { get; private set; }

            public Node(char letter)
            {
                this.Letter = letter;
                this.Children = new NodesList();
            }

            public int CompareTo(Node node)
            {
                return node == null ? int.MinValue : this.Letter.CompareTo(node.Letter);
            }

            public Node GetOrCreateDescendant(char letter)
            {
                return this.Children.GetOrCreateNode(letter);
            }

            public Node GetDescendant(char letter)
            {
                return this.Children.GetNode(letter);
            }

            public void Optimize()
            {
                this.Children.Optimize();
            }
        }

        internal class NodesList : IEnumerable<Node>
        {
            private readonly List<Node> nodes;

            public NodesList()
            {
                nodes = new List<Node>();
            }

            public int Count
            {
                get { return nodes.Count; }
            }

            public Node GetOrCreateNode(char ch)
            {
                // this method is part of the "generate" algorithm, and thus
                //  this list is still unoptimized/unsorted --> just a linear
                //  search, and add new letters to end only
                Node node = nodes.Find(n => n.Letter == ch);
                if (node == null)
                {
                    node = new Node(ch);
                    nodes.Add(node);
                }

                return node;
            }

            public Node GetNode(char ch)
            {
                return nodes.Find(n => n.Letter == ch);
            }

            public IEnumerator<Node> GetEnumerator()
            {
                return nodes.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return nodes.GetEnumerator();
            }

            public void Optimize()
            {
                nodes.Sort();
                nodes.TrimExcess();
            }
        }

        internal class NodeWalk
        {
            public Node Node { get; private set; }
            public int Depth { get; private set; }

            private NodeWalk(Node node, int depth)
            {
                this.Node = node;
                this.Depth = depth;
            }

            public static void Walk(Node start, Action<NodeWalk> action)
            {
                if (start == null || action == null)
                    throw new ArgumentNullException(start == null ? "start" : "action");

                var traversal = new Stack<NodeWalk>();
                traversal.Push(new NodeWalk(start, 0));

                while (traversal.Count > 0)
                {
                    var node = traversal.Pop();
                    action(node);

                    int nextDepth = node.Depth + 1;
                    foreach (Node child in node.Node.Children)
                    {
                        traversal.Push(new NodeWalk(child, nextDepth));
                    }
                }
            }
        }

        public class Actuary
        {
            internal Actuary()
            {
            }

            private int cummulativeChildCount;
            private int internalNodeCount;

            public float AverageChildCount   { get; private set; }
            public float AverageWordLength   { get; private set; }
            public int CummulativeWordLength { get; private set; }
            public int MaxWordLength         { get; private set; }
            public int NodeCount             { get; private set; }
            public int WordCount             { get; private set; }

            internal void Track(NodeWalk node)
            {
                this.NodeCount++;

                if (node.Node.IsEndOfWord)
                {
                    this.WordCount++;
                    this.CummulativeWordLength += node.Depth;

                    if (node.Depth > this.MaxWordLength)
                        this.MaxWordLength = node.Depth;
                }

                int childCount = node.Node.Children.Count;
                if (childCount > 0)
                {
                    internalNodeCount++;
                    cummulativeChildCount += childCount;
                }
            }

            internal void Publish()
            {
                if (this.WordCount != 0)
                    this.AverageWordLength = this.CummulativeWordLength / this.WordCount;

                if (internalNodeCount != 0)
                    this.AverageChildCount = cummulativeChildCount / internalNodeCount;
            }
        }

        private readonly Node root;
        private Node prior;

        public TrieTree()
        {
            root = new Node('.');
            prior = root;
        }

        public int Count
        {
            get;
            private set;
        }

        public Actuary Statistics
        {
            get;
            private set;
        }

        internal bool AddWord(string word)
        {
            if (word == null)
                throw new ArgumentNullException("word");

            // CONSIDER: handle bad letters as word is iterated
            // in order to avoid scanning it twice; but will need a way
            // to rollback partially-added word.
            if (word.Length == 0 || Regex.IsMatch(word, @"[^a-zA-Z]"))
                return false;

            // TODO: convert to lower JIT within the loop
            word = word.ToLowerInvariant();

            Node current = root;
            foreach (char ch in word)
            {
                current = current.GetOrCreateDescendant(ch);
            }

            if (!current.IsEndOfWord)
            {
                current.IsEndOfWord = true;
                this.Count++;
            }

            // note 'true' indicates 'word' is in the dictionary
            return true;
        }

        internal void AddLetter(char letter)
        {
            if (char.IsLetter(letter))
            {
                letter = char.ToLowerInvariant(letter);
                prior = prior.GetOrCreateDescendant(letter);
            }
            else
            {
                MarkWordAndReset();
            }
        }

        private void MarkWordAndReset()
        {
            if (!object.ReferenceEquals(prior, root))
            {
                // prior letter was the last letter of a word
                // mark it as word, if not already:
                if (!prior.IsEndOfWord)
                {
                    prior.IsEndOfWord = true;
                    this.Count++;
                }

                // reset to top of tree, ready for first letter of next word
                prior = root;
            }
        }

        public bool IsWord(string word)
        {
            return IsWordCore(word);
        }

        public bool IsWord(char[] word)
        {
            return IsWordCore(word);
        }

        private bool IsWordCore(IEnumerable<char> word)
        {
            if (word == null)
                throw new ArgumentNullException(nameof(word));

            Node current = root;
            foreach (char ch in word)
            {
                current = current.GetDescendant(char.ToLowerInvariant(ch));
                if (current == null)
                    return false;
            }

            return current.IsEndOfWord;
        }

        internal void Optimize()
        {
            // insure very last word is recorded
            MarkWordAndReset();
            prior = null;

            ComputeStatistics();
        }

        private void ComputeStatistics()
        {
            var actuary = new Actuary();

            NodeWalk.Walk(root, node =>
                {
                    node.Node.Optimize();
                    actuary.Track(node);
                });

            actuary.Publish();
            this.Statistics = actuary;
        }
    }
}