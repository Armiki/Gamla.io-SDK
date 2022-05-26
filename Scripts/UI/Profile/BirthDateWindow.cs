using System;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.UI
{
    public class BirthDateWindow : GUIView
    {
        public event Action<int> OnAgeAccept;
        
        [SerializeField] private Button _accept;
        [SerializeField] private ValidateInputWidget _ageInput;

        void Start()
        {
            _accept.onClick.AddListener(() =>
            {
                if (_ageInput.Validate())
                {
                    if (int.TryParse(_ageInput.text, out int age))
                    {
                        OnAgeAccept?.Invoke(age);
                        Close();
                    }
                }
            });
        }
    }
}
