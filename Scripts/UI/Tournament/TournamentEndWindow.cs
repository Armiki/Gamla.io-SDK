using System.Collections.Generic;
using Gamla.Data;
using Gamla.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.Scripts.UI
{
    public class TournamentEndWindow : GUIView
    {
        [SerializeField] private Text _eventName;
        [SerializeField] private Text _userPlace;
        [SerializeField] private Text _userPrize;
        [SerializeField] private CurrencyBadge _userBadge;
        [SerializeField] private Text _time;

        [SerializeField] private List<TournamentUserInfo> _places;

        public void Init(ServerTournamentEndModel model)
        {
            _eventName.text = model.name;
            var meUser = model.places.Find(p => p.user.id == LocalState.currentUser.uid);
            _userPlace.text = meUser.place + "";
            _userPrize.text = meUser.reward_amount + "";
            _userBadge.Init(meUser.reward_currency);
            _time.text = model.end_at;

            for (int i = 0; i < model.places.Count; i++)
            {
                if (i  < _places.Count )
                {
                    _places[i].Init(model.places[i]);
                }
            }
            if(model.places.Count <= 3)
                _places[3].gameObject.SetActive(false);
        }
        
        public void Init(ServerLeagueEndModel model, bool gold)
        {
            _eventName.text = model.name;
            var places = gold ? model.places_gold : model.places_silver;
            var meUser = places.Find(p => p.user.id == LocalState.currentUser.uid);
            if (meUser != null) {
                _userPlace.text = meUser.place + "";
                _userPrize.text = meUser.reward_amount + "";
                _userBadge.Init(meUser.reward_currency);
            }

            _time.text = model.end_at;

            for (int i = 0; i < places.Count; i++)
            {
                if (i < _places.Count)
                {
                    _places[i].Init(places[i]);
                }
            }
            if(places.Count <= 3)
                _places[3].gameObject.SetActive(false);
        }
    }
}