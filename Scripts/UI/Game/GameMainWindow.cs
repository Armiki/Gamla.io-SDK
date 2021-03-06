using System;
using System.Collections.Generic;
using System.Linq;
using Gamla.Data;
using Gamla.Logic;
using Gamla.UI.Carousel;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.UI
{
    public class GameMainWindow : GUIView //, ICarouselRootView
    {
        public event Action onOpenBattleList;
        public event Action<ServerTournamentModel> onOpenTournamentList;
        
        [SerializeField] private TopbarGame _topbarGame;
        [SerializeField] private ScrollRectCarouselView _carouselView;
        [SerializeField] private RefreshScroll _refreshScroll;

        [SerializeField] private GameObject _tournamentTitleGO;
        [SerializeField] private GameObject _tournamentGO;
        [SerializeField] private GameObject _notificationGO;
        [SerializeField] private Button _activateNotifBtn;
        [SerializeField] private Button _rejectNotifBtn;
        
        [SerializeField] private Image _backImg;
        [SerializeField] private Button _newGameBtn;
        [SerializeField] private Button _newGameHeadBtn;

        [SerializeField] private TournamentMainWidget _tournamentPrefab;
        [SerializeField] private RectTransform _tournamenContent;

        //temp
        [SerializeField] private RectTransform _battleContent;
        [SerializeField] private GameMainWidget _battlePrefab;
        private List<GameMainWidget> _battleWidgets = new List<GameMainWidget>();

        public ScrollRectCarouselView carouselView => _carouselView;
        public GUIView carouselRootView => this;
        private List<TournamentMainWidget> _tournamentWidgets = new List<TournamentMainWidget>();

        public void Start()
        {
            _newGameBtn.onClick.RemoveAllListeners();
            _newGameBtn.onClick.AddListener(() => onOpenBattleList?.Invoke());
            _newGameHeadBtn.onClick.RemoveAllListeners();
            _newGameHeadBtn.onClick.AddListener(() => onOpenBattleList?.Invoke());
            _refreshScroll.onRefresh += OnRefrereshScroll;
            EventManager.OnGameInfoUpdate.Subscribe(UpdateData);
            EventManager.OnTournamentsUpdated.Subscribe(InitTournaments);
            //ServerCommand.GetOrUpdateMatches();
            if (LocalState.battleHistoryList.Count > 0)
            {
                UpdateData();
            }
            
            InvokeRepeating("UpdateProfile", 10, 10);
            UIMapController.CloseSpinner();
        }

        private void OnRefrereshScroll()
        {
            UpdateProfile();
        }

        private void UpdateProfile()
        {
            ServerCommand.GetOrUpdateProfile(LocalState.token);
            //ServerCommand.GetOrUpdateMatches();
        }

        private void UpdateData()
        {
            _topbarGame.Init(LocalState.currentGame);
            SetSimpleData();
            InitTournaments();
        }

        public override void OnEnable()
        {
            base.OnEnable();

        }

        private void InitTournaments()
        {
            // var existTournaments = LocalState.tournaments != null && LocalState.tournaments.Count > 0;
            // _tournamentTitleGO.SetActive(existTournaments);
            // _tournamentGO.SetActive(existTournaments);
            foreach (var existTournament in _tournamentWidgets)
            {
                Destroy(existTournament.gameObject);
            }
            _tournamentWidgets.Clear();

            foreach (var tournament in LocalState.tournaments)
            {
                if (tournament.status != "finished" && tournament.status != "cancelled" && !tournament.isJoined)
                {
                    var item = Instantiate(_tournamentPrefab, _tournamenContent);
                    item.Init(tournament);
                    item.onTournamentClick += () =>
                    {
                        onOpenTournamentList?.Invoke(tournament); //Use Ids instead of referencess?
                    };
                    _tournamentWidgets.Add(item);
                }
            }
            
            _tournamentTitleGO.SetActive(_tournamentWidgets.Count > 0);
            _tournamentGO.SetActive(_tournamentWidgets.Count > 0);
        }

        readonly GridDataSource<HistoryBattleInfo> _testDataSource = new GridDataSource<HistoryBattleInfo>(200, 1);
        CarouselPresenter _gameListCarouselPresenter;
        public void Init(GameInfo current_game)
        {
           /*_gameListCarouselPresenter = Utils.CreateGridViewPresenter(
                this,
                _testDataSource,
                GameMainWidget.LoadPath,
                GridLineParams.@default,
                InitBattleWidget
            );*/
            Show();
//            _topbarGame.Init(current_game);
//            SetSimpleData();

            InitTournaments();
        }

        void InitBattleWidget(GUIView view, int row, int col)
        {
            var battleInfo = _testDataSource.GetData(row, col);
            GameMainWidget gameWidget = view as GameMainWidget;
            gameWidget.Init(battleInfo);
        }

        void SetData()
        {
            var sortedData = LocalState.battleHistoryList.OrderByDescending(x => x.date).ToList();
            _testDataSource.UpdateData(sortedData);
            //_testCarouselPresenter?.ReloadData();
            _gameListCarouselPresenter?.Reset();
        }
        
        //temp
        void SetSimpleData()
        {
            if (_battleContent != null)
            {
                ServerCommand.GetMatchRequests(requests =>
                {
                    if(this ==  null || gameObject == null || !gameObject.activeInHierarchy)
                        return;
                    
                    foreach (var existTournament in _battleWidgets)
                    {
                        if(existTournament != null && existTournament.gameObject != null)
                            Destroy(existTournament.gameObject);
                    }
                    _battleWidgets.Clear();
                    var size = 0f;

                    var sortedData = LocalState.battleHistoryList.OrderByDescending(x => string.IsNullOrEmpty(x.me.score) && x.status == BattleStatus.Waiting).ThenByDescending(x => x.date).ToList();
                    var sortedTournament = LocalState.tournaments.FindAll(t => /*(t.status == "finished" | t.status == "cancelled") &&*/ t.isJoined).OrderByDescending(t => t.end_at);
                    List<MatchStory> totalSortedData = new List<MatchStory>();
                    totalSortedData.AddRange(sortedData);
                    totalSortedData.AddRange(sortedTournament);
                    totalSortedData = totalSortedData.OrderByDescending(t => t.end_at).ToList();

                    var to = requests.to.data.FindAll(r => r.game_id == ClientManager.gameId && r.status == "waiting");
                    var from = requests.from.data.FindAll(r => r.game_id == ClientManager.gameId && r.status == "waiting");
                    foreach (var data in totalSortedData)
                    {
                        var valid = data is HistoryBattleInfo;
                        if (valid)
                        {
                            var value = (HistoryBattleInfo) data;
                            var item = Instantiate(_battlePrefab, _battleContent);
                            bool rematchAvailable = from.All(r => r.to_user != value.opponent.id);
                            item.Init(value, false, rematchAvailable);
                            _battleWidgets.Add(item);
                            size += item.rect.sizeDelta.y;
                        }
                        
                        valid = data is ServerTournamentModel;
                        if (valid)
                        {
                            var value = (ServerTournamentModel) data;
                            var item = Instantiate(_battlePrefab, _battleContent);
                            item.Init(value);
                            _battleWidgets.Add(item);
                            size += item.rect.sizeDelta.y;
                        }
                    }

                    _battleContent.sizeDelta = new Vector2(_battleContent.sizeDelta.x,
                        (size + totalSortedData.Count * 30) + 30) ;

                    foreach (var request in from)
                    {
                        var item = Instantiate(_battlePrefab, _battleContent);
                        item.transform.SetAsFirstSibling();
                        item.Init(request, false);
                        _battleWidgets.Add(item);
                        size += item.rect.sizeDelta.y;
                    }
                    
                    foreach (var request in to)
                    {
                        var item = Instantiate(_battlePrefab, _battleContent);
                        item.transform.SetAsFirstSibling();
                        item.Init(request, true);
                        _battleWidgets.Add(item);
                        size += item.rect.sizeDelta.y;
                    }

                    _battleContent.sizeDelta = new Vector2(_battleContent.sizeDelta.x,
                        (size + totalSortedData.Count * 30));
                });
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            EventManager.OnGameInfoUpdate.Del(UpdateData);
            EventManager.OnTournamentsUpdated.Del(InitTournaments);
            _refreshScroll.onRefresh -= OnRefrereshScroll;
        }
    }
}
