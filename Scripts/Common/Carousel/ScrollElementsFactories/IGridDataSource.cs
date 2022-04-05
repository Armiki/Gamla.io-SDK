using Gamla.Scripts.Common.Carousel.DataSource;

namespace Gamla.Scripts.Common.Carousel.ScrollElementsFactories
{
    public interface IGridDataSource : IScrollElementsDataSource
    {
        int columnsCount { get; }
        bool HasData(int row, int column);
    }
}