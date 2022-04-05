using System;
using System.Collections.Generic;
using System.Linq;
using Gamla.Scripts.Common.UI;
using Gamla.Scripts.Data;
using Gamla.Scripts.Logic;
using UnityEngine;
using UnityEngine.UI;
using Gamla.Scripts.UI.Main;
using Random = UnityEngine.Random;

namespace Gamla.Scripts.UI.Profile
{
    public class PublicProfileWindow : GUIView
    {
        public event Action<long> toBattle;
        public event Action<long> sendMessage;
        public event Action<long> invite;
        
        [SerializeField] private UserProfileWidget _user;

        [SerializeField] private AvatarComponent _flag;
        //[SerializeField] private Image _onlineStatus;
        
        [SerializeField] private Button _addFriend;
        [SerializeField] private Button _chat;
        [SerializeField] private Button _battle;
        
        [SerializeField] private GameObject _newMsgIcon;
        
        [SerializeField] private GameObject _requestStatus;
        [SerializeField] private GameObject _friendsStatus;
        
        [SerializeField] private Button _contextBtn;
        [SerializeField] private GameObject _contextMenu;
        [SerializeField] private Button _shareProfile;
        [SerializeField] private Button _report;
        
        [SerializeField] private Text _matchesCountWin;
        [SerializeField] private Text _matchesCountTotal;
        
        [SerializeField] private GameLvlWidget[] _topGameLvls;
        [SerializeField] private Button _showMoreGameLvls;

        [SerializeField] private TrophieWidget[] _randomTrophies;
        
        [SerializeField] private Image _trophiesProgress;
        [SerializeField] private Text _trophiesCount;
        [SerializeField] private Button _showMoreTrophies;

        private long _id;
        private bool _isFriend;
        public void Start()
        {
            _battle.onClick.RemoveAllListeners();
            _battle.onClick.AddListener(() =>
            {
                toBattle?.Invoke(_id);
                Close();
            });
            
            _chat.onClick.RemoveAllListeners();
            _chat.onClick.AddListener(() =>
            {
                if (_isFriend)
                {
                    sendMessage?.Invoke(_id);
                    Close();
                }
                else
                {
                    UIMapController.OpenNotification(new ServerNotification()
                    {
                        id = -1,
                        notification_id = -1,
                        short_text = "User is not your friend"
                    });
                }
            });
            
            _addFriend.onClick.RemoveAllListeners();
            _addFriend.onClick.AddListener(() =>
            {
                invite?.Invoke(_id);
                Close();
            });
        }
        public void Init(long id)
        {
            _id = id;
            ServerCommand.GetPublicProfile(id, profile =>
            {
                if(gameObject == null || transform == null) return;
                
                _user.Init(profile);
                _matchesCountWin.text = profile.count_all_win.ToString();
                _matchesCountTotal.text = (profile.count_all_win + profile.count_all_lose).ToString();
                _flag.Load(profile.flag);

                var experience = UserGames.Convert(profile.experience);
                for (int i = 0; i < _topGameLvls.Length; i++)
                {
                    if (experience.Count > i)
                    {
                        _topGameLvls[i].Init(experience[i]);
                    }
                    else
                    {
                        _topGameLvls[i].gameObject.SetActive(false);
                    }
                }

                var trophies = profile.trophies.FindAll(trophie => trophie.pivot.count_actions == trophie.count_actions);
                for (int i = 0; i < _randomTrophies.Length; i++)
                {
                    if (i < trophies.Count)
                    {
                        _randomTrophies[i].Init(trophies[i]);
                    }
                }
                
                int total = 54;
                _trophiesProgress.fillAmount = profile.trophies.Count / (float) total;
                _trophiesCount.text = profile.trophies.Count + "/" + total;
                

                _isFriend = LocalState.friends.friends.Any(f => f.id == id);
                if (_isFriend)
                {
                    _friendsStatus.SetActive(true);
                }
                else
                {
                    // if send requst
                    //_requestStatus.SetActive(true);
                    _requestStatus.SetActive(false);
                }
            });
        }
        
    }
}