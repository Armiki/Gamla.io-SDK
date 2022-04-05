using UnityEngine;

namespace Gamla.Scripts.UI.Main
{
    public class UnscrollItem : MonoBehaviour
    {
        [SerializeField] private float _scrollY;
        [SerializeField] private GameObject _itemInScroll;
        [SerializeField] private GameObject _itemInScrollEmpty;
        [SerializeField] private GameObject _itemOnHead;
        
        public float scrollY => _scrollY;

        public void Active()
        {
            _itemInScroll.SetActive(false);
            if(_itemInScrollEmpty != null) _itemInScrollEmpty.SetActive(true);
            _itemOnHead.SetActive(true);
        }
        
        public void Deactive()
        {
            _itemInScroll.SetActive(true);
            if(_itemInScrollEmpty != null) _itemInScrollEmpty.SetActive(false);
            _itemOnHead.SetActive(false);
        }
    }
}