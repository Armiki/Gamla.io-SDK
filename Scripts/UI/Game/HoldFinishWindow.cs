using System.Collections.Generic;
using Gamla.Data;
using Gamla.Logic;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.UI
{
    public class HoldFinishWindow : GUIView
    {
        [SerializeField] private AvatarComponent _avatar;
        [SerializeField] private Text _name;
        
        [SerializeField] private Text _hard;
        [SerializeField] private Text _soft;
        [SerializeField] private Text _tickets;
        
        [SerializeField] private Button _collectAll;
        
        [SerializeField] private RectTransform _battleContent;
        [SerializeField] private GameMainWidget _battlePrefab;
        private List<GameMainWidget> _battleWidgets = new List<GameMainWidget>();

        private float _collectAmountHard = 0;
        private float _collectAmountSoft = 0;
        private float _collectAmountTicket = 0;

        public void Start()
        {
            _collectAll.onClick.RemoveAllListeners();
            _collectAll.onClick.AddListener(CollectAll);

            _hard.text = _collectAmountHard.ToString();
            _soft.text = _collectAmountSoft.ToString();
            _tickets.text = _collectAmountTicket.ToString();
            
            _avatar.Load(LocalState.currentUser.avatarUrl);
            _name.text = LocalState.currentUser.name;
        }

        public void Init(List<HistoryBattleInfo> winBattles)
        {
            SetSimpleData(winBattles);
        }
        
        void SetSimpleData(List<HistoryBattleInfo> holdBattles)
        {
            if (_battleContent != null)
            {
                _battleContent.ClearChilds();
                _battleWidgets.Clear();
                
                _battleContent.sizeDelta = new Vector2(_battleContent.sizeDelta.x,
                    (holdBattles.Count * _battlePrefab.rect.sizeDelta.y + holdBattles.Count * 30));
                foreach (var data in holdBattles)
                {
                    var item = Instantiate(_battlePrefab, _battleContent);
                    item.Init(data, true);
                    item.rect.sizeDelta = new Vector2(GamlaResourceManager.canvas.pixelRect.width - 100, 200);
                    item.onCollectHoldGift += () => Collect(item);
                    _battleWidgets.Add(item);
                }
            }
        }

        void Collect(GameMainWidget item)
        {
            if (item.gift != null && item.gift.active)
            {
                if (item.data.battleInfo.win.type == CurrencyType.USD)
                {
                    _collectAmountHard += item.data.battleInfo.win.amount;
                    _hard.text = _collectAmountHard.ToString();
                }

                if (item.data.battleInfo.win.type == CurrencyType.Z)
                {
                    _collectAmountSoft += item.data.battleInfo.win.amount;
                    _soft.text = _collectAmountSoft.ToString();
                }
                
                item.gift.SetActive(false);
            }

            if (_battleWidgets.FindAll(item => item.gift == null || item.gift.active).Count == 0)
            {
                _collectAll.gameObject.SetActive(false);
            }
        }

        void CollectAll()
        {
            foreach (var bw in _battleWidgets)
            {
                if (bw != null)
                {
                    Collect(bw);
                }
            }
            _collectAll.gameObject.SetActive(false);
        }

        public void OnClose()
        {
            GamlaResourceManager.tabBar.SelectPlay();
        }
    }
}