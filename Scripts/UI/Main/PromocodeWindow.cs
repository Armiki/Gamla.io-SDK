using System;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.UI
{
    public class PromocodeWindow : GUIView
    {
        public event Action<string> onPromoCodeClick;

        [SerializeField] private ValidateInputWidget _promoCodeInput;
        [SerializeField] private Button _enter;
        [SerializeField] private Button _paste;

        public void Start()
        {
            _enter.onClick.RemoveAllListeners();
            _enter.onClick.AddListener(() =>
            {
                if (_promoCodeInput.Validate())
                {
                    Close();
                    onPromoCodeClick?.Invoke(_promoCodeInput.text);
                }
            });
            
            _paste.onClick.RemoveAllListeners();
            _paste.onClick.AddListener(() =>
            {
                _promoCodeInput.SimpleSet(GUIUtility.systemCopyBuffer);
            });
            
            _promoCodeInput.keyboardChecker.onKeyboardChange.RemoveAllListeners();
            _promoCodeInput.keyboardChecker.onKeyboardChange.AddListener(RefreshKeyboardSafeZone);
        }

        void RefreshKeyboardSafeZone(int indentSize)
        {
            var root = GetComponent<RectTransform>();
            root.anchoredPosition = new Vector2(0, indentSize);
        }
    }
}