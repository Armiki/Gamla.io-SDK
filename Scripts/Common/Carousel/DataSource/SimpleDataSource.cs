using System;

namespace Gamla.UI.Carousel
{
    public abstract class SimpleDataSource : IScrollElementsDataSource
    {
        protected abstract IScrollElementsHolder internalHolder { get; }
        public abstract void ElementSizeChanged(int index, float newSize);

        #region IScrollElementsDataSource
        public event EventHandler<SizeChangedEventArgs> OnElementSizeChanged
        {
            add => internalHolder.OnElementSizeChanged += value;
            remove => internalHolder.OnElementSizeChanged -= value;
        }

        public event EventHandler<ElementsHolderChangedArgs> OnHolderChanged
        {
            add => internalHolder.OnHolderChanged += value;
            remove => internalHolder.OnHolderChanged -= value;
        }

        public int TotalCount => internalHolder.TotalCount;

        public float GetElementSize(int index)
        {
            return internalHolder.GetElementSize(index);
        }

        public float GetElementPosition(int index)
        {
            return internalHolder.GetElementPosition(index);
        }

        public int FindElementOnPosition(float position)
        {
            return internalHolder.FindElementOnPosition(position);
        }

        public void Clear()
        {
            internalHolder.Clear();
        }
        
        public float totalHolderSize
        {
            get
            {
                var totalCount = internalHolder.TotalCount;
                if (totalCount == 0) {
                    return 0;
                }
                
                var lastElementIndex = totalCount - 1;
                var totalContentSize = internalHolder.GetElementPosition(lastElementIndex) +
                                       internalHolder.GetElementSize(lastElementIndex);

                return totalContentSize;
            }
        }
        #endregion
    }
}