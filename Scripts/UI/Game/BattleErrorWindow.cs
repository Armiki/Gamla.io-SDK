using UnityEngine;
using UnityEngine.UI;

namespace Gamla.UI
{
    public class BattleErrorWindow : GUIView
    {
        [SerializeField] private Button _addBalance;
        [SerializeField] private Button _dailyReward;
        [SerializeField] private Text _dailyRewardCount;
        [SerializeField] private CurrencyBadge _dailyRewardCurrency;
        [SerializeField] private Button _watchVideo;
        [SerializeField] private Text _watchVideoCount;
        [SerializeField] private CurrencyBadge _watchVideoCurrency;

        public void Init(bool isRewardActive, bool isLowBalance)
        {
            _watchVideo.gameObject.SetActive(!isLowBalance);
            _dailyReward.gameObject.SetActive(!isLowBalance);
            _addBalance.gameObject.SetActive(isLowBalance);
            _dailyReward.interactable = isRewardActive;
        }
    }
}