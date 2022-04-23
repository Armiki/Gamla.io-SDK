using Gamla.Data;
using Gamla.Logic;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.UI
{
    public class UserProfileWidget : MonoBehaviour
    {
        [SerializeField] private AvatarComponent _avatarUser;
        [SerializeField] private Text _nameUser;
        [SerializeField] private Button _button;

        private ServerPublicUser _user;

        private long _id;
        public void Start()
        {
            if (_button != null)
            {
                _button.onClick.RemoveAllListeners();
                _button.onClick.AddListener(() =>
                {
                    if (_id != LocalState.currentUser.uid && _user != null && !_user.IsEmpty())
                    {
                        EventManager.OnUserWidgetClick.Push(_user);   
                    }
                });
            }
        }

        public void Init(ServerPublicUser user)
        {
            _user = user;
            if (user != null)
            {
                _id = user.id;
                _nameUser.text = LocaliseName(user.nickname);
                _avatarUser.Load(user.image);
                _avatarUser.gameObject.SetActive(true);
                //_avatarUser.sprite = GUIConstants.guiSettings.avatars.FirstOrDefault(x => x.name == user.avatar);
            }
        }
        
        public void Init(ServerPublicProfile profile)
        {
            _id = profile.id;
            _nameUser.text = LocaliseName(profile.nickname);
            _avatarUser.Load(profile.image);
            _avatarUser.gameObject.SetActive(true);
        }
        
        public void Init(ServerPlayerMatch user)
        {
            if (user != null)
            {
                _id = user.id;
                _nameUser.text = LocaliseName(user.nickname);
                _avatarUser.Load(user.image);
                _avatarUser.gameObject.SetActive(true);
                //_avatarUser.sprite = GUIConstants.guiSettings.avatars.FirstOrDefault(x => x.name == user.avatar);
            }
        }

        public void Clear()
        {
            _user = null;
            _id = -1;
            _nameUser.text = LocaliseName("To Be Determined");
        }
        
        public void Init(UserInfo user)
        {
            if (user != null)
            {
                _nameUser.text = LocaliseName(user.name);
                _avatarUser.Load(user.avatarUrl);
            }
        }
        
        string LocaliseName(string userName)
        {
            switch (userName)
            {
                case "wait user":
                    return LocalizationManager.Text("gamla.window.tournamentboard.waituser");
                case "To Be Determined":
                    return LocalizationManager.Text("gamla.window.tournamentboard.determined");
                default :
                    return userName;
            }
        }
    }
}