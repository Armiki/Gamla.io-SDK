using Gamla.Scripts.Common.Carousel.ScrollElements;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.Scripts.UI.Friends
{
    public class MessageWidget : MonoBehaviour
    {
        public static readonly string LoadPath = "Widgets/MessageWidget";
        
        [SerializeField] private Text _message;
        [SerializeField] private Text _date;

        public RectTransform rect;

        public void Init(string text, string date)
        {
            _message.text = text;
            //_date.text = date;
        }
    }
}