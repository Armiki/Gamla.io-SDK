using System.Collections.Generic;
using System.Linq;
using Gamla.Data;
using Gamla.Logic;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.UI
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
            foreach (var data in messages)
            {
                if (_messages.Any(m => m.Id == data.id)) {
                    continue;
                }
                
                var widget = data.user_id == LocalState.currentUser.uid ? _messageWidgetMy : _messageWidgetUser;
                var item = Instantiate(widget, _messageContent);
                item.Init(data.id, data.text, data.created_at);
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
            
            //var item = Instantiate(_messageWidgetMy, _messageContent);
            //item.Init( UnityEngine.Random.Range(0, int.MaxValue),Utils.GenerateRandomStr(), DateTime.Now.ToString());
            //_messages.Add(item);
        }

        void RefreshKeyboardSafeZone(int indentSize)
        {
            var root = GetComponent<RectTransform>();
            root.anchoredPosition = new Vector2(0, indentSize);
        }
    }
}