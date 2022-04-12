using System.Collections.Generic;
using Gamla.Logic;

namespace Gamla.UI.Carousel
{
    public sealed class ListCarouselPresenter : CarouselPresenter
    {
        public IReadOnlyDictionary<int, BaseScrollElement> visibleViews => _elementsFactory.visibleViews;


        readonly SimpleScrollElementFactory _elementsFactory;

        public ListCarouselPresenter(IScrollElementsDataSource elementsHolder,
            SimpleScrollElementFactory elementsFactory) : base(elementsHolder, elementsFactory)
        {
            _elementsFactory = elementsFactory;
        }
    }
}