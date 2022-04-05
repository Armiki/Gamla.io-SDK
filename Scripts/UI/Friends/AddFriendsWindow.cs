using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Gamla.Scripts.Common;
using Gamla.Scripts.Common.UI;
using Gamla.Scripts.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.Scripts.UI.Friends
{
    public class AddFriendsWindow : GUIView
    {
        [SerializeField] private InputField _inputField;
        [SerializeField] private Button _seacrh;
        
        [SerializeField] private RectTransform _friendContent;
        [SerializeField] private AddFriendWidget _addFriendWidget;
    }
}