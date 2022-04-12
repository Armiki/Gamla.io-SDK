using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Gamla.Logic;

namespace Gamla.UI.Carousel
{
    public class SimpleGridElementFactory : IScrollElementFactory, IPoolObjectInitializer<GridElementsLine>, IPoolObjectInitializer<BaseScrollElement>
    {
        public Dictionary<(int, int), BaseScrollElement> visibleViews => _visibleViews;
        
        const string GridLineResourceId = "GridLineHorizontal";
        const string GridLineResourcePath = "Carousel/";
        
        readonly Dictionary<(int, int), BaseScrollElement> _visibleViews = new Dictionary<(int, int), BaseScrollElement>();
        readonly GridLineParams _gridLineParams;
        readonly ICarouselView _carouselView;
        readonly IGridDataSource _dataSource;

        readonly Action<GUIView, int, int> _postCreate;
        readonly Action<GUIView, int, int> _preDispose;

        readonly Func<int, int, string> _preCreate;


        readonly ObjectPool<GridElementsLine> _lineElementPool;
        readonly Dictionary<string, ObjectPool<BaseScrollElement>> _cellPools = new Dictionary<string, ObjectPool<BaseScrollElement>>();
        readonly Transform _creationPoint;
        readonly Transform _releasedCellObjectContainer;
        readonly float _releasedObjectPosition;

        #region State
        int _lineIndex;
        #endregion
        
        public static SimpleGridElementFactory CreateMultiCell(
            GridLineParams gridLineParams,
            ICarouselView carouselView,
            IGridDataSource dataSource,
            Action<GUIView, int, int> postCreate,
            Func<int, int, string> preCreate,
            Dictionary<string, string> gridsElementsLoadPaths,
            Action<GUIView, int, int> preDispose = null)
        {
            return new SimpleGridElementFactory(
                gridLineParams,
                carouselView,
                dataSource,
                postCreate,
                preCreate,
                gridsElementsLoadPaths,
                preDispose);
        }

        public static SimpleGridElementFactory CreateMonoCell(
            string gridElementLoadPath,
            GridLineParams gridLineParams,
            ICarouselView carouselView,
            IGridDataSource dataSource,
            Action<GUIView, int, int> postCreate,
            Action<GUIView, int, int> preDispose = null)
        {
            var gridsElementsLoadPaths = new Dictionary<string, string>();
            gridsElementsLoadPaths.Add("@default", gridElementLoadPath);
            Func<int, int, string> preCreate = (row, col) => "@default";
            
            return new SimpleGridElementFactory(gridLineParams, carouselView, dataSource, postCreate, preCreate, gridsElementsLoadPaths, preDispose);
        }

