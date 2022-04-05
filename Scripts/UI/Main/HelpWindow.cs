using Gamla.Scripts.Common.UI;
using Gamla.Scripts.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.Scripts.UI.Main
{
    public class HelpWindow : GUIView
    {
        [SerializeField] private InputField _problem;
        [SerializeField] private InputField _email;
        [SerializeField] private InputField _matchId;
        [SerializeField] private Button _findMathcIdLink;
        
        [SerializeField] private Button _send;

        public void Start()
        {
           
        }
    }
}
