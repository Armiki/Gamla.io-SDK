using Gamla.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.UI
{
    public class RewardWindow : GUIView
    {
        [SerializeField] private CurrencyBadge _currencyBadge;
        [SerializeField] private Text _amount;

        public void Init(float count, CurrencyType type)
        {
            _amount.text = "+" + count.ToString();
            _currencyBadge.Init(type, 3);
        }
    }
}