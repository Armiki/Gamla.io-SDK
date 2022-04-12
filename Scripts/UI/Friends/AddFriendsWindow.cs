using UnityEngine;
using UnityEngine.UI;

namespace Gamla.UI
{
    public class AddFriendsWindow : GUIView
    {
        [SerializeField] private InputField _inputField;
        [SerializeField] private Button _seacrh;
        
        [SerializeField] private RectTransform _friendContent;
        [SerializeField] private AddFriendWidget _addFriendWidget;
    }
}