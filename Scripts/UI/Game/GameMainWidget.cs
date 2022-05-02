using System;
using Gamla.Data;
using Gamla.Logic;
using Gamla.UI.Carousel;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.UI
{
    public class GameMainWidget : BaseScrollElement
    {
        public static readonly string LoadPath = "Widgets/GameMainWidget";
        public const float  GameSkinHeight = 200;
        public const float  GameRematchSkinHeight = 348;
        public const float  TournamentSkinHeight = 252;
        public const float  TournamentStartSkinHeight = 396;
        public const float  PvpSkinHeight = 348;
        public event Action onCollectHoldGift;
        
        [SerializeField] private GameObject _gameSkin;
        [SerializeField] private GameObject _tournamentSkin;
        [SerializeField] private GameObject _pvpSkin;
        
        // game
        [SerializeField] private AvatarComponent _logo;
        [SerializeField] private Text _status;
        [SerializeField] private Text _entryFee;
        [SerializeField] private Image _entryFeeIcon;
        [SerializeField] private GameObject _vsUserGO;
        [SerializeField] private Text _userName;
        [SerializeField] private GameObject _timeLeftGO;
        [SerializeField] private Text _timeLeftCount;
        [SerializeField] private GameObject _returnGO;
        [SerializeField] private Text _returnCount;
        [SerializeField] private GameObject _winGO;
        [SerializeField] private Text _winCount;
        [SerializeField] private CurrencyBadge _winBadge;
        [SerializeField] private Button _rematch;
        [SerializeField] private Button _rematchApply;
        [SerializeField] private Button _matchInfo;
        [SerializeField] private GameObject _winLbl;
        [SerializeField] private RecolorItem _winLblColor;
        //on hold
        [SerializeField] private GameObject _gift;
        
        //tournament
        [SerializeField] private Text _tournamentStatus; //next round / place / lost
        [SerializeField] private Text _tournamentSubStatus; // time / place
        [SerializeField] private Text _round; 
        [SerializeField] private AvatarComponent[] _tournamentPlayers;
        [SerializeField] private GameObject _morePlayersGO;
        [SerializeField] private Text _morePlayers;
        [SerializeField] private Text _tournamentEntryFee;
        [SerializeField] private Image _tournamentEntryFeeIcon;
        [SerializeField] private Text _tournamentWinnerGet;
        [SerializeField] private Image _tournamentWinnerGetIcon;
        
        [SerializeField] private Text _tournamentWinCount;
        [SerializeField] private CurrencyBadge _touenamentWinBadge;
        [SerializeField] private Button _tournamentPlay;
        
        //pvp
        [SerializeField] private AvatarComponent _pvpLogo;
        [SerializeField] private Button _pvpPlay;
        [SerializeField] private Text _pvpEntryFee;
        [SerializeField] private Image _pvpEntryFeeIcon;
        [SerializeField] private Text _pvpWinnerGet;
        [SerializeField] private Image _pvpWinnerGetIcon;

        private string _battleInfoIdTemp = "";

        public RectTransform rect;
        private HistoryBattleInfo _data;
        public HistoryBattleInfo data => _data;
        public GameObject gift => _gift;

        public void Init(HistoryBattleInfo data, bool isGift = false, bool rematchAvailable = true)
        {
            if(_battleInfoIdTemp == data.matchId) return;
            _data = data;
            
            _battleInfoIdTemp = data.matchId;
            _matchInfo.onClick.RemoveAllListeners();
            _matchInfo.onClick.AddListener(() =>
            {
                if(isGift)
                    onCollectHoldGift?.Invoke();
                else
                    UIMapController.OpenGameFinish(data);
            });
            _rematch.onClick.RemoveAllListeners();
            _rematch.onClick.AddListener(() =>
            {
                UIMapController.OpenSimpleWarningWindow(GUIWarningType.RematchRequest, () =>
                {
                    
                }, () =>
                {
                    ServerCommand.CreateMatchRequest(data.battleInfo.entry.amount, data.battleInfo.entry.type.ToString(), data.opponent.id);
                    UIMapController.OpenNotification(new ServerNotification()
                    {
                        id = -1,
                        notification_id = -1,
                        short_text = LocalizationManager.Text("gamla.notification.rematch.request") //
                    });

                    SetContentHeight(false);
                    if (_rematch != null) {
                        _rematch.gameObject.SetActive(false);
                    }

                    //For update battles list to hide rematch buttons on other battles with the opponent
                    EventManager.OnGameInfoUpdate.Push();
                    ClientManager.SaveRematchInfo(data.matchId);
                });
            });
            _userName.text = data.opponent.nickname;
            _entryFee.text = data.battleInfo.entry.amount.ToString();
            _timeLeftCount.text = data.timeLeft.ToString();
            _returnCount.text = data.returnCount.ToString();
            _winCount.text = data.battleInfo.win.amount.ToString();
            _winLbl.SetActive(data.status == BattleStatus.Win);

            _logo.gameObject.SetActive(true);
            if (data.opponent.id > 0)
            {
                if(_logo != null)
                    _logo.Load(data.opponent.image);
            }
            else
            {
                _logo.avatar.texture = GUIConstants.guiSettings.avatars[0].texture;
            }

            _entryFeeIcon.sprite =  data.battleInfo.entry.type == CurrencyType.Z
                ? GUIConstants.guiSettings.soft_icon
                : GUIConstants.guiSettings.hard_icon;
            
            _winBadge.Init(data.battleInfo.win.type, 2);
            
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, GameSkinHeight);

            if (rematchAvailable)
            {
                rematchAvailable = PlayerPrefs.GetInt("rematch_" + data.matchId, 0) == 0;
            }
            
            switch (data.status)
            {
                case BattleStatus.Waiting:
                    _vsUserGO.SetActive(true);
                    _timeLeftGO.SetActive(false);
                    _returnGO.SetActive(false);
                    _winGO.SetActive(false);
                    _rematch.gameObject.SetActive(false);
                    if (string.IsNullOrEmpty(data.me.score))
                    {
                        _winLbl.SetActive(true);
                        _winLblColor.SetNewRecolor("textColorLink");
                        _status.text = LocalizationManager.Text("gamla.battle.waityou.status");
                    }
                    else 
                        _status.text = LocalizationManager.Text("gamla.battle.waitopp.status");
                    break;
                case BattleStatus.Searching:
                    _timeLeftGO.SetActive(true);
                    _vsUserGO.SetActive(false);
                    _returnGO.SetActive(false);
                    _winGO.SetActive(false);
                    _rematch.gameObject.SetActive(false);
                    _status.text = LocalizationManager.Text("gamla.battle.searchopp.status");
                    break;
                case BattleStatus.Win:
                    _vsUserGO.SetActive(true);
                    _winGO.SetActive(true);
                    _timeLeftGO.SetActive(false);
                    _returnGO.SetActive(false);
                    _rematch.gameObject.SetActive(false);
                    _status.text = LocalizationManager.Text("gamla.battle.win.status");
                    break;
                case BattleStatus.Draw:
                    _vsUserGO.SetActive(true);
                    _winGO.SetActive(false);
                    _timeLeftGO.SetActive(false);
                    _returnGO.SetActive(false);
                    _rematch.gameObject.SetActive(false);
                    SetContentHeight(false);
                    _status.text = LocalizationManager.Text("gamla.battle.draw.status");
                    break;
                case BattleStatus.Lost:
                    _vsUserGO.SetActive(true);
                    _winGO.SetActive(false);
                    _timeLeftGO.SetActive(false);
                    _returnGO.SetActive(false);
                    _rematch.gameObject.SetActive(rematchAvailable);
                    SetContentHeight(rematchAvailable);
                    _status.text = LocalizationManager.Text("gamla.battle.lost.status");
                    break;
                case BattleStatus.NoOpponent:
                    _vsUserGO.SetActive(false);
                    _winGO.SetActive(false);
                    _timeLeftGO.SetActive(false);
                    _returnGO.SetActive(false);
                    _rematch.gameObject.SetActive(false);
                    _status.text = LocalizationManager.Text("gamla.battle.noopp.status");
                    break;
            }
            
            if (_gift != null && isGift)
            {
                _gift.SetActive(true);
                _winGO.SetActive(false);
            }
        }

        void SetContentHeight(bool rematchAvailable)
        {
            if (rect == null) {
                return;
            }
        
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, rematchAvailable? GameRematchSkinHeight: GameSkinHeight);
        }
        
        public void Init(ServerRequestMatch request, bool toUser)
        {
            _status.text = LocalizationManager.Text("gamla.widget.rematch.request");
            _matchInfo.onClick.RemoveAllListeners();
            _matchInfo.onClick.AddListener(() =>
            {
                if (toUser)
                {
                    UIMapController.OpenSimpleWarningWindow(GUIWarningType.RematchRequestCallback,
                        null,
                        () => { ServerCommand.AcceptRequest(request.id); },
                        () =>
                        {
                            ServerCommand.RejectRequest(request.id);
                            Destroy(gameObject);
                        });
                }
            });
            _rematchApply.gameObject.SetActive(toUser);
            if (toUser)
            {
                rect.sizeDelta = new Vector2(rect.sizeDelta.x, GameRematchSkinHeight);
            }

            _rematchApply.onClick.RemoveAllListeners();
            _rematchApply.onClick.AddListener(() =>
            {
                if (toUser)
                {
                    UIMapController.OpenSimpleWarningWindow(GUIWarningType.RematchRequestCallback,
                        null,
                        () => { ServerCommand.AcceptRequest(request.id); },
                        () => { ServerCommand.RejectRequest(request.id); Destroy(gameObject); });
                }
            });

            ServerCommand.GetUserPublic(toUser ? request.from_user : request.to_user, u =>
                {
                    if(_userName != null && _userName.gameObject != null)
                        _userName.text = u.nickname;
                });
            
            _entryFee.text = request.bet + "";// data.battleInfo.entry.amount.ToString();
            _timeLeftCount.text = "";
            _returnCount.text = "";
            _winCount.text = (request.bet * 2) + "";
            _winLbl.SetActive(true);
            if (request.status == "waiting")
            {
                _winLblColor.SetNewRecolor("textColorLink");
            }
            else if (request.status == "rejected")
            {
                _winLblColor.SetNewRecolor("textColorError");
            }
            else if (request.status == "accepted")
            {
                _winLblColor.SetNewRecolor("textColorHardSpecial");
            }

            _logo.gameObject.SetActive(true);
            _logo.avatar.texture = GamlaResourceManager.RematchIcon;
//            if (data.opponent.id > 0)
//            {
//                if(_logo != null)
//                    _logo.Load(data.opponent.image);
//            }
//            else
//            {
//                _logo.avatar.texture = GUIConstants.guiSettings.avatars[0].texture;
//            }
            
            _entryFeeIcon.sprite = request.currency == "Z"
                ? GUIConstants.guiSettings.soft_icon
                : GUIConstants.guiSettings.hard_icon;
            
            _winBadge.Init(request.currency == "Z" ? CurrencyType.Z : CurrencyType.USD, 2);
        }

        public void HoldInit()
        {
            _matchInfo.onClick.RemoveAllListeners();
            _matchInfo.onClick.AddListener(Collect);
            if (_gift != null)
            {
                _gift.SetActive(true);
                _winGO.SetActive(false);
            }
        }

        public void Collect()
        {
            onCollectHoldGift?.Invoke();
            _gift.SetActive(false);
            _winGO.SetActive(true);
        }
    }
}