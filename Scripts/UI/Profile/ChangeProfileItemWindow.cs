using System;
using Gamla.Data;
using Gamla.Logic;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.UI
{
    public class ChangeProfileItemWindow : GUIView
    {
        public event Action<string, string> onChangeProfileItem;
        
        [SerializeField] private Text _winTitle;

        [SerializeField] private ValidateInputWidget _item0;
        [SerializeField] private ValidateInputWidget _item1;
        [SerializeField] private ValidateInputWidget _item2;
        
        [SerializeField] private Button _cancelBtn;
        [SerializeField] private Button _sendBtn;

        private Func<bool> _additionValidation = null;

        public void Start()
        {
            _cancelBtn.onClick.RemoveAllListeners();
            _cancelBtn.onClick.AddListener(Close);
            
            _sendBtn.onClick.RemoveAllListeners();
            _sendBtn.onClick.AddListener(() =>
            {
                var isValid0 = _item0.Validate(_item0.text);
                var isValid1 = _item1.Validate(_item1.text);
                var isValid2 = LocalState.currentUser.guest || _item2.Validate(_item2.text);
                var isValidAddition = _additionValidation == null || _additionValidation();
                if (isValid0 && isValid1 && isValid2 && isValidAddition)
                {
                    onChangeProfileItem?.Invoke(_item0.text, _item1.text);
                    Close();
                }
            });
            
            _item2.gameObject.SetActive(!LocalState.currentUser.guest);
        }

        public void ChangePassword(string oldPassword)
        {
            _winTitle.text = LocalizationManager.Text("gamla.window.changeprofile.changepassword");
            _item0.gameObject.SetActive(true);
            _item1.gameObject.SetActive(true);
            _item2.gameObject.SetActive(true);
            _item0.Set(LocalizationManager.Text("gamla.window.changeprofile.password"), ValidateInputType.Password);
            _item1.Set(LocalizationManager.Text("gamla.window.changeprofile.confirmpassword"), ValidateInputType.Password);
            _item2.Set(LocalizationManager.Text("gamla.window.changeprofile.oldpassword"), ValidateInputType.Password);
            _additionValidation = () =>
            {
                bool valid = _item0.Validate(_item1.text) & _item1.Validate(_item0.text);
                if (!valid) {
                    return false;
                }

                if (LocalState.currentUser.guest)
                    return true;

                valid = _item2.text == oldPassword;
                if (!valid) {
                    UIMapController.OpenSimpleErrorWindow("gamla.window.changeprofile.wrongpassword.error");
                }
                return valid;
            };
        }

        public void ChangePhone(string currentItem)
        {
            _winTitle.text = LocalizationManager.Text("gamla.window.changeprofile.changephone");
            _item0.gameObject.SetActive(true);
            _item1.gameObject.SetActive(false);
            _item2.gameObject.SetActive(false);
            _item0.Set(LocalizationManager.Text("gamla.window.changeprofile.phone"), ValidateInputType.Phone, currentItem);
            _item1.Set("", ValidateInputType.None);
            _additionValidation = null;
        }
        
        public void ChangePublishName(string currentItem)
        {
            _winTitle.text = LocalizationManager.Text("gamla.window.changeprofile.changepublishname");
            _item0.gameObject.SetActive(true);
            _item1.gameObject.SetActive(false);
            _item2.gameObject.SetActive(false);
            _item0.Set(LocalizationManager.Text("gamla.window.changeprofile.name"), ValidateInputType.Simple, currentItem);
            _additionValidation = null;
        }
        
        public void ChangeName(string currentItem1, string currentItem2)
        {
            _winTitle.text = LocalizationManager.Text("gamla.window.changeprofile.changename");
            _item0.gameObject.SetActive(true);
            _item1.gameObject.SetActive(true);
            _item2.gameObject.SetActive(false);
            _item0.Set(LocalizationManager.Text("gamla.window.changeprofile.firtsname"), ValidateInputType.Simple, currentItem1);
            _item1.Set(LocalizationManager.Text("gamla.window.changeprofile.lastname"), ValidateInputType.Simple, currentItem2);
            _additionValidation = null;
        }
        
        public void ChangeAddress(string currentItem)
        {
            _winTitle.text = LocalizationManager.Text("gamla.window.changeprofile.changeaddress");
            _item0.gameObject.SetActive(true);
            _item1.gameObject.SetActive(false);
            _item2.gameObject.SetActive(false);
            _item0.Set(LocalizationManager.Text("gamla.window.changeprofile.address"), ValidateInputType.Simple, currentItem);
            _item1.Set("", ValidateInputType.None);
            _additionValidation = null;
        }
    }
}