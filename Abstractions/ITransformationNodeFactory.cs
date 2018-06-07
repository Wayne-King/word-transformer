namespace WayneKing.Practice.Abstractions
{
    public interface ITransformationNodeFactory
    {
        TransformationNode CreateNode(string word);
        TransformationNode CreateNode(TransformationNode precursor, string word);
    }
}