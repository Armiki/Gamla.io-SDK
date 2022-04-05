using System;
using System.Collections.Generic;
using System.IO;
using Gamla.GUI.Common.Carousel;
using Gamla.Scripts.Common.Carousel.Core;
using Gamla.Scripts.Common.Carousel.ScrollElements;
using Gamla.Scripts.Common.GeneralPool;
using UnityEngine;
using Gamla.Scripts.Logic;

namespace Gamla.Scripts.Common.Carousel.ScrollElementsFactories
{
    public class SimpleScrollElementFactory : IScrollElementFactory, IPoolObjectInitializer<BaseScrollElement>
    {
        public IReadOnlyDictionary<int, BaseScrollElement> visibleViews => _visibleViews;
        
        readonly Dictionary<int, BaseScrollElement> _visibleViews = new Dictionary<int, BaseScrollElement>();
        readonly ICarouselView _carouselView;
        readonly Action<BaseScrollElement, int> _postCreate;
        readonly Action<BaseScrollElement, int> _preDispose;
        readonly Dictionary<string, ObjectPool<BaseScrollElement>> _cellPools = new Dictionary<string, ObjectPool<BaseScrollElement>>();
        readonly Transform _poolObjectContainer;
        readonly float _releasedObjectPosition;
        readonly Func<int, string> _preCreate;

        public static SimpleScrollElementFactory CreateMonoCell(string scrollElementLoadPath,
            ICarouselView carouselView, 
            Action<BaseScrollElement, int> postCreate, 
            Action<BaseScrollElement, int> preDispose = null)
        {
            var scrollElementLoadPaths = new Dictionary<string, string>();
            scrollElementLoadPaths.Add("@default", scrollElementLoadPath);
            Func<int, string> preCreate = (index) => "@default";
            
            return new SimpleScrollElementFactory(
                scrollElementLoadPaths,
                preCreate,
                carouselView,
                postCreate,
                preDispose);
        }
        
        public static SimpleScrollElementFactory CreateMultiCell(
            Dictionary<string, string> scrollElementLoadPaths,
            Func<int, string> preCreate,
            ICarouselView carouselView, 
            Action<BaseScrollElement, int> postCreate, 
            Action<BaseScrollElement, int> preDispose = null)
        {
            return new SimpleScrollElementFactory(
                scrollElementLoadPaths,
                preCreate,
                carouselView,
                postCreate,
                preDispose);
        }

        private SimpleScrollElementFactory(
            Dictionary<string, string> scrollElementLoadPaths,
            Func<int, string> preCreate,
            ICarouselView carouselView, 
            Action<BaseScrollElement, int> postCreate, 
            Action<BaseScrollElement, int> preDispose = null)
        {
            _carouselView = carouselView;
            _postCreate = postCreate;
            _preDispose = preDispose;
            _preCreate = preCreate;

            _releasedObjectPosition = InitReleasedObjectPosition();
            _poolObjectContainer = CreatePoolCreationPoint();

            foreach (var cell in scrollElementLoadPaths) {
                var loadPath = cell.Value;
                var id = cell.Key;
                
                string resourceId = Path.GetFileName(loadPath) ?? "";
                string resourcePath = loadPath.Substring(0, loadPath.Length - resourceId.Length);
                var cellElementPoolFactory = new ObjectPoolFactory<BaseScrollElement>(resourceId: resourceId,
                    resourcePath: resourcePath, creationPoint: _poolObjectContainer);
                var cellElementPool =
                    new ObjectPool<BaseScrollElement>(cellElementPoolFactory, cellElementPoolFactory, this);
                _cellPools.Add(id, cellElementPool);
            }
        }

        float InitReleasedObjectPosition()
        {
            var canvas = GamlaResourceManager.canvas;
            var pointOutSideOfScreen = _carouselView.itemsRoot.position.y - 2 * (Mathf.Max(Screen.height, Screen.width) / canvas.scaleFactor);
            return pointOutSideOfScreen;
        }

        Transform CreatePoolCreationPoint()
        {
            return _carouselView.itemsRoot;
        }

        public IScrollElement CreateElement(int index)
        {
            var poolId = _preCreate?.Invoke(index);
            if (!_cellPools.TryGetValue(poolId, out ObjectPool<BaseScrollElement> pool)) {
                //CustomLog.LogError($"can not find pool for cells with id {poolId}");
                return null;
            }
            
            var scrollElement = pool.GetItem();
            scrollElement.SetPoolId(poolId);
            scrollElement.OnCreate(_carouselView, index);
#if UNITY_EDITOR || UNITY_DEBUG
            scrollElement.gameObject.name = $"ScrollElement {index}";
#endif
            _visibleViews.AddOrUpdate(index, scrollElement);
            _postCreate?.Invoke(scrollElement, index);
            //CustomLog.LogDebug($"$ _postCreate {scrollElement.name} :: {index}");
            //CustomLog.LogDebug($"created {scrollElement.gameObject.name}");

            return scrollElement;
        }

        public void DisposeElement(IScrollElement element)
        {
            var baseScrollElement = (BaseScrollElement) element;
            _preDispose?.Invoke(baseScrollElement, baseScrollElement.index);
            _visibleViews.Remove(baseScrollElement.index);
            //CustomLog.LogDebug($"$ _preDispose {baseScrollElement.name} :: {baseScrollElement.index}");

            baseScrollElement.OnDispose();
            var pool = _cellPools[baseScrollElement.poolId];
            pool.PutItem(baseScrollElement);

            //CustomLog.LogDebug($"destroyed {baseScrollElement.gameObject.name}");
        }

        public void UseObject(BaseScrollElement objectRef, bool newObject)
        {
#if UNITY_EDITOR || UNITY_DEBUG
            objectRef.gameObject.name = objectRef.gameObject.name.Replace("_released", string.Empty);
#endif
            if (newObject) {
                HideElementBehindScreen(objectRef);
            }
        }

        public void ReleaseObject(BaseScrollElement objectRef)
        {
#if UNITY_EDITOR || UNITY_DEBUG
            objectRef.gameObject.name += "_released";
#endif
            
            HideElementBehindScreen(objectRef);
        }

        void HideElementBehindScreen(BaseScrollElement objectRef)
        {
            var position = objectRef.cachedTransform.localPosition;
            position = new Vector3(position.x, _releasedObjectPosition, position.z);
            objectRef.cachedTransform.localPosition = position;
        }
    }
}