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
    public class LadderWindow : GUIView//, ICarouselRootView
    {
        [SerializeField] private ScrollRectCarouselView _carouselView;

        [SerializeField] private TopbarGame topbarGame;
        [SerializeField] private CurrencyFilter _currencyFilter;
        [SerializeField] private GameObject _currencyFilterEmpty;
        [SerializeField] private CurrencyFilter _currencyFilterHead;
        [SerializeField] private Text _amountCount;
        [SerializeField] private CurrencyBadge _amountBadge;
        [SerializeField] private Image _amountLogo;
        [SerializeField] private Text _description;
        [SerializeField] private Text _timeLeft;
        [SerializeField] private LadderWidget _userLadderWidget;
        
        //temp
        [SerializeField] private RectTransform _ladderContent;
        [SerializeField] private LadderWidget _ladderPrefab;
        private List<LadderWidget> _ladderWidgets = new List<LadderWidget>();
        private GameInfo _current_game;
        private DateTime time;

        public ScrollRectCarouselView carouselView => _carouselView;
        public GUIView carouselRootView => this;

        readonly GridDataSource<LadderInfo> _testDataSource = new GridDataSource<LadderInfo>(200, 1);
        CarouselPresenter _ladderListCarouselPresenter;
        List<LadderInfo> _fullLadder = new List<LadderInfo>();

        public void Start()
        {
            _currencyFilter.onCurrencyClick += FilterData;
            _currencyFilterHead.onCurrencyClick += FilterData;
            
            _currencyFilter.onUpdateEvent += _currencyFilterHead.ForceUpdateView;
            _currencyFilterHead.onUpdateEvent += _currencyFilter.ForceUpdateView;
            
            EventManager.OnGameInfoUpdate.Subscribe(UpdateData);
        }

        private void UpdateData()
        {
            if(this != null)
                Init(LocalState.currentGame, LocalState.currentUser);
        }

        public void Init(GameInfo current_game, UserInfo current_user)
        {
            _current_game = current_game;
            topbarGame.Init(current_game);
            
            if (current_game?.leagues?.leagues?.data == null || current_game.leagues.leagues.data.Count == 0)
            {
                //NO LEAGUE
                Debug.LogError("NO LEAGUE");
                return;
            }

            time = DateTime.Parse(current_game.leagues.leagues.data[0].end_at);
            _description.text = current_game.leagues.leagues.data[0].name;

            /*_ladderListCarouselPresenter = Utils.CreateGridViewPresenter(
                this,
                _testDataSource,
                LadderWidget.LoadPath,
                GridLineParams.@default,
                InitLadderWidget
            );*/
            Show();
            _fullLadder = current_game.ladder;
            FilterData(CurrencyType.USD);
            SetUserData(current_user);
        }

        private void LateUpdate()
        {
            var delta = time - DateTime.Now;
            _timeLeft.text = $"{delta.Days} {LocalizationManager.Text("gamla.main.time.days")} {delta.Hours}:{delta.Minutes}:{delta.Seconds}";
        }

        void InitLadderWidget(GUIView view, int row, int col)
        {
            var battleInfo = _testDataSource.GetData(row, col);
            LadderWidget ladderidget = view as LadderWidget;
            ladderidget.Init(battleInfo);
        }

        void SetUserData(UserInfo current_user)
        {
            var info = new LadderInfo
            {
                place = 56,
                amount = 200,
                currency = CurrencyType.USD,
                user = current_user.ToPublic()
            };
            _userLadderWidget.Init(info);
        }

        void FilterData(CurrencyType type)
        {
            if(_current_game?.leagues?.leagues?.data == null) return;
            if(_current_game?.leagues?.leagues?.data.Count == 0) return;
            
            bool isGoldLeague = true;
            if (type == CurrencyType.USD)
            {
                _amountCount.text = _current_game.leagues.leagues.data[0].award_usd + "";
                _amountBadge.Init(CurrencyType.USD);
                isGoldLeague = true;
            }

            if (type == CurrencyType.Z)
            {
                _amountCount.text = _current_game.leagues.leagues.data[0].award_z + "";
                _amountBadge.Init(CurrencyType.Z);
                isGoldLeague = false;
            }

            var sortedData = _fullLadder.Where(x => x.isGoldLeague == isGoldLeague).OrderByDescending(x => x.user.trophie).ToList();
            for (int i = 0; i < sortedData.Count; i++)
            {
                sortedData[i].place = i + 1;
            }
            SetSimpleData(sortedData);
        }

        void SetData(List<LadderInfo> ladder)
        {
            _testDataSource.UpdateData(ladder);
            //_testCarouselPresenter?.ReloadData();
            _ladderListCarouselPresenter?.Reset();
        }
        
        //temp
        void SetSimpleData(List<LadderInfo> ladder)
        {
            foreach (var existTournament in _ladderWidgets)
            {
                Destroy(existTournament.gameObject);
            }
            _ladderWidgets.Clear();

            _ladderContent.sizeDelta = new Vector2(_ladderContent.sizeDelta.x, (ladder.Count * _ladderPrefab.rect.sizeDelta.y + ladder.Count * 30));
            foreach (var data in ladder)
            {
                var item = Instantiate(_ladderPrefab, _ladderContent);
                item.Init(data);
                _ladderWidgets.Add(item);
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            EventManager.OnGameInfoUpdate.Del(UpdateData);
        }

    }
}
