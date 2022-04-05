using Gamla.Scripts.Logic;
using UnityEngine;
using UnityEngine.UI;

namespace GamlaSDK.Scripts.UI
{
    public class LocaleTextWidget : MonoBehaviour
    {
        [SerializeField] private Text _textField;
        [SerializeField] private string _localeKey;

        private void Awake()
        {
            if (_textField == null)
                _textField = GetComponent<Text>();
        }

        void Start()
        {
            if (_textField != null)
            {
                EventManager.OnLanguageChange.Subscribe(UpdateLocaleKey);
                UpdateLocaleKey();
            }
        }

        void UpdateLocaleKey()
        {
            if (string.IsNullOrEmpty(_localeKey))
            {
                return;
            }
            _textField.text = LocalizationManager.Text(_localeKey);
        }

        private void OnDestroy()
        {
            if (_textField != null)
            {
                EventManager.OnLanguageChange.Del(UpdateLocaleKey);
            }
        }
    }
}