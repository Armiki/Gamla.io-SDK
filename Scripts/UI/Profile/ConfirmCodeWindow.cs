using System;
using System.Collections;
using Gamla.Scripts.Common.UI;
using Gamla.Scripts.UI.Main;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.Scripts.UI.Profile
{
    public class ConfirmCodeWindow : GUIView
    {
        public event Action onCodeSendAgain;
        public event Action<string> onCodePressed;
        public event Action<string> onEmailCheck;
        public event Action<string> onPasswordCheck;
        
        [SerializeField] private GameObject _email;
        [SerializeField] private GameObject _password;
        [SerializeField] private GameObject _code;
        
        [SerializeField] private Text _emailTxt;
        [SerializeField] private InputField _codeInput;
        [SerializeField] private ValidateInputWidget _emailInput;
        [SerializeField] private ValidateInputWidget _passwordInput;
        
        [SerializeField] private Button _sendCodeAgain;
        [SerializeField] private Text _alreadySendCode;
        
        [SerializeField] private Button _continueBtn;
        [SerializeField] private Button _cancelBtn;
        [SerializeField] private Button _sendBtn;

        private bool _emailState = false;
        private float _time;
        private Coroutine _timerCoroutine;
        public void Start()
        {
            _cancelBtn.onClick.RemoveAllListeners();
            _cancelBtn.onClick.AddListener(Close);
            
            _continueBtn.onClick.RemoveAllListeners();
            _continueBtn.onClick.AddListener(() =>
            {
                if (_emailState)
                {
                    if (_emailInput.Validate())
                    {
                        Close();
                        onEmailCheck?.Invoke(_emailInput.text);
                    }
                }
                else
                {
                    if (_passwordInput.Validate())
                    {
                        onPasswordCheck?.Invoke(_passwordInput.text);
                    }
                }
              
            });
            
            _sendBtn.onClick.RemoveAllListeners();
            _sendBtn.onClick.AddListener(() =>
            {
                Close();
                onCodePressed?.Invoke(_codeInput.text);
            });
            
            _sendCodeAgain.onClick.RemoveAllListeners();
            _sendCodeAgain.onClick.AddListener(() =>
            {
                onCodeSendAgain?.Invoke();
            });
            _timerCoroutine = StartCoroutine(CodeSendTimer());
        }

        public override void OnDestroy()
        {
            StopCoroutine(_timerCoroutine);
            base.OnDestroy();
        }

        IEnumerator CodeSendTimer()
        {
            while (true)
            {
                _alreadySendCode.gameObject.SetActive(true);
                _sendCodeAgain.gameObject.SetActive(false);
                _alreadySendCode.text = "Send code again in 2:00"; //+ _time;
                yield return new WaitForSecondsRealtime(120);
                _sendCodeAgain.gameObject.SetActive(true);
                _alreadySendCode.gameObject.SetActive(true);
                _time = 0;
            }
        }

        public void InitPasswordState()
        {
            _code.SetActive(false);
            _email.SetActive(false);
            _password.SetActive(true);
            _continueBtn.gameObject.SetActive(true);
            _sendBtn.gameObject.SetActive(false);
        }

        public void InitEmailState()
        {
            _emailState = true;
            _code.SetActive(false);
            _email.SetActive(true);
            _password.SetActive(false);
            _continueBtn.gameObject.SetActive(true);
            _sendBtn.gameObject.SetActive(false);
        }
        
        public void InitCodeState(string email, string phone)
        {
            _emailTxt.text = email;
            _code.SetActive(true);
            _email.SetActive(false);
            _password.SetActive(false);
            _continueBtn.gameObject.SetActive(false);
            _sendBtn.gameObject.SetActive(true);
        }
    }
}