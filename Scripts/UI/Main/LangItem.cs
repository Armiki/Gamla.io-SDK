using Gamla.Scripts.Logic;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.Scripts.UI.Main
{
    public class LangItem : MonoBehaviour
    {
        [SerializeField] private Text _title;
        [SerializeField] private Button _btn;
        [SerializeField] private GameObject _selected;

        private string _language;

        public void Init(string lang)
        {
            _language = lang;
            _title.text = lang;
            _selected.SetActive(GamlaSDK.Scripts.LocalizationManager.CurrentLanguage == _language);
            
            EventManager.OnLanguageChange.Subscribe(UpdateViewState);
            
            _btn.onClick.RemoveAllListeners();
            _btn.onClick.AddListener(() =>
            {
                if(GamlaSDK.Scripts.LocalizationManager.CurrentLanguage != _language)
                    GamlaSDK.Scripts.LocalizationManager.ChangeLanguage(_language);
            });
        }

        private void UpdateViewState()
        {
            _selected.SetActive(GamlaSDK.Scripts.LocalizationManager.CurrentLanguage == _language);
        }

        private void OnDestroy()
        {
            EventManager.OnLanguageChange.Del(UpdateViewState);
        }
    }
}
