using System;
using System.Collections.Generic;
using Gamla.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.UI
{
    public class FriendsWindow : GUIView
    {
        public event Action<long> onSelectFriend;
        
        [SerializeField] private RectTransform _friendContent;
        [SerializeField] private FriendWidget _friendWidget;
        
        [SerializeField] private RectTransform _requestContent;
        [SerializeField] private RequestWidget _requestWidget;
        
        [SerializeField] private CurrencyFilter _currencyFilter;

        [SerializeField] private GameObject _friendFilterGO;
        [SerializeField] private GameObject _requestFilterGO;
        
        [SerializeField] private Button _addFriend;
        [SerializeField] private GameObject _contextGO;
        [SerializeField] private Button _chatBtn;
        [SerializeField] private Button _challengeBtn;
        [SerializeField] private Button _deleteBtn;

        private List<FriendWidget> _friendsWidgets = new List<FriendWidget>();

        public void Start()
        {
            _friendFilterGO.SetActive(true);
            _requestFilterGO.SetActive(false);
            _contextGO.SetActive(false);

            _currencyFilter.onCurrencyClick += (type) =>
            {
                if (type == CurrencyType.USD)
                {
                    _friendFilterGO.SetActive(true);
                    _requestFilterGO.SetActive(false);
                }
                
                if (type == CurrencyType.Z)
                {
                    _friendFilterGO.SetActive(false);
                    _requestFilterGO.SetActive(true);
                }
            };
        }

        public void InitFriends(ServerFriends friendData)
        {
            _friendsWidgets.Clear();
            SetData(_friendContent, friendData.friends, false);
            SetData(_requestContent, friendData.friends_out, true);
            SetData(_requestContent, friendData.friends_in, true, false);
            
            _friendFilterGO.SetActive(true);
            _requestFilterGO.SetActive(false);
        }

        private void SetData(RectTransform content, List<ServerPublicUser> friends, bool isRequestWidget, bool isNeedClearContent = true)
        {
            if(isNeedClearContent)content.ClearChilds();
            if (friends != null)
            {
                if (isRequestWidget)
                {
                    content.sizeDelta = new Vector2(content.sizeDelta.x,
                        (friends.Count * _requestWidget.rect.sizeDelta.y + friends.Count * 30));
                    foreach (var data in friends)
                    {
                        var item = Instantiate(_requestWidget, content);
                        item.Init(data, isNeedClearContent);
//                        item.onSelectFriend += SelectFriend;
                    }
                }
                else
                {
                    content.sizeDelta = new Vector2(content.sizeDelta.x,
                        (friends.Count * _friendWidget.rect.sizeDelta.y + friends.Count * 30));
                    foreach (var data in friends)
                    {
                        var item = Instantiate(_friendWidget, content);
                        item.Init(data);
                        item.onSelectFriend += SelectFriend;
                        _friendsWidgets.Add(item);
                    }
                }
            }
        }

        void SelectFriend(long id)
        {
            foreach (var friendsWidget in _friendsWidgets)
            {
                friendsWidget.isSelected = friendsWidget.userId == id;
            }
            onSelectFriend?.Invoke(id);
        }

    }
}