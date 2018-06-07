using System;
using WayneKing.Practice.Abstractions;

namespace WayneKing.Practice.Apps
{
    public class TransformationNodeFactory : ITransformationNodeFactory
    {
        public TransformationNodeFactory()
        {
        }

        public TransformationNode CreateNode(string word)
        {
            return new TransformationNode(word);
        }

        public TransformationNode CreateNode(TransformationNode precursor, string word)
        {
            // can remove this restriction if we want (since the other overload allows it, anyway)
            if (precursor == null)
                throw new ArgumentNullException(nameof(precursor));

            var node = new TransformationNode(word);
            node.AttachToParent(precursor);
            return node;
        }
    }
}