         private SimpleGridElementFactory(
            GridLineParams gridLineParams,
            ICarouselView carouselView,
            IGridDataSource dataSource,
            Action<GUIView, int, int> postCreate,
            Func<int, int, string> preCreate,
            Dictionary<string, string> gridsElementsLoadPaths,
            Action<GUIView, int, int> preDispose = null)
        {
            _gridLineParams = gridLineParams;
            _carouselView = carouselView;
            _dataSource = dataSource;
            _postCreate = postCreate;
            _preCreate = preCreate;
            _preDispose = preDispose;
            
            _creationPoint = CreatePoolCreationPoint();
            _releasedCellObjectContainer = CreateCellContainer();
            _releasedObjectPosition = InitReleasedObjectPosition();
            
            //line
            var scrollLineElementPoolFactory = new ObjectPoolFactory<BaseScrollElement>(resourceId: GridLineResourceId,
                resourcePath: GridLineResourcePath, creationPoint: _creationPoint);
            _lineElementPool =
                new ObjectPool<GridElementsLine>(scrollLineElementPoolFactory, scrollLineElementPoolFactory, this);
            
            //cells
            foreach (var cells in gridsElementsLoadPaths) {
                var loadPath = cells.Value;
                var id = cells.Key;
                
                string resourceId = Path.GetFileName(loadPath) ?? "";
                string resourcePath = loadPath.Substring(0, loadPath.Length - resourceId.Length);
                var cellElementPoolFactory = new ObjectPoolFactory<BaseScrollElement>(resourceId: resourceId,
                    resourcePath: resourcePath, creationPoint: _creationPoint);
                
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

        Transform CreateCellContainer()
        {
            var poolObjectContainer = new GameObject("ReleasedCells").transform;
            poolObjectContainer.gameObject.SetActive(false);
            poolObjectContainer.SetParent(_carouselView.itemsRoot, false);

            return poolObjectContainer;
        }

        #region -- IScrollElementFactory
        IScrollElement IScrollElementFactory.CreateElement(int index)
        {
            _lineIndex = index; //save context for pool
            
            var scrollElementLine = _lineElementPool.GetItem();
            InitCells(scrollElementLine, index);
            
            scrollElementLine.OnCreate(_carouselView, index);
#if UNITY_EDITOR || UNITY_DEBUG
            scrollElementLine.gameObject.name = $"line {index}";
#endif

            //CustomLog.LogDebug($"created {scrollElementLine.gameObject.name}");
            return scrollElementLine;
        }

        void IScrollElementFactory.DisposeElement(IScrollElement element)
        {
            var baseScrollElement = (GridElementsLine) element;
            DeInitCells(baseScrollElement, baseScrollElement.index);
            baseScrollElement.OnDispose();
            _lineElementPool.PutItem(baseScrollElement);
            
            //CustomLog.LogDebug($"destroyed {baseScrollElement.gameObject.name}");
        }
        #endregion

        void InitCells(GridElementsLine line, int lineCount)
        {
            int row = lineCount;
            for (int column = 0; column < line.size; column++)
            {
                BaseScrollElement cell = line[column];
#if UNITY_EDITOR || UNITY_DEBUG
                cell.gameObject.name = $"{cell.GetType().Name} cell {row} X {column}";
#endif

                if (_dataSource.HasData(row, column))
                {
                    Enable(cell);
                    _visibleViews.AddOrUpdate((row, column), cell);
                    _postCreate?.Invoke(cell, row, column);
                    //CustomLog.LogDebug($"_postCreate :: {cell.name} :: {row}x{column}");
                }
                else
                {
                    Disable(cell);
                }
            }
        }
        
        void DeInitCells(GridElementsLine line, int lineCount)
        {
            int row = lineCount;
            for (int column = 0; column < line.size; column++)
            {
                BaseScrollElement cell = line[column];
                if (_dataSource.HasData(row, column))
                {
                    _preDispose?.Invoke(cell, row, column);
                    _visibleViews.Remove((row, column));
                    //CustomLog.LogDebug($"_preDispose :: {cell.name} :: {row}x{column}");
                }
            }
        }

        void Enable(BaseScrollElement cell)
        {
            //wee need to save auto layout like in grid view if there is no full line of elements
            //so hide elements without data
            ChangeCanvasAlpha(cell, true);
        }

        void Disable(BaseScrollElement cell)
        {
            //wee need to save auto layout like in grid view if there is no full line of elements
            //so hide elements without data
            ChangeCanvasAlpha(cell, false);
        }

        void ChangeCanvasAlpha(BaseScrollElement cell, bool visible)
        {
            var canvasGroup = cell.gridCellCanvasGroup;
            bool allreadySet = canvasGroup.interactable == visible;
            if (allreadySet) {
                return;
            }
            
            canvasGroup.alpha = visible ? 1 : 0;
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;
        }

        #region -- Grid Line LifeCycle
        void IPoolObjectInitializer<GridElementsLine>.UseObject(GridElementsLine gridLine, bool newObject)
        {
            if (newObject) {
                HideElementBehindScreen(gridLine);
            }
            
            UseObject(gridLine, _lineIndex);
        }

        void UseObject(GridElementsLine gridLine, int lineIndex)
        {
#if UNITY_EDITOR || UNITY_DEBUG
            gridLine.gameObject.name = gridLine.gameObject.name.Replace("_released", string.Empty);
#endif
            gridLine.horizontalSpacing = _gridLineParams.horizontalSpacing;
            gridLine.verticalSpacing = _gridLineParams.verticalSpacing;
            
            for (int column = 0; column < _dataSource.columnsCount; column++) {
                var poolId = _cellPools.FirstOrDefault().Key;
                if (_dataSource.HasData(lineIndex, column)) {
                    poolId = _preCreate?.Invoke(lineIndex, column) ?? string.Empty;
                }
                if (!_cellPools.TryGetValue(poolId, out ObjectPool<BaseScrollElement> pool)) {
                    //CustomLog.LogError($"can not find pool for cells with id {poolId}");
                    return;
                }
                
                var cell = pool.GetItem();
                cell.SetPoolId(poolId);
                gridLine.AddCell(cell);
            }
            gridLine.SetEnableDynamicHeight(true);
        }

        void IPoolObjectInitializer<GridElementsLine>.ReleaseObject(GridElementsLine gridLine)
        {
#if UNITY_EDITOR || UNITY_DEBUG
            gridLine.gameObject.name += "_released";
#endif

            HideElementBehindScreen(gridLine);
            
            gridLine.DisposeCells();
            PutLineElementsToPoolReversed(gridLine);
            gridLine.ClearCells();
            gridLine.SetEnableDynamicHeight(false);
        }
        
        void HideElementBehindScreen(BaseScrollElement objectRef)
        {
            var position = objectRef.cachedTransform.localPosition;
            position = new Vector3(position.x, _releasedObjectPosition, position.z);
            objectRef.cachedTransform.localPosition = position;
        }
        
        void PutLineElementsToPoolReversed(GridElementsLine gridLine)
        {
            for (int i = gridLine.size - 1; i >= 0 ; i--) {
                var cell = gridLine[i];
                var pool = _cellPools[cell.poolId];
                pool.PutItem(cell);
            }
        }
        #endregion

        #region -- Cell LifeCycle
        void IPoolObjectInitializer<BaseScrollElement>.UseObject(BaseScrollElement cell, bool newObject)
        {
#if UNITY_EDITOR || UNITY_DEBUG
            cell.gameObject.name = cell.gameObject.name.Replace("_released_cell", string.Empty);
#endif
        }

        void IPoolObjectInitializer<BaseScrollElement>.ReleaseObject(BaseScrollElement cell)
        {
#if UNITY_EDITOR || UNITY_DEBUG
            cell.gameObject.name += "_released_cell";
#endif
            cell.gameObject.transform.SetParent(_releasedCellObjectContainer, false);
        }
        #endregion
    }
}