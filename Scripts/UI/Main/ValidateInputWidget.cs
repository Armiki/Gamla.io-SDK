using System;
using Gamla.Scripts.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.Scripts.UI.Main
{
    public enum ValidateInputType
    {
        None,
        Email,
        Password,
        Simple,
        Phone
    }
    
    public class ValidateInputWidget : MonoBehaviour
    {
        public event Action<string> onInputChange;
        public InputFieldKeyboardChecker keyboardChecker;
        
        [SerializeField] private Text _title;
        [SerializeField] private ValidateInputType _type;
        [SerializeField] private InputField _input;
        [SerializeField] private RecolorItem _inputRecolor;
        [SerializeField] private Text _unCorrectGO;

        public string text => _input.text;

        public void Start()
        {
            _input.onValueChanged.AddListener((_) =>
            {
                ChangeCorrectStatus(true, "");
                onInputChange?.Invoke(_input.text);
            });
        }

        public void Set(string title, ValidateInputType type, string currentValue = "")
        {
            _title.text = title;
            _type = type;
            _input.text = currentValue;
        }
        
        public void SimpleSet(string currentValue = "")
        {
            _input.text = currentValue;
        }

        public bool Validate(string otherText = "")
        {
            var isValid = false;
            switch (_type)
            {
                case ValidateInputType.None:
                    isValid = true;
                    break;
                case ValidateInputType.Simple:
                    isValid = SimpleValidate();
                    break;
                case ValidateInputType.Email:
                    isValid = EmailValidate();
                    break;
                case ValidateInputType.Password:
                    isValid = PasswordValidate(otherText);
                    break;
                case ValidateInputType.Phone:
                    isValid = PhoneValidate();
                    break;
            }
            return isValid;
        }

        bool SimpleValidate()
        {
            if (!string.IsNullOrEmpty(_input.text))
            {
                return true;
            }
            ChangeCorrectStatus(false, "please fill in the input field");
            return false;
        }

        bool EmailValidate()
        {
            if (Utils.IsValidEmail(_input.text))
            {
                return true;
            }
            ChangeCorrectStatus(false, "incorrect email");
            return false;
        }
        
        bool PhoneValidate()
        {
            if (Utils.IsValidPhone(_input.text))
            {
                return true;
            }
            ChangeCorrectStatus(false, "incorrect phone");
            return false;
        }

        bool PasswordValidate(string otherText)
        {
            //if (Utils.IsValidPassword(_input.text))
            //{
            if(otherText.Length < 8)
            {
                ChangeCorrectStatus(false, "Password must be at least 8 characters");
            }
            else if (String.Compare(_input.text, otherText, StringComparison.Ordinal) == 0)
            {
                return true;
            }
            else
            {
                ChangeCorrectStatus(false, "password mismatch");
            }
            /*}
            else
            {
                ChangeCorrectStatus(false,
                    "Password must be at least 4 characters, no more than 8 characters, and must include at least one upper case letter, one lower case letter, and one numeric digit.");
            }*/
            return false;
        }

        void ChangeCorrectStatus(bool isCorrect, string msg)
        {
            if (_unCorrectGO != null) 
            {
                _unCorrectGO.gameObject.SetActive(!isCorrect);
                _unCorrectGO.text = msg;
            }
            _inputRecolor.recolorFilter = isCorrect ? "textColorTertiary" : "textColorError";
            _inputRecolor.Recolor();
        }
    }
}