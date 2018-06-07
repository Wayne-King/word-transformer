using System;
using System.Collections.Generic;
using WayneKing.Practice.Abstractions;

namespace WayneKing.Practice.Apps
{
    public class FifoTraversal : ITransformationTraversal
    {
        private readonly Queue<TransformationNode> traversal;

        public FifoTraversal()
        {
            this.traversal = new Queue<TransformationNode>();
        }

        public bool IsEmpty
        {
            get { return traversal.Count == 0; }
        }

        public void Embark(TransformationNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            traversal.Enqueue(node);
        }

        public TransformationNode Disembark()
        {
            if (traversal.Count == 0)
                throw new InvalidOperationException("There are no nodes to traverse.");
                
            return traversal.Dequeue();
        }
    }
}