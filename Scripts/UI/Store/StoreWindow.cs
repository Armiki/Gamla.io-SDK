using System;
using System.Collections.Generic;
using System.Linq;
using Gamla.Data;
using Gamla.Logic;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.UI
{
    public class StoreWindow : GUIView
    {
        public event Action onPromoCodeClick;

        [SerializeField] private TopbarGame topbarGame;
        
        [SerializeField] private RectTransform _storeHardContent;
        [SerializeField] private RectTransform _storeSoftContent;
        
        [SerializeField] private StoreWidget _storeHardWidgetPref;

        //[SerializeField] private Text _promocode;
        [SerializeField] private Button _enterPromocodeBtn;
        [SerializeField] private Button _copyPromocodeBtn;

        [SerializeField] public BonusDepositWidget _dailyReward;
        [SerializeField] public BonusDepositWidget _freeCoins;
        [SerializeField] public BonusDepositWidget _watchVideo;

        private string _generatePromocode;

        private List<StoreWidget> _storeSoftWidgetList= new List<StoreWidget>();
        private List<StoreWidget> _storeHardWidgetList= new List<StoreWidget>();

        public void Start()
        {
            _enterPromocodeBtn.onClick.RemoveAllListeners();
            _enterPromocodeBtn.onClick.AddListener(() =>
            {
                onPromoCodeClick?.Invoke();
            });
            
            _copyPromocodeBtn.onClick.RemoveAllListeners();
            _copyPromocodeBtn.onClick.AddListener(() =>
            {
                GUIUtility.systemCopyBuffer = _generatePromocode;
                UIMapController.OpenNotification(new ServerNotification()
                {
                    id = -1,
                    notification_id = -1,
                    short_text = LocalizationManager.Text( "gamla.redeemreferralcode.notification")
                });
                UIMapController.OpenPopupWindow(LocalizationManager.Text("gamla.redeemreferralcode.title"), 
                    LocalizationManager.Text("gamla.redeemreferralcode.copied"), 
                    LocalizationManager.Text("gamla.main.copy"), _generatePromocode, () =>
                {
                    GUIUtility.systemCopyBuffer = _generatePromocode;
                    UIMapController.OpenNotification(new ServerNotification()
                    {
                        id = -1,
                        notification_id = -1,
                        short_text = LocalizationManager.Text( "gamla.redeemreferralcode.notification")
                    });
                });
            });

            UpdateRewardUI();
            
            _dailyReward.onBonusClick += () =>
            {
                ServerCommand.CollectDailyReward(reward =>
                {
                    ServerCommand.GetOrUpdateProfile(LocalState.token);
                    UIMapController.OpenRewardWindow(new Pack(reward.payment));
                });
            };
            _freeCoins.onBonusClick += () =>
            {
                ServerCommand.CollectHourReward(reward =>
                {
                    ServerCommand.GetOrUpdateProfile(LocalState.token);
                    UIMapController.OpenRewardWindow(new Pack(reward.payment));
                });
            };
            
            EventManager.OnProfileUpdate.Subscribe(UpdateRewardUI);
            
            _watchVideo.Init(false, null, -1);
        }

        private void UpdateRewardUI()
        {
            _dailyReward.Init(LocalState.currentUser.timer_daily_bonus <= 0, LocalState.currentUser.dailyBonus, LocalState.currentUser.timer_daily_bonus);
            _freeCoins.Init(LocalState.currentUser.timer_hours_bonus <= 0, LocalState.currentUser.hoursBonus, LocalState.currentUser.timer_hours_bonus);
        }
        
        public void Init(GameInfo current_game, string generatePromocode,List<Pack> packs)
        {
            //_promocode.text = generatePromocode;
            topbarGame.Init(current_game);
            _generatePromocode = generatePromocode;

            ClearHardWidgets();
            ClearSoftWidgets();

            foreach (var pack in packs.Where(x=>x.main.type == CurrencyType.USD))
            {
                var item = Instantiate(_storeHardWidgetPref, _storeHardContent);
                item.Init(pack);
                item.onBuyClick += (pack) =>
                {
                    var validateWindow = UIMapController.OpenValidateWindow();
                    validateWindow.onCloseAction += GamlaResourceManager.tabBar.SelectPlay;
                    ServerCommand.AddPack(pack, validateWindow);
                };
                _storeHardWidgetList.Add(item);
            }
            
            /*foreach (var pack in packs.Where(x=>x.main.type == CurrencyType.Z))
            {
                var item = Instantiate(_storeSoftWidgetPref, _storeSoftContent);
                item.Init(pack);
                item.onBuyClick += ServerCommand.AddPack;
                _storeSoftWidgetList.Add(item);
            }*/
        }

        void ClearHardWidgets()
        {
            for (int i = _storeHardWidgetList.Count - 1; i >= 0; i--)
            {
                Destroy(_storeHardWidgetList[i].gameObject);
            }
            _storeHardWidgetList.Clear();
        }
        
        void ClearSoftWidgets()
        {
            for (int i = _storeSoftWidgetList.Count - 1; i >= 0; i--)
            {
                Destroy(_storeSoftWidgetList[i].gameObject);
            }
            _storeSoftWidgetList.Clear();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            EventManager.OnProfileUpdate.Del(UpdateRewardUI);
        }
    }
}
