using System.Collections.Generic;
using Gamla.Scripts.Common.Carousel.DataSource;
using Gamla.Scripts.Common.Carousel.ScrollElements;
using Gamla.Scripts.Common.Carousel.ScrollElementsFactories;

public sealed class ListCarouselPresenter : CarouselPresenter
{
    public IReadOnlyDictionary<int, BaseScrollElement> visibleViews => _elementsFactory.visibleViews;

    
    readonly SimpleScrollElementFactory _elementsFactory;

    public ListCarouselPresenter(IScrollElementsDataSource elementsHolder, SimpleScrollElementFactory elementsFactory) : base(elementsHolder, elementsFactory)
    {
        _elementsFactory = elementsFactory;
    }
}