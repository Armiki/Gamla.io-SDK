using System;
using UnityEngine;
using UnityEngine.UI;


namespace Gamla.UI
{
    public class TabBarItem : MonoBehaviour
    {
        public event Action onSelect;
        
        [SerializeField] private Button _select;
        [SerializeField] private GameObject _lineSelected;
        
        [SerializeField] private RecolorItem _titleRecolor;
        [SerializeField] private RecolorItem _iconRecolor;
        private bool _pressed;

        public bool pressed
        {
            set
            {
                _titleRecolor.recolorFilter = value ? "textColorLink" : "buttonSecondaryColor";
                _iconRecolor.recolorFilter = value ? "textColorLink" : "buttonSecondaryColor";
                _titleRecolor.Recolor();
                _iconRecolor.Recolor();
                _lineSelected.SetActive(value);

                _pressed = value;
            }
        }
        public void Start()
        {
            _select.onClick.RemoveAllListeners();
            _select.onClick.AddListener(() =>
            {
                if (!_pressed)
                    pressed = true;
                    onSelect?.Invoke();
            });
        }
    }
}
