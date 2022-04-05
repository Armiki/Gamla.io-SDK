using System.Collections.Generic;
using Gamla.Scripts.Common.Carousel.DataSource;
using Gamla.Scripts.Common.Carousel.ScrollElementsFactories;
using Gamla.Scripts.Common;
using Gamla.Scripts.Common.UI;

namespace Gamla.Scripts.Test
{
    public class TestCarouselPresenter
    {
        #region - Carousels
        readonly GridDataSource<TestCarouselData> _testDataSource =
            new GridDataSource<TestCarouselData>(300, GridLineParams.DefaultMaxGridLineSize);

        CarouselPresenter _testCarouselPresenter;
        TestCarouselWindow _testCarouselWindow;
        #endregion

        public void Init(TestCarouselWindow window)
        {
            _testCarouselWindow = window;
            _testCarouselPresenter = Utils.CreateGridViewPresenter(
                _testCarouselWindow,
                _testDataSource,
                TestCarouselItem.LoadPath,
                GridLineParams.@default, 
                InitTestItemCell
            );
            window.Show();
            SetData();
        }

        void SetData()
        {
            //mock
            var listTest = new List<TestCarouselData>
            {
                new TestCarouselData {id = 1},
                new TestCarouselData {id = 2},
                new TestCarouselData {id = 3},
                new TestCarouselData {id = 4},
                new TestCarouselData {id = 5},
                new TestCarouselData {id = 6},
                new TestCarouselData {id = 7},
                new TestCarouselData {id = 8},
                new TestCarouselData {id = 9},
                new TestCarouselData {id = 10},
                new TestCarouselData {id = 11},
                new TestCarouselData {id = 12},
                new TestCarouselData {id = 13},
                new TestCarouselData {id = 14},
                new TestCarouselData {id = 15},
                new TestCarouselData {id = 16},
                new TestCarouselData {id = 17}
            };
            _testDataSource.UpdateData(listTest);
            
            //_testCarouselPresenter?.ReloadData();
            _testCarouselPresenter?.Reset();
        }
        
        void InitTestItemCell(GUIView view, int row, int col)
        {
            var shopItemData = _testDataSource.GetData(row, col);
            TestCarouselItem shopItem = view as TestCarouselItem;
            shopItem.Init(shopItemData);
            // init or action subscribe
        }
    }
}