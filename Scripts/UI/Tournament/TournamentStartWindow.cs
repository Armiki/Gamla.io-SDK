using System;
using Gamla.Scripts.Common.UI;
using Gamla.Scripts.Data;
using Gamla.Scripts.UI.Main;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.Scripts.UI.Tournament
{
    public class TournamentStartWindow : GUIView
    {
        [SerializeField] private Text _title;

        [SerializeField] private CurrencyBadge _winnerCurrencyBadge;
        [SerializeField] private Text _winnerCurrencyValue;
        [SerializeField] private CurrencyBadge _finalistCurrencyBadge;
        [SerializeField] private Text _finalistCurrencyValue;

        [SerializeField] private Text _roundTime;
        [SerializeField] private Button _playBtn;

        [SerializeField] private Text _playerCount;
        [SerializeField] private CurrencyBadge _entryCurrencyBadge;
        [SerializeField] private Text _entryCurrencyValue;
        

        public void Init(ServerTournamentModel tournament, Action playBtn)
        {
            _title.text = tournament.name;
            var _winner = tournament.awards.Find(a => a.place == 1);
            _winnerCurrencyBadge.Init(_winner.currency == "Z" ? CurrencyType.Z : CurrencyType.USD);
            _winnerCurrencyValue.text = "" + _winner.amount;
            
            var _finalist = tournament.awards.Find(a => a.place == 2);
            _finalistCurrencyBadge.Init(_finalist.currency == "Z" ? CurrencyType.Z : CurrencyType.USD);
            _finalistCurrencyValue.text = "" + _finalist.amount;

            _playerCount.text = tournament.players_count + "";
            
            _playBtn.onClick.RemoveAllListeners();
            _playBtn.onClick.AddListener(playBtn.Invoke);
            
            _entryCurrencyBadge.Init(tournament.currency == "Z" ? CurrencyType.Z : CurrencyType.USD);
            _finalistCurrencyValue.text = "" + tournament.entry_cost;
        }
    }
}