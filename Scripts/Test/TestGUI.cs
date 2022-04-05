using Gamla.Scripts.Logic;
using UnityEngine;

namespace Gamla.Scripts.Test
{
    public class TestGUI : MonoBehaviour
    {
        public void Start()
        {
            //var item = GameResourceManager.gameResourceManager.GetResource("test");
            //Instantiate(item, GameResourceManager.windowsContainer);
            //TestCarousel();
        }

        public void TestCarousel()
        {
            TestCarouselPresenter carouselPresenter = new TestCarouselPresenter();
            
            var pref = GamlaResourceManager.GamlaResources.GetResource("Test/TestCarouselWindow").GetComponent<TestCarouselWindow>();
            var window = Instantiate(pref, GamlaResourceManager.windowsContainer);
            carouselPresenter.Init(window);
        }
    }
}