using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Gamla.UI.Carousel
{
    public class GridDataSource<T> : SimpleDataSource, IGridDataSource
    {
        public ReadOnlyCollection<T> rawData => _data.AsReadOnly();
        public int columnsCount => _columnsCount;
        
        #region - State
        List<T> _data;
        #endregion

        readonly CarouselEqualSizeElementsHolder _elementsHolder;

        int linesCount
        {
            get
            {
                int lastElementIndex = (_data.Count - 1);
                int lastElementRowIndex = lastElementIndex / _columnsCount;

                return lastElementRowIndex + 1;
            }
        }

        #region State
        int _columnsCount;
        #endregion

        protected override IScrollElementsHolder internalHolder => _elementsHolder;

        public GridDataSource(List<T> data, float estimatedLineHeight, int columnsCount = 1)
        {
            _data = data;
            _elementsHolder = new CarouselEqualSizeElementsHolder(estimatedLineHeight, data.Count);

            _columnsCount = columnsCount;
            UpdateHolderCount();
        }

        public GridDataSource(float estimatedLineHeight, int columnsCount = 1)
        {
            _data = new List<T>();
            _elementsHolder = new CarouselEqualSizeElementsHolder(estimatedLineHeight, 0);

            _columnsCount = columnsCount;
            UpdateHolderCount();
        }

        public override void ElementSizeChanged(int index, float newSize)
        {
            _elementsHolder.UpdateSize(newSize, index);
        }

        public void UpdateData(List<T> newData)
        {
            _data = newData;
            UpdateHolderCount();
        }

        //public void SetMaxColumnsCount(int maxColumns)
        //{
        //    _maxColumnsCount = maxColumns;
        //    UpdateHolderCount();
        //}

        public int GetCarouselLineIndex(int rawDataIndex)
        {
            return (int) rawDataIndex / _columnsCount;
        }

        public bool HasData(int row, int column)
        {
            int index = row * _columnsCount + column;
            return index <= _data.Count - 1;
        }

        public T GetData(int row, int column)
        {
            int index = row * _columnsCount + column;
            return _data[index];
        }

        public void InsertData(T data, int index)
        {
            _data.Insert(index, data);
            UpdateHolderCount();
        }

        public void RemoveData(int index)
        {
            if (_data.Count < index + 1) {
                //CustomLog.LogError($"there is no data at index {index}");
                return;
            }
            
            _data.RemoveAt(index);
            UpdateHolderCount();
        }

        public void AddLast(T data)
        {
            _data.Add(data);
            UpdateHolderCount();
        }

        public void RemoveLast()
        {
            if (_data.Count <= 0) {
                //CustomLog.LogError($"there is no data");
                return;
            }

            _data.RemoveAt(_data.Count - 1);
            UpdateHolderCount();
        }

        void UpdateHolderCount()
        {
            _elementsHolder.SetCount(linesCount);
        }
    }
}