using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;

namespace WayneKing.Practice.Trees
{
    public class TrieTreeBuilder
    {
        private readonly TrieTree tree;
        
        public TrieTreeBuilder()
        {
            tree = new TrieTree();
        }

        public TimeSpan LoadTime
        {
            get;
            private set;
        }

        /// <summary>
        /// Load a list of words, where each word is on a separate line.
        /// </summary>
        public void LoadWordListFromUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException("url");

            using (WebClient client = new WebClient())
            using (Stream stream = client.OpenRead(url))
            {
                LoadWordListFromStream(stream);
            }
        }

        /// <summary>
        /// Load a list of words, where each word is on a separate line.
        /// </summary>
        public void LoadWordListFromFile(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            using (Stream stream = File.OpenRead(path))
            {
                LoadWordListFromStream(stream);
            }
        }

        internal void LoadWordListFromStream(Stream stream)
        {
            if (this.LoadTime != TimeSpan.Zero)
                throw new InvalidOperationException("This builder has already been loaded; an instance cannot be loaded more than once.");

            using (StreamReader reader = OpenStreamReader(stream))
            {
                var stopwatch = Stopwatch.StartNew();

                for (string word = reader.ReadLine(); word != null; word = reader.ReadLine())
                {
                    TryAddWord(word);
                }

                this.LoadTime = stopwatch.Elapsed;
            }
        }

        private static StreamReader OpenStreamReader(Stream stream)
        {
            return new StreamReader(stream, Encoding.UTF8, false, 2048);
        }

        private void TryAddWord(string word)
        {
            if (!tree.AddWord(word))
            {
                Console.WriteLine($"Invalid word not added to tree: '{word}'.");
            }
        }

        /// <summary>
        /// Load words that are delimited by any non-letters.
        /// </summary>
        public void LoadWordsFromFile(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            using (Stream stream = File.OpenRead(path))
            {
                LoadWordsFromStream(stream);
            }
        }

        internal void LoadWordsFromStream(Stream stream)
        {
            if (this.LoadTime != TimeSpan.Zero)
                throw new InvalidOperationException("This builder has already been loaded; an instance cannot be loaded more than once.");

            using (StreamReader reader = OpenStreamReader(stream))
            {
                var stopwatch = Stopwatch.StartNew();

                for (int read = reader.Read(); read != -1; read = reader.Read())
                {
                    tree.AddLetter((char)read);
                }

                this.LoadTime = stopwatch.Elapsed;
            }
        }

        public TrieTree ToTree()
        {
            tree.Optimize();
            return tree;
        }
    }
}