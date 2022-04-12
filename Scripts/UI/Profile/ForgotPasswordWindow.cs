using System;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.UI
{
    public class ForgotPasswordWindow : GUIView
    {
        public event Action<string> onResetEmailClick;
        public event Action<string> onResetPhoneClick;

        [SerializeField] private GameObject _email;
        [SerializeField] private ValidateInputWidget _emailInput;
        [SerializeField] private Button _usePhoneBtn;
        [SerializeField] private GameObject _phone;
        [SerializeField] private ValidateInputWidget _phoneInput;
        [SerializeField] private Button _useEmailBtn;

        [SerializeField] private Button _sendLinkBtn;

        private bool _isEmailState;
        
        public void Start()
        {
            _sendLinkBtn.onClick.RemoveAllListeners();
            _sendLinkBtn.onClick.AddListener(() =>
            {
                if (_isEmailState)
                {
                    if (_emailInput.Validate())
                    {
                        Close();
                        onResetEmailClick?.Invoke(_emailInput.text);
                    }
                }
                else
                {
                    if (_phoneInput.Validate())
                    {
                        Close();
                        onResetPhoneClick?.Invoke(_phoneInput.text);
                    }
                }
                
            });
            
            _useEmailBtn.onClick.RemoveAllListeners();
            _useEmailBtn.onClick.AddListener(EmailState);
            _usePhoneBtn.onClick.RemoveAllListeners();
            _usePhoneBtn.onClick.AddListener(PhoneState);
        }

        public void Init()
        {
            EmailState();
        }

        void EmailState()
        {
            _isEmailState = true;
            _email.SetActive(true);
            _phone.SetActive(false);
            _useEmailBtn.gameObject.SetActive(false);
            _usePhoneBtn.gameObject.SetActive(true);
        }
        
        void PhoneState()
        {
            _isEmailState = false;
            _email.SetActive(false);
            _phone.SetActive(true);
            _useEmailBtn.gameObject.SetActive(true);
            _usePhoneBtn.gameObject.SetActive(false);
        }
        
        
    }
}