using System;
using System.Collections;
using Gamla.Logic;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.UI.Carousel
{
    public class ScrollRectCarouselView : GUIView, ICarouselView
    {
#pragma warning restore 1591
        [SerializeField] Direction _direction = Direction.Normal;
        [SerializeField] CarouselScrollRect _scrollRect;
        public event Action<ICarouselView, float> onScrollPositionChanged;
        public event Action<ICarouselView, float> onScrollWindowSizeChanged;
        public event Action<ICarouselView> onUpdate;
        public event Action<ICarouselView, int, float> onElementSizeChanged;

        public float normalizedScrollPosition => _horizontal ? _scrollRect.horizontalNormalizedPosition : _scrollRect.verticalNormalizedPosition;
        public bool isHorizontal => _horizontal;
        public Transform itemsRoot => _scrollRect.viewport;
        public CarouselPresenter carouselPresenter => _carouselPresenter;
        public CarouselScrollRect rawScrollRect => _scrollRect;
        public float windowSize => _cachedWindowSize;

        public int directionValue => (int) _direction;

        float scrollItemsInternalWindowSize => _cachedItemsInternalWindowSize;

        float currentScrollPosition
        {
            get
            {
                var contentPosition = _scrollRect.content.anchoredPosition;
                float carouselPosition = _horizontal ? -contentPosition.x : contentPosition.y;

                return carouselPosition;
            }
            set
            {
                var content = _scrollRect.content;
                var currentContentPosition = content.anchoredPosition;

                content.anchoredPosition = _horizontal
                    ? new Vector2(-value, currentContentPosition.y)
                    : new Vector2(currentContentPosition.x, value);
            }
        }

        float contentSize
        {
            get => _cachedContentSize;
            set
            {
                var newValue = Mathf.Max(value, scrollItemsInternalWindowSize);
                //check same size set
                if (Mathf.Approximately(_cachedContentSize, newValue)) {
                    return;
                }
                
                _cachedContentSize = newValue;
                _scrollRect.content.sizeDelta = _horizontal
                    ? new Vector2(newValue, 0)
                    : new Vector2(0, newValue);
            }
        }

        #region - State
        float _cachedContentSize;
        float _cachedWindowSize;
        float _cachedItemsInternalWindowSize;

        bool _horizontal;
        float? _compensationAccumulator;
        CarouselPresenter _carouselPresenter;
        
        //set position
        ICoroutineState _setScrollPositionCoroutine;
        ScrollRect.MovementType _setScrollPositionOriginalMovementType;
        
        //init process 
        ICoroutineState _initProcess;
        float _originAlpha;
        bool _originInertia;

        //window components
        RectTransform _rectTransform;
        CanvasGroup _itemsRootCanvasGroup;
        RectTransform _itemsRootRectTransform;

        //Reset
        Action _resetComplete;
        #endregion

        #region -- ICarouselView
        void ICarouselView.SetContentSize(float newContentSize)
        {
            contentSize = newContentSize;
        }
        
        void ICarouselView.Reset(Action completionHandler = null)
        {
            _resetComplete = completionHandler;
            currentScrollPosition = 0;
            StartInitialisationProcess();
            //UpdateScrollPosition();
        }

        void ICarouselView.Refresh()
        {
            UpdateScrollPosition();
        }
        

        void ICarouselView.SetScrollPosition(float position, Action onComplete)
        {
            var newPosition = directionValue * position;
            if (_setScrollPositionCoroutine != null) {
                _setScrollPositionCoroutine.Cancel();
                _setScrollPositionCoroutine = null;
            } else {
                //set original only when no other active SetScrollPositionHelperCoroutines
                _setScrollPositionOriginalMovementType = _scrollRect.movementType;
            }
            
            _scrollRect.movementType = ScrollRect.MovementType.Clamped; //prevent glitching while moving
            currentScrollPosition = newPosition;
            UpdateScrollPosition();
            _setScrollPositionCoroutine = HomeThreadHelper.homeThread.ExecuteCoroutine(SetScrollPositionHelperCoroutine(_setScrollPositionOriginalMovementType, onComplete));
        }

        IEnumerator SetScrollPositionHelperCoroutine(ScrollRect.MovementType movementType, Action onComplete)
        {
            yield return new WaitForEndOfFrame();
            _scrollRect.movementType = movementType;
            yield return new WaitForEndOfFrame();
            onComplete?.Invoke();
        }

        void ICarouselView.OnElementSizeChanged(int index, float newSize)
        {
            //CustomLog.LogDebug($"size of element {index} changed to {newSize}");
            onElementSizeChanged?.Invoke(this, index, newSize);
        }

        void ICarouselView.CompensateScrollPosition(float compensationDelta)
        {
            if (_compensationAccumulator == null)
            {
                _compensationAccumulator = compensationDelta;
            }
            else
            {
                _compensationAccumulator += compensationDelta;
            }
        }
#pragma warning disable 1591
        #endregion

        #region - LifeCycle
        public void Bind(CarouselPresenter carouselPresenter)
        {
            if (carouselPresenter == null) {
                //CustomLog.LogError("Error: trying to bind null presenter");
                return;
            }
            
            if (_carouselPresenter != null && !(ReferenceEquals(carouselPresenter, _carouselPresenter)))
            {
                //CustomLog.LogError($"view already binded to presenter {carouselPresenter.GetType().Name}, need to unbind it firstly");
                return;
            }

            _carouselPresenter = carouselPresenter;
        }
        
        public void UnBind()
        {
            _carouselPresenter = null;
        }
        
        protected override void Awake()
        {
            base.Awake();
            _scrollRect.onValueChanged.AddListener(UpdateScrollPositionFromEngineEvent);

            if (_scrollRect.horizontal && _scrollRect.vertical)
            {
                throw new Exception("ScrollRect component for carousel can be either horizontal or vertical");
            }

            _horizontal = _scrollRect.horizontal;
            _itemsRootRectTransform = itemsRoot.GetComponent<RectTransform>();
            _rectTransform = GetComponent<RectTransform>();

            InitCanvasGroup();
        }

        void Update()
        {
            onUpdate?.Invoke(this);
            //update overall window
            var carouselRootRect = _rectTransform.rect;
            _cachedWindowSize = _horizontal ? carouselRootRect.width : carouselRootRect.height;
            
            //updateInternalWindow
            var itemsRootRect = _itemsRootRectTransform.rect;
            _cachedItemsInternalWindowSize =
                _horizontal ? itemsRootRect.width : itemsRootRect.height;
                
            UpdateWindowSize();
        }

        void LateUpdate()
        {
            if (_compensationAccumulator != null) {
                var compensation = directionValue * _compensationAccumulator.Value;
                CompensationScroll(compensation);
                _compensationAccumulator = null;
            }
        }

        public override void OnDestroy()
        {
            onScrollPositionChanged = null;
            onScrollWindowSizeChanged = null;
            _scrollRect.onValueChanged.RemoveAllListeners();
            base.OnDestroy();
        }
        #endregion


        void UpdateWindowSize()
        {
            onScrollWindowSizeChanged?.Invoke(this, scrollItemsInternalWindowSize);
        }
        
        void UpdateScrollPositionFromEngineEvent(Vector2 newValue = default)
        {
            var position = directionValue * currentScrollPosition;
            onScrollPositionChanged?.Invoke(this, position);
            //moved to StartInitialisationProcess
            //_resetComplete?.Invoke();
            //_resetComplete = null;
        }

        void UpdateScrollPosition(Vector2 newValue = default)
        {
            var position = directionValue * currentScrollPosition;
            onScrollPositionChanged?.Invoke(this, position);
        }

        void CompensationScroll(float compensationDelta)
        {
            //if (scrollItemsInternalWindowSize >= contentSize)
            {
                return;
            }
            
            var normalizedDiff = compensationDelta / (contentSize - scrollItemsInternalWindowSize);

            if (_horizontal)
            {
                var newNormalizedPosition = _scrollRect.horizontalNormalizedPosition - normalizedDiff;
                _scrollRect.CustomSetHorizontalNormalizedPosition(newNormalizedPosition);
            }
            else
            {
                var newNormalizedPosition = _scrollRect.verticalNormalizedPosition - normalizedDiff;
                _scrollRect.CustomSetVerticalNormalizedPosition(newNormalizedPosition);
            }
        }
        
        void InitCanvasGroup()
        {
            _itemsRootCanvasGroup = itemsRoot.GetComponent<CanvasGroup>();
            if (_itemsRootCanvasGroup == null)
            {
                _itemsRootCanvasGroup = itemsRoot.gameObject.AddComponent<CanvasGroup>();
            }
        }

        /// <summary>
        /// Ждем пару кадров, чтобы рассчитался реальный размер ячеек в карусели
        /// иначе будет заметен глитч.
        /// </summary>
        void StartInitialisationProcess()
        {
            if (_itemsRootCanvasGroup == null) {
                InitCanvasGroup();
            }
            
            if (_initProcess != null && !_initProcess.canceled) {
                //CustomLog.LogWarning("init process already started, cancelling and start again");
                _initProcess?.Cancel();
            } else {
                _originAlpha = _itemsRootCanvasGroup.alpha;
                _originInertia = _scrollRect.inertia;
            }

            _scrollRect.inertia = false;
            _itemsRootCanvasGroup.alpha = 0;

            _initProcess =
                HomeThreadHelper.homeThread.ExecuteCoroutine(
                    WaitSeveralFramesAndMakeVisable(_originAlpha, _originInertia));
        }

        IEnumerator WaitSeveralFramesAndMakeVisable(float originAlpha, bool originInertia)
        {
            //ждем пока карусель подстроится под размер ячеек
            yield return null;
            yield return new WaitForEndOfFrame();
            yield return null;
            yield return new WaitForEndOfFrame();

            if (_scrollRect == null) {
                yield break;
            }
            
            UpdateScrollPosition();
            _itemsRootCanvasGroup.alpha = originAlpha;
            _scrollRect.inertia = originInertia;
            _initProcess = null;
            _resetComplete?.Invoke();
            _resetComplete = null;
        }
    }
}