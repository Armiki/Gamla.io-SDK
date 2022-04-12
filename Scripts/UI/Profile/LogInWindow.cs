using System;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.UI
{
    public class LogInWindow : GUIView
    {
        public event Action<string, string> onLogInClick;
        public event Action onSignUpClick;
        public event Action onForgotPasswordClick;
        
        [SerializeField] private ValidateInputWidget _email;
        [SerializeField] private ValidateInputWidget _password;

        [SerializeField] private Button _forgotPassword;
        [SerializeField] private Button _login;
        [SerializeField] private Button _signUp;
        
        public void Start()
        {
            _login.onClick.RemoveAllListeners();
            _login.onClick.AddListener(() =>
            {
                if (_email.text.Length == 0 || _password.text.Length == 0)
                {
                    return;
                }
                Close();
                onLogInClick?.Invoke(_email.text, _password.text);
            });
            
            _signUp.onClick.RemoveAllListeners();
            _signUp.onClick.AddListener(() =>
            {
                Close();
                onSignUpClick?.Invoke();
            });
            
            _forgotPassword.onClick.RemoveAllListeners();
            _forgotPassword.onClick.AddListener(() =>
            {
                onForgotPasswordClick?.Invoke();
            });
            
            _email.keyboardChecker.onKeyboardChange.RemoveAllListeners();
            _email.keyboardChecker.onKeyboardChange.AddListener(RefreshSafeZone);
            
            _password.keyboardChecker.onKeyboardChange.RemoveAllListeners();
            _password.keyboardChecker.onKeyboardChange.AddListener(RefreshSafeZone);
        }
    }
}