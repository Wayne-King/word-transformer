using System;

namespace WayneKing.Practice.Abstractions
{
    public class TransformationNode
    {
        public TransformationNode(string word)
        {
            if (word == null)
                throw new ArgumentNullException(nameof(word));

            this.Word = word;
        }

        internal TransformationNode Parent { get; private set; }

        public string Word { get; private set; }

        internal void AttachToParent(TransformationNode parent)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));
            if (this.Parent != null)
                throw new InvalidOperationException("This node is already attached to a parent.");

            // CONSIDER: any need to check for a cycle?

            this.Parent = parent;
        }
    }
}