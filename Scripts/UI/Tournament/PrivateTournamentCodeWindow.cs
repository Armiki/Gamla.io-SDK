using System;
using Gamla.Scripts.Common.UI;
using Gamla.Scripts.UI.Main;
using UnityEngine;
using UnityEngine.UI;

namespace GamlaSDK.Scripts.UI.Tournament
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