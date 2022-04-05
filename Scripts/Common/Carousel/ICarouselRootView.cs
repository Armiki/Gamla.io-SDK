using Gamla.Scripts.Common.UI;

namespace Gamla.Scripts.Common.Carousel
{
    public interface ICarouselRootView
    {
        ScrollRectCarouselView carouselView { get; }
        GUIView carouselRootView { get; }
    }
}