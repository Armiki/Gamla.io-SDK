using System.Collections.Generic;
using Gamla.Logic;

namespace Gamla.UI.Carousel
{
    public sealed class GridCarouselPresenter : CarouselPresenter
    {
        /// <summary>
        /// row x col
        /// </summary>
        public IReadOnlyDictionary<(int, int), BaseScrollElement> visibleViews => _elementsFactory.visibleViews;


        readonly SimpleGridElementFactory _elementsFactory;

        public GridCarouselPresenter(IScrollElementsDataSource elementsHolder, SimpleGridElementFactory elementsFactory)
            : base(elementsHolder, elementsFactory)
        {
            _elementsFactory = elementsFactory;
        }
    }
}