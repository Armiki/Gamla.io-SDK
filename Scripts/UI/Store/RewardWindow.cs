using Gamla.Scripts.Common.UI;
using Gamla.Scripts.Data;
using Gamla.Scripts.UI.Main;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.Scripts.UI.Store
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