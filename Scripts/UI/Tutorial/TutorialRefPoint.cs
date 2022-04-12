using UnityEngine;

namespace Gamla.UI
{
    public class TutorialRefPoint : MonoBehaviour
    {
        [SerializeField] private string _tutorialStepName;
        [SerializeField] private RectTransform _refPoint;

        public string TutorialStepName => _tutorialStepName;
        public RectTransform RefPoint => _refPoint;
    }
}