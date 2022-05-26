using Gamla.Data;
using Gamla.Logic;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.UI
{
    public class RequestWidget : MonoBehaviour
    {
        [SerializeField] private UserProfileWidget _user;
        [SerializeField] private Button _addBtn;
        [SerializeField] private Button _cancelBtn;
        [SerializeField] private Text _descriptionText;
        
        public RectTransform rect;
        private ServerPublicUser _data;

        public void Init(ServerPublicUser user, bool isOutRequest)
        {
            _data = user;
            _user.Init(user);
            if (isOutRequest)
            {
                _addBtn.gameObject.SetActive(false);
                _descriptionText.text = LocalizationManager.Text("gamla.widget.friend.request.to");
            }
            else
            {
                _descriptionText.text = LocalizationManager.Text("gamla.widget.friend.request.from");
            }

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