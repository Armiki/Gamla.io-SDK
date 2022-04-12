using Gamla.Data;
using Gamla.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.Scripts.UI
{
    public class TournamentUserInfo : MonoBehaviour
    {
        [SerializeField] private UserProfileWidget _profile;
        [SerializeField] private Text _place;
        [SerializeField] private Text _score;
        [SerializeField] private Text _prize;
        [SerializeField] private CurrencyBadge _badge;

        public void Init(ServerLeagueTournamentEndPlace place)
        {
            _place.text = place.place + "";
            if(string.IsNullOrEmpty(place.score))
                _score.gameObject.SetActive(false);
            _score.text = place.score;
            _prize.text = place.reward_amount + "";
            _badge.Init(place.reward_currency);
            _profile.Init(place.user);
        }
    }
}