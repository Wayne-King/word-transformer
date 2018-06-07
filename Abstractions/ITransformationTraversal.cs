namespace WayneKing.Practice.Abstractions
{
    public interface ITransformationTraversal
    {
        bool IsEmpty { get; }
        
        void Embark(TransformationNode node);

        TransformationNode Disembark();
    }
}