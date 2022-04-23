using System;
using System.Collections.Generic;
using Gamla.Data;
using Gamla.Logic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Toggle = UnityEngine.UI.Toggle;

namespace Gamla.UI
{
    public class ProfileWindow : GUIView
    {
        public event Action onSignUpClick;
        public event Action onEditProfileClick;
        
        public event Action onFriendsClick;
        public event Action onAccountBalanceClick;
        public event Action onInfoClick;
        public event Action onBlockUsersClick;
        public event Action onLanguageClick;
        public event Action onRequestNewData;
        public event Action onShowMoreGameLvlsClick;
        public event Action onShowMoreTrophieClick;
        public event Action onCreateTournamentClick;
        public event Action onJoinTournamentClick;
        public event Action onAttachReferral;
        public event Action<bool> onNotificationUpdate;
        
        [SerializeField] private TopbarGame topbarGame;
        [SerializeField] private UserProfileWidget _user;
        [SerializeField] private Image _flag;
        [SerializeField] private Button _editProfileBtn;

        [SerializeField] private GameObject _signUpGO;
        [SerializeField] private Button _signUpBtn;

        [SerializeField] private GameLvlWidget[] _topGameLvls;
        [SerializeField] private Button _addNewGames;
        [SerializeField] private Button _showMoreGameLvls;

        [SerializeField] private TrophieWidget[] _randomTrophies;
        
        [SerializeField] private Image _trophiesProgress;
        [SerializeField] private Text _trophiesCount;
        [SerializeField] private Button _showMoreTrophies;


        [SerializeField] private Button _friends;
        [SerializeField] private Button _accountBalance;
        [SerializeField] private Toggle _notification;
        [SerializeField] private Button _infoBtn;
        [SerializeField] private Button _blockUsersBtn;
        [SerializeField] private Button _languageBtn;
        
        [SerializeField] private Button _createTournamentBtn;
        [SerializeField] private Button _joinTournamentBtn;
        [SerializeField] private Button _referralCodeBtn;

        [SerializeField] private Text _matchesCountWin;
        [SerializeField] private Text _matchesCountTotal;

        public void Start()
        {
            var flag = GUIConstants.guiSettings.GetFlagByCurrentCulture();
            if (flag != null)
            {
                _flag.sprite = flag;
            }
            _editProfileBtn.onClick.RemoveAllListeners();
            _editProfileBtn.onClick.AddListener(() => onEditProfileClick?.Invoke());
            
            _signUpBtn.onClick.RemoveAllListeners();
            _signUpBtn.onClick.AddListener(() => onSignUpClick?.Invoke());
            
            _friends.onClick.RemoveAllListeners();
            _friends.onClick.AddListener(() => onFriendsClick?.Invoke());
            
            _accountBalance.onClick.RemoveAllListeners();
            _accountBalance.onClick.AddListener(() => onAccountBalanceClick?.Invoke());
            
            _infoBtn.onClick.RemoveAllListeners();
            _infoBtn.onClick.AddListener(() => onInfoClick?.Invoke());
            
            _blockUsersBtn.onClick.RemoveAllListeners();
            _blockUsersBtn.onClick.AddListener(() => onBlockUsersClick?.Invoke());
            
            _languageBtn.onClick.RemoveAllListeners();
            _languageBtn.onClick.AddListener(() => onLanguageClick?.Invoke());
            
            _showMoreGameLvls.onClick.RemoveAllListeners();
            _showMoreGameLvls.onClick.AddListener(() => onShowMoreGameLvlsClick?.Invoke());
            
            _showMoreTrophies.onClick.RemoveAllListeners();
            _showMoreTrophies.onClick.AddListener(() => onShowMoreTrophieClick?.Invoke());
            
            _createTournamentBtn.onClick.RemoveAllListeners();
            _createTournamentBtn.onClick.AddListener(() => onCreateTournamentClick?.Invoke());
            
            _joinTournamentBtn.onClick.RemoveAllListeners();
            _joinTournamentBtn.onClick.AddListener(() => onJoinTournamentClick?.Invoke());
            
            _referralCodeBtn.onClick.RemoveAllListeners();
            _referralCodeBtn.onClick.AddListener(() => onAttachReferral?.Invoke());
            
            _addNewGames.onClick.RemoveAllListeners();
            _addNewGames.onClick.AddListener(() =>
            {
                GamlaResourceManager.tabBar.SelectGames();
                //TODO: check url from server
                //Application.OpenURL("https://gamla.io/games");
            });

            if (_notification != null)
            {
                _notification.onValueChanged.RemoveAllListeners();
                _notification.onValueChanged.AddListener(b =>
                {
                    onNotificationUpdate?.Invoke(b);
                });
            }

            EventManager.OnProfileUpdate.Subscribe(onRequestNewData);
        }

        private bool isDestroy;
        public override void OnDestroy()
        {
            isDestroy = true;
            base.OnDestroy();
            EventManager.OnProfileUpdate.Del(onRequestNewData);
        }
        
        public void Init(GameInfo current_game, UserInfo current_user)
        {
            topbarGame.Init(current_game);
            _user.Init(current_user);

            for (int i = 0; i < _topGameLvls.Length; i++)
            {
                if (current_user.games != null && current_user.games.Count > i) {
                    _topGameLvls[i].Init(current_user.games[i]);
                } else {
                    _topGameLvls[i].gameObject.SetActive(false);
                }
            }

            _matchesCountWin.text = "" + current_user.countAllWins;
            _matchesCountTotal.text = "" + current_user.countTotalPlays;

            _signUpGO.SetActive(current_user.guest);
            
            ServerCommand.GetTrophies(UpdateTrophies);
        }

        private void UpdateTrophies(ServerTrophies source)
        {
            if(isDestroy) return;
            var trophies = new List<ServerTrophiesModel>(source.trophies);
            trophies.AddRange(source.trophies_to_get);


            var ready = trophies.FindAll(t => t.pivot != null && t.pivot.count_actions == t.count_actions);
            var notReady = trophies.FindAll(t => t.pivot != null && t.pivot.count_actions != t.count_actions);
            int readyCount = ready.Count;

            var randReadyAll = readyCount > (_randomTrophies.Length - 2) ? (_randomTrophies.Length - 2) : readyCount;

            List<ServerTrophiesModel> currentTroph = new List<ServerTrophiesModel>();
            for (int i = 0; i < randReadyAll; i++)
            {
                
                var randomReady = Random.Range(0, readyCount);
                var contentTrophies = ready[randomReady];
                int c = 0;

                while (currentTroph.Contains(contentTrophies) && c < 10)
                {
                    randomReady = Random.Range(0, readyCount);
                    contentTrophies = ready[randomReady];
                    c++;
                }
                
                _randomTrophies[i].Init(contentTrophies);
                currentTroph.Add(contentTrophies);
            }
            
            currentTroph.Clear();

            var randNotReadyAll = _randomTrophies.Length - randReadyAll;
            for (int i = 0; i < randNotReadyAll; i++)
            {
                //var randomNotReady = Random.Range(0, notReady.Count);
                _randomTrophies[randReadyAll + i].Clear();//.Init(notReady[randomNotReady]);
            }
            
            int total = trophies.Count;
            _trophiesProgress.fillAmount = readyCount / (float) total;
            _trophiesCount.text = readyCount + "/" + total;
        }
    }
}
