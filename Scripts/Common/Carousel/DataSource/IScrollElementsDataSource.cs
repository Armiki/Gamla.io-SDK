
namespace Gamla.UI.Carousel
{
    public interface IScrollElementsDataSource : IScrollElementsHolder
    {
        float totalHolderSize { get; }
        void ElementSizeChanged(int index, float newSize);
    }
}