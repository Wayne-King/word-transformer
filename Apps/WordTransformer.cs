using System;
using System.Collections.Generic;
using WayneKing.Practice.Abstractions;

namespace WayneKing.Practice.Apps
{
    public class WordTransformer
    {
        private readonly ITransformationTraversal traversal;
        private readonly ITransformationNodeFactory nodeFactory;
        private readonly IWordModulator wordModulator;

        public WordTransformer(ITransformationTraversalWithMonitor traversal,
                ITransformationNodeFactory nodeFactory, IWordModulator wordModulator)
        {
            if (traversal == null)
                throw new ArgumentNullException(nameof(traversal));
            if (nodeFactory == null)
                throw new ArgumentNullException(nameof(nodeFactory));
            if (wordModulator == null)
                throw new ArgumentNullException(nameof(wordModulator));

            this.traversal = traversal;
            this.nodeFactory = nodeFactory;
            this.wordModulator = wordModulator;
        }

        public string[] Transform(string from, string to)
        {
            if (string.IsNullOrEmpty(from))
                throw new ArgumentNullException(nameof(from));
            if (string.IsNullOrEmpty(to))
                throw new ArgumentNullException(nameof(to));

            // CONSIDER: for cleanliness, maybe ITransformationTraversal needs a "clear/reset" action

            traversal.Embark(nodeFactory.CreateNode(from));
            do
            {
                var node = traversal.Disembark();

                if (node.Word == to)
                {
                    // hit goal
                    return GetRouteTo(node);
                }

                ModulateAndTraverse(node);

            } while (!traversal.IsEmpty);

            // transform not possible
            return new string[0];
        }

        private string[] GetRouteTo(TransformationNode node)
        {
            var stack = new Stack<string>();
            do
            {
                stack.Push(node.Word);
            } while ((node = node.Parent) != null);

            return stack.ToArray();
        }

        protected virtual void ModulateAndTraverse(TransformationNode node)
        {
            int count = 0;

            foreach (string word in wordModulator.GetModulationsOf(node.Word))
            {
                traversal.Embark(nodeFactory.CreateNode(node, word));
                count++;
            }

            OnEmbarkedModulationsOf(node.Word, count);
        }

        protected virtual void OnEmbarkedModulationsOf(string word, int count)
        {
        }
    }
}
