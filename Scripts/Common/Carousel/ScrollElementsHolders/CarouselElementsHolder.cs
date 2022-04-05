using System;
using System.Collections.Generic;
using Gamla.Scripts.Common.Carousel.Core;

namespace Gamla.Scripts.Common.Carousel.ScrollElementsHolders
{
    public class CarouselElementsHolder : IScrollElementsHolder
    {
        public event EventHandler< SizeChangedEventArgs > OnElementSizeChanged = delegate( object sender, SizeChangedEventArgs args ) {  };
        public event EventHandler< ElementsHolderChangedArgs > OnHolderChanged = delegate( object sender, ElementsHolderChangedArgs args ) {  };

        protected List< CarouselElement > _carouselElements;
        protected float _totalContentSize;
        
        public CarouselElementsHolder()
        {
            _carouselElements = new List< CarouselElement >();
        }

        public void AddElement( float size  )
        {
            var carouselElement = new CarouselElement( _totalContentSize, size );
            _carouselElements.Add( carouselElement );
            _totalContentSize += size;            
        }

        public void Clear()
        {
            _totalContentSize = 0;
            _carouselElements.Clear();
        }

        public int TotalCount => _carouselElements.Count;
        public float GetElementSize( int index )
        {
            return _carouselElements[ index ].Size;
        }

        public float GetElementPosition( int index )
        {
            return _carouselElements[ index ].Position;
        }

        public int FindElementOnPosition( float position )
        {
            return Math.Max( 0, _carouselElements.FindLastIndex( e => e.Position <= position ) );
        }        
    }
}