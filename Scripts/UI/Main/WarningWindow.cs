using System;
using UnityEngine;
using UnityEngine.UI;
using Gamla.Data;
using Gamla.Logic;

namespace Gamla.UI
{
    public class WarningWindow : GUIView
    {
        public event Action onActionClick;

        [SerializeField] private Image _logo;
        [SerializeField] private Text _title;
        [SerializeField] private Text _description;
        [SerializeField] private Text _actionTitle;
        [SerializeField] private Button _actionBtn;
        [SerializeField] private Text _cancelTitle;
        [SerializeField] private InputField _uniqText;
        [SerializeField] private InputFieldKeyboardChecker _keyboardChecker;

        public void Start()
        {
            _actionBtn.onClick.RemoveAllListeners();
            _actionBtn.onClick.AddListener(() =>
            {
                onActionClick?.Invoke();
                Close();
            });

            if (_keyboardChecker != null) {
                _keyboardChecker.onKeyboardChange.RemoveAllListeners();
                _keyboardChecker.onKeyboardChange.AddListener(RefreshKeyboardSafeZone);
            }
        }

        public void Init(GUIWarningType type, bool withAction = false)
        {
            if (!DataConstants.warningDict.ContainsKey(type)) return;

            Init(DataConstants.warningDict[type]);
            _actionBtn.gameObject.SetActive(withAction);

            var descHeight = withAction ? 118 : 193;
            _description.rectTransform.sizeDelta = new Vector2(_description.rectTransform.sizeDelta.x, descHeight);
        }

        public void Init(GUIInfoWinData data)
        {
            _actionBtn.gameObject.SetActive(!string.IsNullOrEmpty(data.actionTitle));
            _title.text = data.title;
            _description.text = data.description;
            _actionTitle.text = data.actionTitle;
            _cancelTitle.text = data.closeTitle;
            
            if (!string.IsNullOrEmpty(data.logo))
            {
                _logo.sprite = GamlaResourceManager.GetSmiles(data.logo);
            }
        }
        public void Init(GUIInfoWinData data, string uniqText)
        {
            _title.text = data.title;
            _description.text = data.description;
            _actionTitle.text = data.actionTitle;
            _cancelTitle.text = data.closeTitle;
            
            if (!string.IsNullOrEmpty(data.logo))
            {
                _logo.sprite = GamlaResourceManager.GetSmiles(data.logo);
            }

            if (_uniqText != null)
            {
                _uniqText.text = uniqText;
            }
        }

        void RefreshKeyboardSafeZone(int indentSize)
        {
            var root = GetComponent<RectTransform>();
            root.anchoredPosition = new Vector2(0, indentSize);
        }
    }
}