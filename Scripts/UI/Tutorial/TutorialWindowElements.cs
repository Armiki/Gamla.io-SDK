using System.Collections.Generic;
using UnityEngine;

namespace Gamla.Scripts.UI.Tutorial
{
    public class TutorialWindowElements : MonoBehaviour
    {
        [SerializeField] private List<TutorialElement> _elements;
        
        private void Start()
        {
            if (_elements != null && !TutorialManager.Instance.IsCompleted)
            {
                TutorialManager.Instance.TryActivateStepWithElements(_elements);
            }
        }
    }
}