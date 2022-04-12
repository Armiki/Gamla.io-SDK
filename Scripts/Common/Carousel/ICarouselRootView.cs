namespace Gamla.UI.Carousel
{
    public interface ICarouselRootView
    {
        ScrollRectCarouselView carouselView { get; }
        GUIView carouselRootView { get; }
    }
}