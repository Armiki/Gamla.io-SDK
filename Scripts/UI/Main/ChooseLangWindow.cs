using Gamla.Logic;
using UnityEngine;

namespace Gamla.UI
{
    public class ChooseLangWindow : GUIView
    {
        [SerializeField] private LangItem _langItmPref;
        [SerializeField] private RectTransform _content;

        public void Start()
        {
            foreach (var locale in LocalizationManager.Config.Locales)
            {
                var item = Instantiate(_langItmPref, _content).GetComponent<LangItem>();
                item.Init(locale.Locale);
            }
        }
    }
}
