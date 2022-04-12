using System;
using UnityEngine;

namespace Gamla.UI.Carousel
{
    public class CarouselEqualSizeElementsHolder : IUpdatableScrollElementsHolder
    {
        #region State
        float _elementSize;
        int _elementsCount;
        float _spacing;
        #endregion

        public event EventHandler<SizeChangedEventArgs> OnElementSizeChanged =
            delegate(object sender, SizeChangedEventArgs args) { };

        public event EventHandler<ElementsHolderChangedArgs> OnHolderChanged =
            delegate(object sender, ElementsHolderChangedArgs args) { };

        public CarouselEqualSizeElementsHolder(float estimatedSize, int elementsCount = default, float spacing = 0)
        {
            _elementSize = estimatedSize;
            _elementsCount = elementsCount;
            _spacing = spacing;
        }

        void UpdateSizeInternal(float newSize)
        {
           if (Mathf.Approximately(_elementSize, newSize))
           {
               //no need to change size
              return;
           }

            var oldSize = _elementSize;
            _elementSize = newSize;

            for (int i = 0; i < _elementsCount; i++)
            {
                OnElementSizeChanged?.Invoke(this, new SizeChangedEventArgs(i, oldSize, newSize));
            }
        }

        void AddInternal(int index)
        {
            _elementsCount += 1;
            OnHolderChanged?.Invoke(this, new ElementsHolderChangedArgs(index, _elementSize + _spacing, ElementChangeReason.Added));
        }

        void RemoveInternal(int index)
        {
            if (_elementsCount <= 0) {
                //CustomLog.LogError($"there is no elements - cant remove");
                return;
            }
            
            _elementsCount -= 1;
            OnHolderChanged?.Invoke(this, new ElementsHolderChangedArgs(index, 0, ElementChangeReason.Removed));
        }

        public void UpdateSize(float newSize, int index = default)
        {
            //since all size must be equals in this holder
            UpdateSizeInternal(newSize);
        }

        public void AddLast()
        {
            AddInternal(_elementsCount);
        }

        public void RemoveLast()
        {
            RemoveInternal(_elementsCount - 1);
        }

        public void Insert(int index)
        {
            AddInternal(index);
        }

        public void Remove(int index)
        {
            RemoveInternal(index);
        }

        public void ClearAndUpdateSettings(float newExpectedSizeOfElement)
        {
            Clear();
            _elementSize = newExpectedSizeOfElement;
        }

        public void SetCount(int elementsCount)
        {
            _elementsCount = elementsCount;
        }

        public void Clear() => SetCount(0);
        public int TotalCount => _elementsCount;

        public float GetElementSize(int index)
        {
            return _elementSize;
        }

        public float GetElementPosition(int index)
        { 
            return Math.Max(0, index * (_elementSize + _spacing));
        } 

        public int FindElementOnPosition(float position)
        {
            var index = Math.Min(_elementsCount - 1, (int) (position / (_elementSize + _spacing)));
            return Math.Max(0, index);
        }
    }
}