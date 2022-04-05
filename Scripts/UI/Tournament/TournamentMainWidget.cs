using System;
using System.Linq;
using Gamla.Scripts.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.Scripts.UI.Main
{
    public class TournamentMainWidget : MonoBehaviour
    {
        public event Action onTournamentClick;
        
        [SerializeField] private Text _winCount;
        [SerializeField] private Image _winCurrency;
        [SerializeField] private Image _winCurrencyBack;
        [SerializeField] private Text _playersCount;
        [SerializeField] private Text _entry;
        [SerializeField] private Image _entryCurrency;
        [SerializeField] private Button _tournamentBtn;
        [SerializeField] private GameObject _newLable;
        [SerializeField] private Image _statusGo;

        public void Start()
        {
            _tournamentBtn.onClick.RemoveAllListeners();
            _tournamentBtn.onClick.AddListener(() => onTournamentClick?.Invoke());
        }
        public void Init(ServerTournamentModel tournamentInfo)
        {
            _playersCount.text = tournamentInfo.players_count.ToString();
            _newLable.SetActive(!tournamentInfo.isJoined);

            tournamentInfo.awards.Sort(((a1, a2) => a2.place.CompareTo(a1.place)));
            var award = tournamentInfo.awards[tournamentInfo.awards.Count - 1];
            
            if (award.currency == CurrencyType.USD.ToString())
            {
                _winCurrency.sprite = GUIConstants.guiSettings.hard_icon;
                _winCurrencyBack.color = GUIConstants.colorTemplate.colors.First(x => x.use).hardBackColorSpecialMono;
            }
            else if (award.currency == CurrencyType.TICKETS.ToString())
            {
                _winCurrency.sprite = GUIConstants.guiSettings.ticket_icon;
                _winCurrencyBack.color = GUIConstants.colorTemplate.colors.First(x => x.use).softBackColorSecondary;
            }
            else if (award.currency == CurrencyType.Z.ToString())
            {
                _winCurrency.sprite = GUIConstants.guiSettings.soft_icon;
                _winCurrencyBack.color = GUIConstants.colorTemplate.colors.First(x => x.use).softBackColorSecondary;
            }
            
            _winCount.text = award.amount.ToString();
            _winCurrency.color = GUIConstants.colorTemplate.colors.First(x => x.use).textColorSecondary;
            
            _entry.text = tournamentInfo.entry_cost.ToString();
            _entryCurrency.sprite = tournamentInfo.currency == "Z"
                ? GUIConstants.guiSettings.soft_icon
                : GUIConstants.guiSettings.hard_icon;
            
            _statusGo.gameObject.SetActive(tournamentInfo.isJoined);
            bool isCanPlay = tournamentInfo.matches.Any(m => m.players.Any(p => p.id == LocalState.currentUser.uid && p.score == 0));
            _statusGo.color = isCanPlay
                ? GUIConstants.colorTemplate.colors.First(x => x.use).textColorHardSpecial
                : GUIConstants.colorTemplate.colors.First(x => x.use).textColorSoftSpecial;
        }
    }
}