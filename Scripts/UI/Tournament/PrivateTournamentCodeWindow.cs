using System;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.UI
{
    public class PrivateTournamentCodeWindow : GUIView
    {
        public event Action<string> onPromoCodeClick;

        [SerializeField] private ValidateInputWidget _promoCodeInput;
        [SerializeField] private Button _enter;

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