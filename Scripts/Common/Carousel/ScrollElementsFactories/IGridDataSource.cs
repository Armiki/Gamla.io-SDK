namespace Gamla.UI.Carousel
{
    public interface IGridDataSource : IScrollElementsDataSource
    {
        int columnsCount { get; }
        bool HasData(int row, int column);
    }
}