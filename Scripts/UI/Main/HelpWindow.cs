using UnityEngine;
using UnityEngine.UI;

namespace Gamla.UI
{
    public class HelpWindow : GUIView
    {
        [SerializeField] private InputField _problem;
        [SerializeField] private InputField _email;
        [SerializeField] private InputField _matchId;
        [SerializeField] private Button _findMathcIdLink;
        
        [SerializeField] private Button _send;
    }
}
