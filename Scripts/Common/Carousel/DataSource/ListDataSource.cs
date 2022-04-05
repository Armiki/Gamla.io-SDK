using System.Collections.Generic;
using System.Collections.ObjectModel;
using Gamla.Scripts.Common.Carousel.Core;

namespace Gamla.Scripts.Common.Carousel.DataSource
{
    public class ListDataSource<T> : SimpleDataSource
    {
        public ReadOnlyCollection<T> data => _data.AsReadOnly();
        
        readonly List<T> _data;
        readonly IUpdatableScrollElementsHolder _elementsHolder;
        protected override IScrollElementsHolder internalHolder => _elementsHolder;
        
        public ListDataSource(List<T> data, IUpdatableScrollElementsHolder elementsHolder)
        {
            _data = data;
            _elementsHolder = elementsHolder;
        }
        
        public ListDataSource(IUpdatableScrollElementsHolder elementsHolder)
        {
            _data = new List<T>();
            _elementsHolder = elementsHolder;
        }
        
        public void InsertData(T data, int index)
        {
            _data.Insert(index, data);
            _elementsHolder.Insert(index);
        }
        
        public void RemoveData(int index)
        {
            if (_data.Count < index + 1) {
                //CustomLog.LogError($"there is no data at index {index}");
                return;
            }
            
            _data.RemoveAt(index);
            _elementsHolder.Remove(index);
        }
        
        public void AddLast(T data)
        {
            _data.Add(data);
            _elementsHolder.AddLast();
        }
        
        public void RemoveLast()
        {
            if (_data.Count <= 0) {
                //CustomLog.LogError($"there is no data");
                return;
            }
            
            _data.RemoveAt(_data.Count - 1);
            _elementsHolder.RemoveLast();
        }

        public T GetData(int index)
        {
            return _data[index];
        }
        
        public void UpdateData(List<T> newData, float? newExpectedSizeOfElement = null)
        {
            _data.Clear();
            if (newExpectedSizeOfElement == null) {
                _elementsHolder.Clear();
            } else {
                _elementsHolder.ClearAndUpdateSettings(newExpectedSizeOfElement.Value);
            }
            foreach (var data in newData)
            {
                AddLast(data);    
            }
        }

        public bool HasData(int index)
        {
            if (index >= _data.Count)
            {
                return false;
            }

            return _data[index] != null;
        }

        public override void ElementSizeChanged(int index, float newSize)
        {
            _elementsHolder.UpdateSize(newSize, index);
        }
    }
}