using System.Collections.Generic;
using Gamla.Scripts.Common.Carousel.DataSource;
using Gamla.Scripts.Common.Carousel.ScrollElements;
using Gamla.Scripts.Common.Carousel.ScrollElementsFactories;

public sealed class GridCarouselPresenter : CarouselPresenter
{
    /// <summary>
    /// row x col
    /// </summary>
    public IReadOnlyDictionary<(int,int), BaseScrollElement> visibleViews => _elementsFactory.visibleViews;

    
    readonly SimpleGridElementFactory _elementsFactory;

    public GridCarouselPresenter(IScrollElementsDataSource elementsHolder, SimpleGridElementFactory elementsFactory) : base(elementsHolder, elementsFactory)
    {
        _elementsFactory = elementsFactory;
    }
}