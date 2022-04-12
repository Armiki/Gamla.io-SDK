using System;
using System.Collections.Generic;
using System.Linq;
using Gamla.UI.Carousel;
using UnityEngine;

namespace Gamla.Logic
{
    public class CarouselPresenter : ITicktackable
    {
        public Action<IPullOnRefreshStrategy> onPullOnRefresh;
        public int startActiveItem;

        #region - State

        ICarouselView _view;
        readonly List<Action> _lateUpdateActions = new List<Action>();

        #endregion

        readonly IScrollElementsDataSource _elementsHolder;
        readonly IScrollController _scrollController;

        IPullOnRefreshStrategy _pullOnRefreshStrategy;

        bool viewIsAlive
        {
            get
            {
                //hack: иногда interface ссылка продолжает жить, несмотря на то что сам gameobject уже уничтожен
                bool isAlive = (_view as MonoBehaviour) != null;
                return isAlive;
            }
        }

        private CarouselPresenter()
        {
            //disable direct creation use inheritors classes
        }

        protected CarouselPresenter(IScrollElementsDataSource elementsHolder,
            IScrollElementFactory elementsFactory)
        {
            _elementsHolder = elementsHolder;
            _scrollController = new ScrollController(elementsHolder, elementsFactory);
            _scrollController.OnPositionCompensation += PositionCompensation;
            _scrollController.OnStartItemChanged += StartItemChanged;

            TickTackManager.Register(this, true);
        }

        void StartItemChanged(object sender, ScrollControllerChangedEventArgs e)
        {
            startActiveItem = (int) e.newValue;
        }

        void OnPullOnRefresh()
        {
            onPullOnRefresh?.Invoke(_pullOnRefreshStrategy);
        }

        void PositionCompensation(object sender, ScrollControllerChangedEventArgs e)
        {
            var delta = e.newValue - e.oldValue;
            if (viewIsAlive)
            {
                _view.CompensateScrollPosition(delta);
            }
        }

        public void Bind(ICarouselView view)
        {
            if (view == null)
            {
                //CustomLog.LogError("Error: trying to bind null view");
                return;
            }

            if (_view != null && !(ReferenceEquals(_view, view)))
            {
                //CustomLog.LogError($"presenter already binded to view {_view.name} need to unbind it firstly");
                return;
            }

            _view = view;

            _view.onScrollPositionChanged += ScrollPositionChanged;
            _view.onScrollWindowSizeChanged += WindowSizeChanged;
            _view.onUpdate += Update;
            _view.onElementSizeChanged += ElementSizeChanged;

            _pullOnRefreshStrategy = new DefaultPullOnRefreshStrategy(_elementsHolder, _view);
            _pullOnRefreshStrategy.onRefresh += OnPullOnRefresh;
        }

        public void UnBind()
        {
            _view.onScrollPositionChanged -= ScrollPositionChanged;
            _view.onScrollWindowSizeChanged -= WindowSizeChanged;
            _view.onUpdate -= Update;
            _view.onElementSizeChanged -= ElementSizeChanged;

            _pullOnRefreshStrategy.onRefresh -= OnPullOnRefresh;

            _pullOnRefreshStrategy = null;
            _view = null;
        }

        void ElementSizeChanged(ICarouselView view, int index, float newSize)
        {
            _elementsHolder.ElementSizeChanged(index, newSize);
        }

        void Update(ICarouselView view)
        {
            if (!viewIsAlive)
            {
                return;
            }

            view?.SetContentSize(_elementsHolder.totalHolderSize);
        }

        public void Reset(Action completionHandler = null)
        {
            _scrollController.Reset();
            _view?.Reset(completionHandler);
        }

        public void ReloadData()
        {
            var currentWindowSize = _scrollController.WindowSize;
            var currentPosition = _scrollController.Position;

            if (viewIsAlive)
            {
                _view?.SetContentSize(_elementsHolder.totalHolderSize);
            }

            _scrollController.Reset();
            _scrollController.SetWindowSize(currentWindowSize);
            _scrollController.SetPosition(currentPosition);
        }

        public void RefreshCarouselView()
        {
            _lateUpdateActions.Add(() => { _view?.Refresh(); });
        }

        public void InstantScrollToLine(int index, Action onComplete = null)
        {
            if (index < 0)
            {
                //CustomLog.LogError($"index can not be less than zero: index - {index}");
                return;
            }

            var count = _elementsHolder.TotalCount;
            if (index >= count)
            {
                //CustomLog.LogError($"index can not be greater than elements count: index - {index} count: {count}");
                return;
            }

            var elementPosition = _elementsHolder.GetElementPosition(index);
            _view.SetScrollPosition(elementPosition, onComplete);
        }

        public void InstantScrollToItem<T>(Func<T, bool> searchPredicate)
        {
            if (searchPredicate == null)
            {
                //CustomLog.LogError($"can not scroll to null item");
                return;
            }

            if (_elementsHolder is GridDataSource<T> gridDataSource)
            {
                var item = gridDataSource.rawData.FirstOrDefault(searchPredicate.Invoke);
                if (item == null)
                {
                    //CustomLog.LogError($"can not find item: {searchPredicate.Target}");
                    return;
                }

                var rawDataIndex = gridDataSource.rawData.IndexOf(item);
                var index = gridDataSource.GetCarouselLineIndex(rawDataIndex);
                InstantScrollToLine(index);
            }
            else if (_elementsHolder is ListDataSource<T> listDataSource)
            {
                var item = listDataSource.data.FirstOrDefault(searchPredicate.Invoke);
                if (item == null)
                {
                    //CustomLog.LogError($"can not find item: {searchPredicate.Target}");
                    return;
                }

                var index = listDataSource.data.IndexOf(item);
                InstantScrollToLine(index);
            }
            else
            {
                //CustomLog.LogError($"Unexpected data source type: {_elementsHolder.GetType()}");
                return;
            }
        }

        void WindowSizeChanged(ICarouselView view, float newWindowSize)
        {
            if (newWindowSize >= 0)
            {
                _scrollController.SetWindowSize(newWindowSize);
            }
            else
            {
                _scrollController.SetWindowSize(0);
            }
        }

        void ScrollPositionChanged(ICarouselView view, float newPosition)
        {
            _scrollController.SetPosition(newPosition);
            _pullOnRefreshStrategy.Update();
        }

        void ITicktackable.TickTack()
        {
            if (!_lateUpdateActions.Any())
            {
                return;
            }

            foreach (var action in _lateUpdateActions)
            {
                action.Invoke();
            }

            _lateUpdateActions.Clear();
        }
    }
}