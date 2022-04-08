using Gamla.Scripts.Common.Carousel.ScrollElements;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.Scripts.UI.Friends
{
    public class MessageWidget : MonoBehaviour
    {
        public static readonly string LoadPath = "Widgets/MessageWidget";

        public long Id { get; private set; }

        [SerializeField] private Text _message;
        [SerializeField] private Text _date;

        public RectTransform rect;

        public void Init(long id, string text, string date)
        {
            Id = id;
            _message.text = text;
            //_date.text = date;
        }
    }
}