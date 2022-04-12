using System.Collections.Generic;
using UnityEngine;

namespace Gamla.UI
{
    public class TutorialBlocker : MonoBehaviour
    {
        [SerializeField] private List<TutorialRefPoint> _tutorialRefPoints;
        
        public List<TutorialRefPoint> TutorialRefPoints => _tutorialRefPoints;
    }
}
