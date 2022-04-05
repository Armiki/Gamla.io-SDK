using System;
using Gamla.Scripts.Common.UI;
using UnityEngine;

namespace Gamla.GUI.Common.Carousel
{
    public interface ICarouselView : IGUIView
    {
        event Action<ICarouselView, float> onScrollPositionChanged;
        event Action<ICarouselView, float> onScrollWindowSizeChanged;
        event Action<ICarouselView, int, float> onElementSizeChanged;
        event Action<ICarouselView> onUpdate;
        
        Transform itemsRoot { get; }
    
        float normalizedScrollPosition { get; }
        bool isHorizontal { get; }
        int directionValue { get; }
        void SetContentSize(float contentSize);
        void Reset(Action completionHandler = null);
        void OnElementSizeChanged(int index, float newSize);
        void CompensateScrollPosition(float delta);
        void SetScrollPosition(float position, Action onComplete = null);
        void Refresh();
    }
}