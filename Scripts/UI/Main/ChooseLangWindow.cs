using Gamla.Scripts.Common.UI;
using UnityEngine;


namespace Gamla.Scripts.UI.Main
{
    public class ChooseLangWindow : GUIView
    {
        [SerializeField] private LangItem _langItmPref;
        [SerializeField] private RectTransform _content;

        public void Start()
        {
            foreach (var locale in GamlaSDK.Scripts.LocalizationManager.Config.Locales)
            {
                var item = Instantiate(_langItmPref, _content).GetComponent<LangItem>();
                item.Init(locale.Locale);
            }
        }
        
    }
}
