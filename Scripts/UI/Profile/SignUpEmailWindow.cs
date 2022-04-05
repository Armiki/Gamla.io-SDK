using System;
using Gamla.Scripts.Common;
using Gamla.Scripts.Common.UI;
using Gamla.Scripts.UI.Main;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.Scripts.UI.Profile
{
    public class SignUpEmailWindow : GUIView
    {
        public event Action<string, string, string> onSignUpClick;
        public event Action onLogInClick;
        
        [SerializeField] private ValidateInputWidget _email;
        [SerializeField] private ValidateInputWidget _password;
        [SerializeField] private ValidateInputWidget _confirmPassword;

        [SerializeField] private Button _login;
        [SerializeField] private Button _signUp;

        public void Start()
        {
            _login.onClick.RemoveAllListeners();
            _login.onClick.AddListener(() =>
            {
                Close();
                onLogInClick?.Invoke();
            });
            
            _signUp.onClick.RemoveAllListeners();
            _signUp.onClick.AddListener(TrySignUp);
        }

        void TrySignUp()
        {
            var isValidEmail = _email.Validate();
            var _name = _email.text.Split('@')[0];
            var isValidPassword = _password.Validate(_confirmPassword.text);
            var isValidConfirmPassword = _confirmPassword.Validate(_password.text);
            if (isValidEmail && isValidPassword && isValidConfirmPassword)
            {
                Close();
                onSignUpClick?.Invoke(_name, _email.text, _password.text);
            }
        }
    }
}