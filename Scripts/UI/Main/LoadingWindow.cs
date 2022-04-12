using UnityEngine;
using UnityEngine.UI;

namespace Gamla.UI
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
