using System;
using UnityEngine;
using UnityEngine.UI;
using Gamla.Data;


namespace Gamla.UI
{
    public class UserInfoWindow : GUIView
    {
        public event Action<long> toBattle;
        public event Action<long> sendMessage;
        public event Action<long> invite;
        public event Action<long> profile;
        
        [SerializeField] private UserProfileWidget _userProfileWidget;
        [SerializeField] private Button _toBattleBtn;
        [SerializeField] private Button _sendMessageBtn;
        [SerializeField] private Button _inviteBtn;
        [SerializeField] private Button _profileBtn;

        private long _id;
        public void Start()
        {
            _toBattleBtn.onClick.RemoveAllListeners();
            _toBattleBtn.onClick.AddListener(() =>
            {
                toBattle?.Invoke(_id);
                Close();
            });
            
            _sendMessageBtn.onClick.RemoveAllListeners();
            _sendMessageBtn.onClick.AddListener(() =>
            {
                sendMessage?.Invoke(_id);
                Close();
            });
            
            _inviteBtn.onClick.RemoveAllListeners();
            _inviteBtn.onClick.AddListener(() =>
            {
                invite?.Invoke(_id);
                Close();
            });
            
            _profileBtn.onClick.RemoveAllListeners();
            _profileBtn.onClick.AddListener(() =>
            {
                profile?.Invoke(_id);
                Close();
            });
        }

        public void Init(ServerPublicUser user, bool isFriend)
        {
            _id = user.id;
            _userProfileWidget.Init(user);
            _toBattleBtn.gameObject.SetActive(isFriend);
            _sendMessageBtn.gameObject.SetActive(isFriend);
            _inviteBtn.gameObject.SetActive(!isFriend);
        }
    }
}