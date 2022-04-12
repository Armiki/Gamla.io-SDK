using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gamla.UI.Carousel
{
    public class CarouselDynamicSizeElementsHolder : IUpdatableScrollElementsHolder
    {
        float _expectedSize;
        readonly float _spacing;

        public event EventHandler<SizeChangedEventArgs> OnElementSizeChanged =
            delegate(object sender, SizeChangedEventArgs args) { };

        public event EventHandler<ElementsHolderChangedArgs> OnHolderChanged =
            delegate(object sender, ElementsHolderChangedArgs args) { };

        #region State
        readonly List<CarouselElement> _carouselElements;
        #endregion

        public CarouselDynamicSizeElementsHolder(float expectedSize, int count = default, float spacing = 0)
        {
            _expectedSize = expectedSize;
            _spacing = spacing;
            _carouselElements = new List<CarouselElement>(capacity: count);
            for (int i = 0; i < count; i++)
            {
                AddLastInternal(expectedSize);
            }
        }
        
        public void ClearAndUpdateSettings(float newExpectedSizeOfElement)
        {
            Clear();
            _expectedSize = newExpectedSizeOfElement;
        }

        public void Clear()
        {
            _carouselElements.Clear();
        }

        public int TotalCount => _carouselElements.Count;

        public float GetElementSize(int index)
        {
            if (_carouselElements.Count < index + 1) {
                return 0;
            }
            
            return _carouselElements[index].Size;
        }

        public float GetElementPosition(int index)
        {
            if (_carouselElements.Count < index + 1) {
                return 0;
            }

            return _carouselElements[index].Position;
        }

        public int FindElementOnPosition(float position)
        {
            return Math.Max(0, _carouselElements.FindLastIndex(e => e.Position <= position));
        }

        public void UpdateSize(float newSize, int index)
        {
            if (_carouselElements.Count < index + 1) {
                //CustomLog.LogError($"there is no element at index {index}");
                return;
            }
            
            UpdateSizeInternal(index, newSize);
        }

        public void AddLast()
        {
            AddLastInternal(_expectedSize);
        }

        public void RemoveLast()
        {
            RemoveLastInternal();
        }

        public void Insert(int index)
        {
            float newElementSize = _expectedSize;
            InsertInternal(index, newElementSize);
        }

        public void Remove(int index)
        {
            if (_carouselElements.Count < index + 1) {
               //CustomLog.LogError($"there is no element at index {index}");
               return;
            }
            
            RemoveInternal(index);
        }

        void UpdateSizeInternal(int index, float newSize)
        {
            float oldSize = _carouselElements[index].Size;
            
            if (Mathf.Approximately(oldSize, newSize))
            {
                //no need to change size
                return;
            }
            
            float offsetPosition = newSize - oldSize;

            //update size
            _carouselElements[index].Size = newSize;

            //shift all elements after
            for (int i = index + 1; i < _carouselElements.Count; i++)
            {
                _carouselElements[i].Position += offsetPosition;
            }

            OnElementSizeChanged?.Invoke(this, new SizeChangedEventArgs(index, oldSize, newSize));
        }

        void InsertInternal(int index, float newElementSize)
        {
            float oldElementPosition = _carouselElements[index].Position;
            float offsetPosition = newElementSize + _spacing;

            //update holder
            _carouselElements.Insert(index, new CarouselElement(oldElementPosition, newElementSize));

            //shift position all elements after new element
            for (int i = index + 1; i < _carouselElements.Count; i++)
            {
                _carouselElements[i].Position += offsetPosition;
            }
            
            OnHolderChanged?.Invoke(this, new ElementsHolderChangedArgs(index, newElementSize + _spacing, ElementChangeReason.Added));
        }

        void RemoveInternal(int index)
        {
            float oldElementSize = _carouselElements[index].Size;
            float offsetPosition = -oldElementSize - _spacing;

            //update holder
            _carouselElements.RemoveAt(index);

            //shift position all elements after deleted element
            for (int i = index; i < _carouselElements.Count; i++)
            {
                _carouselElements[i].Position += offsetPosition;
            }
            
            OnHolderChanged?.Invoke(this, new ElementsHolderChangedArgs(index, oldElementSize + _spacing, ElementChangeReason.Removed));
        }

        void RemoveLastInternal()
        {
            if (_carouselElements.Count > 0)
            {
                _carouselElements.RemoveAt(_carouselElements.Count - 1);
                
                OnHolderChanged?.Invoke(this, new ElementsHolderChangedArgs(_carouselElements.Count - 1, _expectedSize, ElementChangeReason.Removed));
            }
        }

        void AddLastInternal(float size)
        {
            float position = 0;
            if (_carouselElements.Count > 0)
            {
                var currentLastElement = _carouselElements[_carouselElements.Count - 1];
                position = currentLastElement.Position + currentLastElement.Size + _spacing;
            }

            var carouselElement = new CarouselElement(position, size);
            _carouselElements.Add(carouselElement);
            
            OnHolderChanged?.Invoke(this, new ElementsHolderChangedArgs(_carouselElements.Count - 1, _expectedSize, ElementChangeReason.Added));
        }
    }
}