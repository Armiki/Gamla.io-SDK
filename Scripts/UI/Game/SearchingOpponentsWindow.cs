using System;
using UnityEngine;
using UnityEngine.UI;
using Gamla.Data;
using Gamla.Logic;

namespace Gamla.UI
{
    public class SearchingOpponentsWindow : GUIView
    {
        public event Action<BattleInfo> onPlayGameClick;
        
        [SerializeField] private UserProfileWidget _user1;
        [SerializeField] private UserProfileWidget _user2;
        [SerializeField] private Text _sec;
        [SerializeField] private Text _entryfee;
        [SerializeField] private Image _entryfeeCurrencyLogo;
        [SerializeField] private Text _winnerCount;
        [SerializeField] private Image _winnerCurrencyLogo;
        [SerializeField] private RecolorItem _winnerCurrencyBackLogo;
        [SerializeField] private Text _trophieCount;
        [SerializeField] private Text _expCount;
        [SerializeField] private Button _beginMatch;
        [SerializeField] private GameObject[] _backBtn;

        private float _timeToStart = 10;
        private bool _matchStarted = false;
        private bool _matchClicked = false;
        private BattleInfo _battleInfo;
        public void Start()
        {
            _beginMatch.onClick.RemoveAllListeners();
            _beginMatch.onClick.AddListener(() =>
            {
                StartMatch();
                onPlayGameClick?.Invoke(_battleInfo);
                _beginMatch.interactable = false;
                foreach (var btn in _backBtn)
                {
                    btn.SetActive(false);
                }
                _matchClicked = true;
            });
        }

        private void StartMatch()
        {
            if(_matchStarted) 
                return;
            _matchStarted = true;
            ServerCommand.CreateMatch(_battleInfo, OnCreateMatch);
        }

        private void OnCreateMatch(ServerMatchStart match)
        {
            if (this == null) {
                return;
            }

            if (match != null)
            {
                if (match.match.players.Count > 1)
                {
                    var opponent = match.match.players.Find(p => p.id != LocalState.currentUser.uid);
                    if (opponent != null)
                    {
                        _user2.Init(opponent);
                    }
                }

                if (_matchClicked)
                {
                    onPlayGameClick?.Invoke(_battleInfo);
                }
            }
        }

        public void SetCurrentUser(UserInfo current_user)
        {
            _user1.Init(current_user.ToPublic());
        }
        
        public void SetOpponentCarousel()
        {
            
        }

        private void Update()
        {
            if (_beginMatch.interactable)
            {
                _timeToStart -= Time.deltaTime;
                _sec.text = ((int) _timeToStart) + "";
                if (_timeToStart <= 0)
                {
                    onPlayGameClick?.Invoke(_battleInfo);
                    _beginMatch.interactable = false;
                }

                if (_timeToStart < 3)
                {
                    StartMatch();
                }
                foreach (var btn in _backBtn)
                {
                    btn.SetActive(_timeToStart > 3);
                }
            }
        }

        public void InitBattleInfo(BattleInfo info)
        {
            _battleInfo = info;
            _entryfee.text = info.entry.amount.ToString();
            _entryfeeCurrencyLogo.sprite = info.entry.type == CurrencyType.USD
                ? GUIConstants.guiSettings.hard_icon
                : GUIConstants.guiSettings.soft_icon;
            _winnerCount.text = info.win.amount.ToString();
            _winnerCurrencyLogo.sprite = info.win.type == CurrencyType.USD
                ? GUIConstants.guiSettings.hard_icon
                : GUIConstants.guiSettings.soft_icon;
            _winnerCurrencyBackLogo.recolorFilter = info.win.type == CurrencyType.Z
                ? "softBackColor"
                : "hardBackColor";
            _winnerCurrencyBackLogo.Recolor();
            _trophieCount.text = info.trophie.ToString();
            _expCount.text = info.exp.ToString();
        }
    }
}
