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
        [SerializeField] private RawImage _flag;

        private ServerPublicUser _user;
        private ServerPublicProfile _profile;
        private string _flagUrl;

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

            ReLoadFlag();
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
                _flagUrl = user.flag;
            }
        }
        
        public void Init(ServerPublicProfile profile)
        {
            _profile = profile;
            _id = profile.id;
            _nameUser.text = LocaliseName(profile.nickname);
            _avatarUser.Load(profile.image);
            _avatarUser.gameObject.SetActive(true);
            _flagUrl = profile.flag;
        }
        
        public void Init(ServerPlayerMatch user)
        {
            if (user != null)
            {
                _id = user.id;
                _nameUser.text = LocaliseName(user.nickname);
                _avatarUser.Load(user.image);
                _avatarUser.gameObject.SetActive(true);
                _flagUrl = "";
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
                _flagUrl = user.flagUrl;
            }
        }
        
        void OnEnable()
        {
            //Todo: Load on init, and reload when language changed 
            ReLoadFlag();
        }
        
        void ReLoadFlag()
        {
            if (_flag != null)
            {
                if (_flagUrl != null && _flagUrl.Contains("gamla"))
                {
                    RemoteResourceManager.LoadToRawImage(_flag, _flagUrl);
                    return;
                }

                string code = "us";
                if (LocalizationManager.CurrentLanguage == "russian") {
                    code = "ru";
                }
                
                var flagUrl = $"https://gamla.io/assets/img/flugs/{code}.png";
                RemoteResourceManager.LoadToRawImage(_flag, flagUrl);
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