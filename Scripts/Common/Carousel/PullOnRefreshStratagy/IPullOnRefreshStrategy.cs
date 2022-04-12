using System;

namespace Gamla.UI.Carousel
{
    public interface IPullOnRefreshStrategy
    {
        void Update();

        event Action onRefresh;
        void EndRefreshing();

        bool isRefreshing { get; }
    }
}