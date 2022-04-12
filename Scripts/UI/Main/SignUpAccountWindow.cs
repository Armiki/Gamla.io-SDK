using System;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.UI
{
    public class SignUpAccountWindow : GUIView
    {

        public event Action onSignUpAppleClick;
        public event Action onSignUpGoogleClick;
        public event Action onSignUpEmailClick;

        [SerializeField] private Button _signUpApple;
        [SerializeField] private Button _signUpGoogle;
        [SerializeField] private Button _signUpEmail;


        public void Start()
        {
            _signUpApple.onClick.RemoveAllListeners();
            _signUpApple.onClick.AddListener(() => onSignUpAppleClick?.Invoke());

            _signUpGoogle.onClick.RemoveAllListeners();
            _signUpGoogle.onClick.AddListener(() => onSignUpGoogleClick?.Invoke());

            _signUpEmail.onClick.RemoveAllListeners();
            _signUpEmail.onClick.AddListener(() => onSignUpEmailClick?.Invoke());
        }
    }
}
