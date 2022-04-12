using System;
using System.Collections.Generic;
using Gamla.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.UI
{
    public class WithdrawWindow  : GUIView
    {
        public event Action onAddWithdraClick;
        public event Action<string, float, string> onWithdrawClick;
        
        [SerializeField] private Text _accountBalance;
        [SerializeField] private Text _bonusCash;
        [SerializeField] private Text _available;
        
        [SerializeField] private ValidateInputWidget _inputWidget;
        
        [SerializeField] private Text _processing;
        [SerializeField] private Text _totalAmount;
        
        [SerializeField] private Button _addWithDraw;
        [SerializeField] private Button _withDraw;
        
        [SerializeField] private RectTransform _withdrawContent;
        [SerializeField] private WithdrawWidget _withdrawPrefab;
        private List<WithdrawWidget> _withdrawWidgets = new List<WithdrawWidget>();
        
        private float _withdrawValue = 0;
        private CardHolderModel _currentCard;

        public override void OnEnable()
        {
            base.OnEnable();
            InitWidgets();
        }

        public void Start()
        {
            _accountBalance.text = LocalState.currentUser.wallet.hardTotal + "";
            _bonusCash.text = LocalState.currentUser.wallet.hardBonus + "";
            _available.text = LocalState.currentUser.wallet.hard + "";
            
            _inputWidget.onInputChange += UpdateWithdraw;
            UpdateWithdraw(LocalState.currentUser.wallet.hard + "");
            
            _addWithDraw.onClick.RemoveAllListeners();
            _addWithDraw.onClick.AddListener(()=> onAddWithdraClick?.Invoke());
            _withDraw.onClick.RemoveAllListeners();
            _withDraw.interactable = _currentCard != null;
            _withDraw.onClick.AddListener(()=>
            {
                if (_withdrawValue > 0 && _currentCard != null)
                {
                    onWithdrawClick?.Invoke(_currentCard.platform, _withdrawValue, _currentCard.data);
                }
            });

            _inputWidget.keyboardChecker.onKeyboardChange.RemoveAllListeners();
            _inputWidget.keyboardChecker.onKeyboardChange.AddListener(RefreshSafeZone);

            InitWidgets();
        }

        private void InitWidgets()
        {
            _withdrawContent.ClearChilds();
            _withdrawWidgets.Clear();
            CardHolderList cardInfos =
                JsonUtility.FromJson<CardHolderList>(
                    PlayerPrefs.GetString(LocalState.currentUser.innerUserInfo.email + "_cards", ""));
            if (cardInfos != null)
            {
                foreach (var cardHolder in cardInfos.list)
                {
                    var widget = Instantiate(_withdrawPrefab, _withdrawContent);
                    widget.Init(cardHolder, UpdateWithdrawWidget);
                    _withdrawWidgets.Add(widget);
                }
            }
        }

        private void UpdateWithdrawWidget(CardHolderModel cardHolderModel)
        {
            _currentCard = cardHolderModel;
            _withdrawWidgets.ForEach(w => w.UpdateView(cardHolderModel));
            _withDraw.interactable = _currentCard != null;
        }

        private void UpdateWithdraw(string value)
        {
            if (int.TryParse(value, out int result))
            {
                if (result <= LocalState.currentUser.wallet.hard)
                {
                    var delta = result * 0.3f;
                    _processing.text = delta + "";
                    _withdrawValue = result;
                    _totalAmount.text = (result - delta) + "";
                    _withDraw.interactable = true;
                }
                else
                {
                    _withDraw.interactable = false;
                }
            }
            else
            {
                _withDraw.interactable = false;
            }
        }
        
    }
}