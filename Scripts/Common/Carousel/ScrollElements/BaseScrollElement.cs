using Gamla.GUI.Common.Carousel;
using Gamla.Scripts.Common.UI;
using Gamla.Scripts.Common.Carousel.Core;
using UnityEngine;

namespace Gamla.Scripts.Common.Carousel.ScrollElements
{
    public abstract class BaseScrollElement : GUIView, IScrollElement
    {
        public int index => _index;
        public string poolId => _poolId;
        public CanvasGroup gridCellCanvasGroup => _canvasGroup;

        public Transform cachedTransform => _transform == null ? gameObject.transform : _transform;

        #region - State
        bool _inited;
        float _position;
        float _size;
        ICarouselView _containerView;
        int _index;
        //GODestroyCatcher _destroyCatcher;
        string _poolId;
        protected RectTransform _rectTransform;
        Transform _transform;
        #endregion

        protected override void Awake()
        {
            GameObject o;
            _rectTransform = (o = gameObject).GetComponent<RectTransform>();
            _transform = o.transform;
            
            base.Awake();
        }

        public void SetIndex(int index)
        {
            _index = index;
        }

        public void SetPoolId(string poolId)
        {
            _poolId = poolId;
        }
        
        public void OnCreate(ICarouselView containerView, int index)
        {
            _index = index;
            _containerView = containerView;
            _inited = true;
        }

        public virtual void OnDispose()
        {
            _inited = false;
            _index = 0;
            _containerView = null;
            _size = 0;

            ClearResources();
        }
        
        void ClearResources()
        {
            /*if (_destroyCatcher == null) {
                TryGetComponent(out _destroyCatcher);
            }

            if (_destroyCatcher != null) {
                _destroyCatcher.OnPoolRelease();
            }*/
        }

        public float Position
        {
            get => _position;
            set => _position = value;
        }

        protected virtual void LateUpdate()
        {
            if (!_inited)
            {
                return;
            }
            
            UpdateSize();

            UpdatePosition();
        }

        void UpdatePosition()
        {
            var lastPosition = _transform.localPosition;
            Vector3 newPosition;
            if (_containerView.isHorizontal)
            {
                newPosition = new Vector3(_position, _transform.localPosition.y, lastPosition.z);
            }
            else {
                var direction = -_containerView.directionValue;
                newPosition = new Vector3(lastPosition.x, direction * _position, lastPosition.z);
            }

            _transform.localPosition = newPosition;
        }

        void UpdateSize()
        {
            var rect = _rectTransform.rect;
            var newSize = _containerView.isHorizontal ? rect.width : rect.height;

            if (!Mathf.Approximately(newSize, _size))
            {
                _containerView.OnElementSizeChanged(_index, newSize);
            }

            _size = newSize;
        }
        
        public void AddSelfCanvasGroupReference(CanvasGroup canvasGroup)
        {
            _canvasGroup = canvasGroup;
        }
    }
}