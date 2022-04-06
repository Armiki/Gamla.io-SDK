using System;
using Gamla.Scripts.Common.UI;
using Gamla.Scripts.UI.Main;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.Scripts.UI.Profile
{
    public class ChangeAddressWindow : GUIView
    {
        public event Action<string> onChangeAddress;
        
        [SerializeField] private ValidateInputWidget _street1;
        [SerializeField] private ValidateInputWidget _street2;
        [SerializeField] private ValidateInputWidget _country;
        [SerializeField] private ValidateInputWidget _city;
        [SerializeField] private ValidateInputWidget _state1;
        [SerializeField] private ValidateInputWidget _state2;
        
        [SerializeField] private Button _cancelBtn;
        [SerializeField] private Button _sendBtn;

        [SerializeField] private RectTransform _content;

        private float _defaultContentHeight;
        
        public void Start()
        {
            _defaultContentHeight = _content.sizeDelta.y;
            
            _street1.keyboardChecker.onKeyboardChange.RemoveAllListeners();
            _street1.keyboardChecker.onKeyboardChange.AddListener(RefreshKeyboardSafeZone);

            _street2.keyboardChecker.onKeyboardChange.RemoveAllListeners();
            _street2.keyboardChecker.onKeyboardChange.AddListener(RefreshKeyboardSafeZone);

            _country.keyboardChecker.onKeyboardChange.RemoveAllListeners();
            _country.keyboardChecker.onKeyboardChange.AddListener(RefreshKeyboardSafeZone);

            _city.keyboardChecker.onKeyboardChange.RemoveAllListeners();
            _city.keyboardChecker.onKeyboardChange.AddListener(RefreshKeyboardSafeZone);

            _state1.keyboardChecker.onKeyboardChange.RemoveAllListeners();
            _state1.keyboardChecker.onKeyboardChange.AddListener(RefreshKeyboardSafeZone);

            _state2.keyboardChecker.onKeyboardChange.RemoveAllListeners();
            _state2.keyboardChecker.onKeyboardChange.AddListener(RefreshKeyboardSafeZone);

            _cancelBtn.onClick.RemoveAllListeners();
            _cancelBtn.onClick.AddListener(Close);
            
            _sendBtn.onClick.RemoveAllListeners();
            _sendBtn.onClick.AddListener(() =>
            {
                var formatAddress = string.Format("{0}|{1}|{2}|{3}|{4}|{5}", _street1.text, _street2.text,
                    _country.text, _city.text, _state1.text, _state2.text);
                if (_street1.Validate("please fill your street info") && _country.Validate("please fill your country")
                                                                      && _city.Validate("please fill your city")) ;
                Close();
                onChangeAddress?.Invoke(formatAddress);
            });
        }

        public void ChangeAddress(string data)
        {
            var splitData = data.Split('|');
            if (splitData.Length < 6)
            {
                Debug.LogWarning("problem address format");
                return;
            }
            _street1.SimpleSet(splitData[0]);
            _street2.SimpleSet(splitData[1]);
            _country.SimpleSet(splitData[2]);
            _city.SimpleSet(splitData[3]);
            _state1.SimpleSet(splitData[4]);
            _state2.SimpleSet(splitData[5]);
        }

        void RefreshKeyboardSafeZone(int indentSize)
        {
            _content.sizeDelta = new Vector2(_content.sizeDelta.x, _defaultContentHeight + indentSize);
            base.RefreshSafeZone(indentSize);
        }
    }
}