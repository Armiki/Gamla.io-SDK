using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Gamla.Scripts.Common;
using Gamla.Scripts.Common.UI;
using Gamla.Scripts.Data;
using Gamla.Scripts.Logic;
using Gamla.Scripts.UI.Main;
using GamlaSDK;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.Scripts.UI.Friends
{
    public class ChatWindow : GUIView
    {
        [SerializeField] private InputField _inputField;
        [SerializeField] private InputFieldKeyboardChecker _keyboardChecker;
        
        [SerializeField] private RectTransform _messageContent;
        [SerializeField] private MessageWidget _messageWidgetMy;
        [SerializeField] private MessageWidget _messageWidgetUser;
        
        [SerializeField] private Button _sendMessage;
        
        private List<MessageWidget> _messages = new List<MessageWidget>();
        private float _lastUpdateTime = 0;
        private long _chatId;

        public void Start()
        {
            _sendMessage.onClick.RemoveAllListeners();
            _sendMessage.onClick.AddListener(AddNewMessage);
            
            _keyboardChecker.onKeyboardChange.RemoveAllListeners();
            _keyboardChecker.onKeyboardChange.AddListener(RefreshKeyboardSafeZone);
        }
        
        public void InitMessages(long chatId, List<ServerChatMessage> messages)
        {
            _chatId = chatId;
            _messageContent.ClearChilds();
            _messages.Clear();

            _messageContent.sizeDelta = new Vector2(_messageContent.sizeDelta.x, (messages.Count * 170));
            foreach (var data in messages)
            {
                var widget = data.user_id == LocalState.currentUser.uid ? _messageWidgetMy : _messageWidgetUser;
                var item = Instantiate(widget, _messageContent);
                item.Init(data.text, data.created_at);
                _messages.Add(item);
            }

            _lastUpdateTime = Time.time;
        }

        public void Update()
        {
            if (Time.time - _lastUpdateTime >= 2)
            {
                ServerCommand.GetFriendChat(_chatId, list =>
                {
                    if(list.Count != _messages.Count)
                        InitMessages(_chatId, list);
                });
                _lastUpdateTime = Time.time;
            }
        }

        void AddNewMessage()
        {
            ServerCommand.SendFriendChat(_chatId, _inputField.text);
            _inputField.text = "";
//            var item = Instantiate(_messageWidgetMy, _messageContent);
//            item.Init(Utils.GenerateRandomStr(), DateTime.Now.ToString());
//            _messages.Add(item);
        }

        void RefreshKeyboardSafeZone(int indentSize)
        {
            if (indentSize <= 0) {
                RefreshSafeZone(GamlaResourceManager.safeAreaManager.Indent);
            } else {
                var root = GetComponent<RectTransform>();
                root.anchoredPosition = new Vector2(0, indentSize);
            }
        }
    }
}