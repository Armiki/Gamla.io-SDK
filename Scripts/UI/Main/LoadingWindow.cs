using Gamla.Scripts.Common.UI;
using UnityEngine;
using UnityEngine.UI;


namespace Gamla.Scripts.UI.Main
{
    public class LoadingWindow : GUIView
    {
       [SerializeField] private Image _progress;

        public void Start()
        {
            _progress.fillAmount = 0;
        }

        public void ChangeProgress(float progress)
        {
            _progress.fillAmount = progress;
        }
    }
}
