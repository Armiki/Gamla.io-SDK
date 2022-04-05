using System;
using Gamla.Scripts.Common.UI;
using Gamla.Scripts.UI.Main;
using GamlaSDK.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.Scripts.UI.Profile
{
    public class ChangeProfileItemWindow : GUIView
    {
        public event Action<string, string> onChangeProfileItem;
        
        [SerializeField] private Text _winTitle;

        [SerializeField] private ValidateInputWidget _item0;
        [SerializeField] private ValidateInputWidget _item1;
        
        [SerializeField] private Button _cancelBtn;
        [SerializeField] private Button _sendBtn;

        public void Start()
        {
            _cancelBtn.onClick.RemoveAllListeners();
            _cancelBtn.onClick.AddListener(Close);
            
            _sendBtn.onClick.RemoveAllListeners();
            _sendBtn.onClick.AddListener(() =>
            {
                var isValid0 = _item0.Validate(_item1.text);
                var isValid1 = _item1.Validate(_item0.text);
                if (isValid0 && isValid1)
                {
                    onChangeProfileItem?.Invoke(_item0.text, _item1.text);
                    Close();
                }
            });
        }

        public void ChangePassword()
        {
            _winTitle.text = LocalizationManager.Text("gamla.window.changeprofile.changepassword");
            _item0.gameObject.SetActive(true);
            _item1.gameObject.SetActive(true);
            _item0.Set(LocalizationManager.Text("gamla.window.changeprofile.password"), ValidateInputType.Password);
            _item1.Set(LocalizationManager.Text("gamla.window.changeprofile.confirmpassword"), ValidateInputType.Password);
        }

        public void ChangePhone(string currentItem)
        {
            _winTitle.text = LocalizationManager.Text("gamla.window.changeprofile.changephone");
            _item0.gameObject.SetActive(true);
            _item1.gameObject.SetActive(false);
            _item0.Set(LocalizationManager.Text("gamla.window.changeprofile.phone"), ValidateInputType.Phone, currentItem);
            _item1.Set("", ValidateInputType.None);
        }
        
        public void ChangePublishName(string currentItem)
        {
            _winTitle.text = LocalizationManager.Text("gamla.window.changeprofile.changepublishname");
            _item0.gameObject.SetActive(true);
            _item1.gameObject.SetActive(false);
            _item0.Set(LocalizationManager.Text("gamla.window.changeprofile.name"), ValidateInputType.Simple, currentItem);
        }
        
        public void ChangeName(string currentItem1, string currentItem2)
        {
            _winTitle.text = LocalizationManager.Text("gamla.window.changeprofile.changename");
            _item0.gameObject.SetActive(true);
            _item1.gameObject.SetActive(true);
            _item0.Set(LocalizationManager.Text("gamla.window.changeprofile.firtsname"), ValidateInputType.Simple, currentItem1);
            _item1.Set(LocalizationManager.Text("gamla.window.changeprofile.lastname"), ValidateInputType.Simple, currentItem2);
        }
        
        public void ChangeAddress(string currentItem)
        {
            _winTitle.text = LocalizationManager.Text("gamla.window.changeprofile.changeaddress");
            _item0.gameObject.SetActive(true);
            _item1.gameObject.SetActive(false);
            _item0.Set(LocalizationManager.Text("gamla.window.changeprofile.address"), ValidateInputType.Simple, currentItem);
            _item1.Set("", ValidateInputType.None);
        }
    }
}