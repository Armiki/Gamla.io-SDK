using System;
using Gamla.Scripts.Common.Carousel.ScrollElements;
using Gamla.Scripts.Data;
using Gamla.Scripts.Logic;
using Gamla.Scripts.UI.Main;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.Scripts.UI.Friends
{
    public class FriendWidget : MonoBehaviour
    {
        public static readonly string LoadPath = "Widgets/FriendWidget";
        public event Action<long> onSelectFriend;
        
        [SerializeField] private UserProfileWidget _user;
        [SerializeField] private Button _selectBtn;
        [SerializeField] private GameObject _selectGO;
        [SerializeField] private GameObject _newMsgFlag;
        
        public RectTransform rect;
        private ServerPublicUser _data;

        public long userId => _data.id;

        public bool isSelected
        {
            set
            {
                _selectGO.SetActive(value);
            }
        }

        public void Start()
        {
            _selectBtn.onClick.RemoveAllListeners();
            _selectBtn.onClick.AddListener(() => onSelectFriend?.Invoke(_data.id));
            _newMsgFlag.SetActive(false);
        }

        public void MatchRequest()
        {
            UIMapController.OpenSimpleWarningWindow(GUIWarningType.RematchRequest, () => {}, () =>
            {
                ServerCommand.CreateMatchRequest(2, "Z", _data.id);
            });
        }

        public void Init(ServerPublicUser user)
        {
            _data = user;
            _user.Init(user);
        }
        
    }
}