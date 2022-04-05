using Gamla.Scripts.Common.Carousel.Core;

namespace Gamla.Scripts.Common.Carousel.DataSource
{
    public interface IScrollElementsDataSource : IScrollElementsHolder
    {
        float totalHolderSize { get; }
        void ElementSizeChanged(int index, float newSize);
    }
}