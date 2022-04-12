using System.Collections.Generic;
using Gamla.Logic;
using UnityEngine;

namespace Gamla.UI
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