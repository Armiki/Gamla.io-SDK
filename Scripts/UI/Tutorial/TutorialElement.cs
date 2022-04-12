using UnityEngine;
using UnityEngine.UI;

namespace Gamla.UI
{
    public class TutorialElement : MonoBehaviour
    {
        [SerializeField] private string _tutorialStepName;
        [SerializeField] private Button _tutorialButton;

        public string TutorialStepName
        {
            get => _tutorialStepName;
            set => _tutorialStepName = value;
        }
        
        public Button TutorialButton => _tutorialButton;
    }
}