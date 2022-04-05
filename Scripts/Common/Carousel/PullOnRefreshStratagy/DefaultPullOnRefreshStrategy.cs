using System;
using Gamla.GUI.Common.Carousel;
using Gamla.Scripts.Common.Carousel;
using Gamla.Scripts.Common.Carousel.DataSource;

public class DefaultPullOnRefreshStrategy : IPullOnRefreshStrategy
{
    public event Action onRefresh;
    public bool isRefreshing { get; private set; }

    readonly IScrollElementsDataSource _dataSource;
    readonly ICarouselView _view;
    int linesCount => _dataSource.TotalCount;
    float normalizedIndex => _normalizedPosition * linesCount;

    float _normalizedPosition;

    const float ElementsCountBeforeEndToBeginRefresh = 5f;

    public DefaultPullOnRefreshStrategy(
        IScrollElementsDataSource dataSource,
        ICarouselView view)
    {
        _dataSource = dataSource;
        _view = view;
    }

    public void EndRefreshing()
    {
        isRefreshing = false;
    }

    public void Update()
    {
        _normalizedPosition = _view.normalizedScrollPosition;
        
        if (IsNearBorder())
        {
            HandleRefresh();
        }
    }

    bool IsNearBorder()
    {
        if (normalizedIndex < ElementsCountBeforeEndToBeginRefresh)
        {
            return true;
        }

        if (normalizedIndex > linesCount - ElementsCountBeforeEndToBeginRefresh)
        {
            return true;
        }

        return false;
    }

    void HandleRefresh()
    {
        if (isRefreshing)
        {
            //return;
        }

        isRefreshing = true;
        onRefresh?.Invoke();
    }
}