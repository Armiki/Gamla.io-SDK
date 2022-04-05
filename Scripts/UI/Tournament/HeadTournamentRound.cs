using System;
using System.Linq;
using Gamla.Scripts.Common.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Gamla.Scripts.Data;
using Gamla.Scripts.UI.Main;
using UnityEngine.EventSystems;

namespace GamlaSDK.Scripts.UI.Game
{
    public class HeadTournamentRound : MonoBehaviour
    {
        [SerializeField] private Text _roundNum;
        [SerializeField] private Text _currency;
        [SerializeField] private CurrencyBadge _currencyBadge;
        [SerializeField] private Button _leftBtn;
        [SerializeField] private Button _rightBtn;

        public void SetData(ServerTournamentAward model)
        {
            _roundNum.text = "Place " + model.place;
            _currency.text = model.amount + "";
            if (Enum.TryParse(model.currency.ToUpper(), out CurrencyType type))
            {
                _currencyBadge.Init(type);
            }
        }
        
        public void SetDataWinner(ServerTournamentAward model)
        {
            _roundNum.text = "WINNER!";
            _currency.text = model.amount + "";
            if (Enum.TryParse(model.currency.ToUpper(), out CurrencyType type))
            {
                _currencyBadge.Init(type);
            }
        }
    }
}