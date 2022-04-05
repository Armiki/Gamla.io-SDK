using Gamla.Scripts.Data;
using Gamla.Scripts.Logic;
using Gamla.Scripts.UI.Main;
using UnityEngine;
using UnityEngine.UI;
namespace Gamla.Scripts.UI.Friends
{
    public class RequestWidget : MonoBehaviour
    {
        [SerializeField] private UserProfileWidget _user;
        [SerializeField] private Button _addBtn;
        [SerializeField] private Button _cancelBtn;
        
        public RectTransform rect;
        private ServerPublicUser _data;

        public void Init(ServerPublicUser user, bool isOutRequest)
        {
            _data = user;
            _user.Init(user);
            if(isOutRequest)
                _addBtn.gameObject.SetActive(false);
            
            _addBtn.onClick.RemoveAllListeners();
            _addBtn.onClick.AddListener(() => ServerCommand.FriendsRequestAccept(user.id));
            
            _cancelBtn.onClick.RemoveAllListeners();
            _cancelBtn.onClick.AddListener(() =>
            {
                ServerCommand.FriendsRequestReject(user.id);
                Destroy(gameObject);
            });
        }
    }
}