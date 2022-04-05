using Gamla.Scripts.Common.Carousel.ScrollElements;
using TMPro;
using UnityEngine;

namespace Gamla.Scripts.Test
{
    public class TestCarouselItem : BaseScrollElement
    {
        public static readonly string LoadPath = "Test/TestCarouselItem";

        [SerializeField] private TextMeshProUGUI _id;
        
        public void Init(TestCarouselData data)
        {
            _id.text = data.id.ToString();
        }
    }
}