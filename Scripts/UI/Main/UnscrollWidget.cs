using UnityEngine;

namespace Gamla.Scripts.UI.Main
{
    public class UnscrollWidget : MonoBehaviour
    {
        [SerializeField] private RectTransform _scroll;
        [SerializeField] private UnscrollItem[] _unscrollItems;

        public void Update()
        {
            foreach (var item in _unscrollItems)
            {
                if (_scroll.localPosition.y >= item.scrollY)
                {
                    item.Active();
                }
                else
                {
                    item.Deactive();
                }
            }
        }
    }
}