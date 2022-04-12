using Gamla.Logic;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.UI
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
            _selected.SetActive(LocalizationManager.CurrentLanguage == _language);
            
            EventManager.OnLanguageChange.Subscribe(UpdateViewState);
            
            _btn.onClick.RemoveAllListeners();
            _btn.onClick.AddListener(() =>
            {
                if(LocalizationManager.CurrentLanguage != _language)
                    LocalizationManager.ChangeLanguage(_language);
            });
        }

        private void UpdateViewState()
        {
            _selected.SetActive(LocalizationManager.CurrentLanguage == _language);
        }

        private void OnDestroy()
        {
            EventManager.OnLanguageChange.Del(UpdateViewState);
        }
    }
}
