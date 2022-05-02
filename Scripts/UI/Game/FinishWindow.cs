using System;
using Gamla.Data;
using Gamla.Logic;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.UI
{
    public class FinishWindow : GUIView
    {
        public event Action onPlayAgain;
        public event Action onPlay;
        
        [SerializeField] private Sprite _winSmile;
        [SerializeField] private Sprite _lostSmile;
        [SerializeField] private Sprite _waitSmile;
        
        [SerializeField] private Text _resultStatus;
        [SerializeField] private Image _resultImage;
        [SerializeField] private Text _resultDate;
        [SerializeField] private Text _winCount;
        [SerializeField] private CurrencyBadge _winLogo;
        
        [SerializeField] private Text _trophieCount;
        [SerializeField] private Text _expCount;
        [SerializeField] private Text _ticketCount;

        [SerializeField] private UserProfileWidget _user1;
        [SerializeField] private Text _pointUser1;
        
        [SerializeField] private UserProfileWidget _user2;
        [SerializeField] private Text _pointUser2;
        
        [SerializeField] private Text _entryFee;
        [SerializeField] private Image _entryIcon;
        [SerializeField] private Text _matchId;
        [SerializeField] private Button _copy;
        
        [SerializeField] private Button _showRecord;
        
        [SerializeField] private Button _restart;
        [SerializeField] private Button _play;

        private HistoryBattleInfo _historyBattleInfo;
        public void Start()
        {
            _restart.onClick.RemoveAllListeners();
            _restart.onClick.AddListener(() =>
            {
                onPlayAgain?.Invoke();
            });
            _copy.onClick.RemoveAllListeners();
            _copy.onClick.AddListener(() =>
            {
                GUIUtility.systemCopyBuffer = _historyBattleInfo.matchId;
            });
            _play.onClick.RemoveAllListeners();
            _play.onClick.AddListener(() =>
            {
                onPlay?.Invoke();
            });
            
            EventManager.OnGameInfoUpdate.Subscribe(ForceUpdateView);
        }

        private void ForceUpdateView()
        {
            if (_historyBattleInfo != null)
            {
                var match = LocalState.currentGame.history.Find(m => m.matchId == _historyBattleInfo.matchId);
                if (match != null)
                {
                    Init(match, LocalState.currentUser);
                }
            }
        }
        public void Init(HistoryBattleInfo historyBattleInfo, UserInfo current_user)
        {
            _historyBattleInfo = historyBattleInfo;
            _matchId.text = historyBattleInfo.matchId;
            
            _winCount.gameObject.SetActive(historyBattleInfo.status == BattleStatus.Win || historyBattleInfo.status == BattleStatus.Waiting);
            _resultDate.text = historyBattleInfo.date.ToShortDateString() + " " +
                               historyBattleInfo.date.ToShortTimeString();

            _restart.gameObject.SetActive(historyBattleInfo.status == BattleStatus.Lost);
            _play.gameObject.SetActive(historyBattleInfo.status == BattleStatus.Waiting && string.IsNullOrEmpty(historyBattleInfo.me.score));

            switch (historyBattleInfo.status)
            {
                case BattleStatus.Win:
                    _resultImage.sprite = _winSmile;
                    _resultStatus.text = LocalizationManager.Text("gamla.battle.win.status");
                    break;
                case BattleStatus.Draw:
                    _resultImage.sprite = _lostSmile;
                    _resultStatus.text = LocalizationManager.Text("gamla.battle.draw.status");
                    break;
                case BattleStatus.Lost:
                    _resultImage.sprite = _lostSmile;
                    _resultStatus.text = LocalizationManager.Text("gamla.battle.lost.status");
                    break;
                case BattleStatus.Waiting:
                    _resultImage.sprite = _waitSmile;
                    _resultStatus.text = LocalizationManager.Text( string.IsNullOrEmpty(historyBattleInfo.me.score) ? "gamla.battle.waityou.status" : "gamla.battle.waitopp.status");
                    break;
            }
            
            _user1.Init(current_user.ToPublic());
            _pointUser1.text = historyBattleInfo.status == BattleStatus.Draw ? (historyBattleInfo.drawScore  + "") : historyBattleInfo.me.score + "";
            _user2.Init(historyBattleInfo.opponent);
            _pointUser2.text = historyBattleInfo.status == BattleStatus.Draw ? (historyBattleInfo.drawScore  + "") : historyBattleInfo.opponent.score + "";

            _trophieCount.text = historyBattleInfo.status == BattleStatus.Win ? "2" : "0";// historyBattleInfo.battleInfo.trophie.ToString();
            _expCount.text = historyBattleInfo.battleInfo.exp.ToString();
            _ticketCount.text = historyBattleInfo.battleInfo.tickets.ToString();

            _winCount.text = historyBattleInfo.battleInfo.win.amount.ToString();
            _winLogo.Init(historyBattleInfo.battleInfo.win.type, 1);
            
            _entryFee.text = historyBattleInfo.battleInfo.entry.amount.ToString();
            _entryIcon.sprite = historyBattleInfo.battleInfo.entry.type == CurrencyType.USD
                ? GUIConstants.guiSettings.hard_icon
                : GUIConstants.guiSettings.soft_icon;
            
            if(!gameObject.activeSelf)
                Show();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            EventManager.OnGameInfoUpdate.Del(ForceUpdateView);
        }
    }
}