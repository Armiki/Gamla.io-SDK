using Gamla.Scripts.Common.Carousel;
using Gamla.Scripts.Common.UI;
using UnityEngine;

namespace Gamla.Scripts.Test
{
    public class TestCarouselWindow : GUIView,  ICarouselRootView
    {
        [SerializeField] private ScrollRectCarouselView _carouselView;
        
        public ScrollRectCarouselView carouselView => _carouselView;
        public GUIView carouselRootView => this;
    }
}