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
    public class AccountBalanceWindow : GUIView, ICarouselRootView
    {
        public event Action onWithdraClick;

        [SerializeField] private Text _balanceCount;
        [SerializeField] private Text _bonusCount;
        [SerializeField] private Button _withDraw;
        [SerializeField] private CurrencyFilter _currencyFilter;
        //filters
        [SerializeField] private ScrollRectCarouselView _carouselView;
        
        //temp
        [SerializeField] private RectTransform _ladderContent;
        [SerializeField] private AccountBalanceWidget _balancePrefab;
        private List<AccountBalanceWidget> _accountBalanceWidgets = new List<AccountBalanceWidget>();

        private List<AccountBalanceData> _fullAccountBalance = new List<AccountBalanceData>();

        public ScrollRectCarouselView carouselView => _carouselView;
        public GUIView carouselRootView => this;

        readonly GridDataSource<AccountBalanceData> _testDataSource = new GridDataSource<AccountBalanceData>(200, 1);
        CarouselPresenter _balanceCarouselPresenter;

        public void Start()
        {
            _currencyFilter.onCurrencyClick += FilterData;
            
            _withDraw.onClick.RemoveAllListeners();
            _withDraw.onClick.AddListener(()=> onWithdraClick?.Invoke());
        }

        public void Init(UserInfo current_user)
        {
            if (LocalState.currentUser.wallet != null) {
                _balanceCount.text = LocalState.currentUser.wallet.hard + "";
                _bonusCount.text = LocalState.currentUser.wallet.hardBonus + "";
            }

            _ladderContent.ClearChilds();
            _balanceCarouselPresenter = Utils.CreateGridViewPresenter(
               this,
               _testDataSource,
               AccountBalanceWidget.LoadPath,
               GridLineParams.@default,
               InitAccountBalanceWidget
           );
            Show();

            ServerCommand.GetBalanceTransaction(pages =>
            {
                _fullAccountBalance = AccountBalanceData.Convert(pages.usd_payments.data);
                _fullAccountBalance.AddRange(AccountBalanceData.Convert(pages.z_payments.data));
                _fullAccountBalance.AddRange(AccountBalanceData.Convert(pages.ticket_payments.data));
                
                _fullAccountBalance.Sort((a, b) => String.Compare(b.date, a.date, StringComparison.Ordinal));
                FilterData(CurrencyType.USD);
            });
        }

        void InitAccountBalanceWidget(GUIView view, int row, int col)
        {
            var balanceInfo = _testDataSource.GetData(row, col);
            AccountBalanceWidget gameWidget = view as AccountBalanceWidget;
            gameWidget.Init(balanceInfo);
        }
        
        void FilterData(CurrencyType type)
        {
            var sortedData = _fullAccountBalance.Where(x => x.currency.type == type).ToList();
            SetSimpleData(sortedData);
        }

        void SetData(List<AccountBalanceData> balanceData)
        {
            _testDataSource.UpdateData(balanceData);
            //_testCarouselPresenter?.ReloadData();
            _balanceCarouselPresenter?.Reset();
        }

        public void SetSimpleData(List<AccountBalanceData> balanceData)
        {
            if (balanceData == null) {
                Debug.LogError("AccountBalanceWindow.SetSimpleData: Empty balance data list");
                return;
            }

            foreach (var existTournament in _accountBalanceWidgets)
            {
                Destroy(existTournament.gameObject);
            }
            _accountBalanceWidgets.Clear();
            
            _ladderContent.sizeDelta = new Vector2(_ladderContent.sizeDelta.x, (balanceData.Count * _balancePrefab.rect.sizeDelta.y + balanceData.Count * 30));
            foreach (var data in balanceData)
            {
                if (data == null) {
                    Debug.LogError("AccountBalanceWindow.SetSimpleData: Empty balance data item");
                    return;
                }
                
                var item = Instantiate(_balancePrefab, _ladderContent);
                item.Init(data);
                _accountBalanceWidgets.Add(item);
            }
        }
    }
}
  
