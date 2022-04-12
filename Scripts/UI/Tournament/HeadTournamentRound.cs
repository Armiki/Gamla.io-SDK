using System;
using UnityEngine;
using UnityEngine.UI;
using Gamla.Data;
using Gamla.Logic;

namespace Gamla.UI
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
            _roundNum.text = LocalizationManager.Text("gamla.window.tournamentboard.place") + " " + model.place;
            _currency.text = model.amount + "";
            if (Enum.TryParse(model.currency.ToUpper(), out CurrencyType type))
            {
                _currencyBadge.Init(type);
            }
        }
        
        public void SetDataWinner(ServerTournamentAward model)
        {
            _roundNum.text = LocalizationManager.Text("gamla.widget.tournamentwinner.title");
            _currency.text = model.amount + "";
            if (Enum.TryParse(model.currency.ToUpper(), out CurrencyType type))
            {
                _currencyBadge.Init(type);
            }
        }
    }
}