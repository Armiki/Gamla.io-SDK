using System;

public interface IPullOnRefreshStrategy
{
    void Update();

    event Action onRefresh;
    void EndRefreshing();

    bool isRefreshing { get; }
}