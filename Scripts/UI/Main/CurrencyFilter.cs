using System;
using System.Collections;
using Gamla.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.UI
{
    public class CurrencyFilter : MonoBehaviour
    {
        public event Action<CurrencyType> onCurrencyClick;
        public event Action<CurrencyType> onUpdateEvent;

        public bool enableTickets = false;
        [SerializeField] private Button _soft;
        [SerializeField] private Button _hard;
        [SerializeField] private RecolorItem _softRecolor;
        [SerializeField] private RecolorItem _hardRecolor;
        [SerializeField] private RecolorItem _softRecolorText;
        [SerializeField] private RecolorItem _hardRecolorText;
        [SerializeField] private GameObject _ticketsGO;
        [SerializeField] private Button _tickets;
        [SerializeField] private RecolorItem _ticketsRecolor;
        [SerializeField] private RecolorItem _ticketRecolorText;

        public CurrencyType currentCurrency;
        public void Awake()
        {
            if(_soft != null)_soft.onClick.RemoveAllListeners();
            if(_soft != null)_soft.onClick.AddListener(() => SoftClick());
            
            if(_hard != null)_hard.onClick.RemoveAllListeners();
            if(_hard != null)_hard.onClick.AddListener(() => HardClick());

            if(_ticketsGO != null)_ticketsGO.SetActive(enableTickets);
            
            if(_tickets != null)_tickets.onClick.RemoveAllListeners();
            if(_tickets != null)_tickets.onClick.AddListener(() => TicketClick());
        }

        IEnumerator Start()
        {
            yield return new WaitForSeconds(0.1f);
            HardClick();
        }

        void SetInactiveBack(RecolorItem back)
        {
            if (back != null)
            {
                back.recolorFilter = "filterOffColor";
                back.Recolor();
            }
        }
        
        void SetActiveBack(RecolorItem back)
        {
            if (back != null)
            {
                back.recolorFilter = "filterOnColor";
                back.Recolor();
            }
        }
        
        void SetInActiveText(RecolorItem text)
        {
            if (text != null)
            {
                text.recolorFilter = "textColorTertiary";
                text.Recolor();
            }
        }
        
        void SetActiveText(RecolorItem text)
        {
            if (text != null)
            {
                text.recolorFilter = "textColorSecondary";
                text.Recolor();
            }
        }

        public void ForceUpdateView(CurrencyType types)
        {
            switch (types)
            {
                case CurrencyType.USD: HardClick(true); break;
                case CurrencyType.Z: SoftClick(true); break;
                case CurrencyType.TICKETS: TicketClick(true); break;
            }
        }

        void SoftClick(bool justViewUpdate = false)
        {
            SetActiveBack(_softRecolor);
            SetInactiveBack(_hardRecolor);
            SetInactiveBack(_ticketsRecolor);
            SetActiveText(_softRecolorText);
            SetInActiveText(_hardRecolorText);
            SetInActiveText(_ticketRecolorText);
            
            currentCurrency = CurrencyType.Z;
            if (!justViewUpdate)
            {
                onCurrencyClick?.Invoke(CurrencyType.Z);
                onUpdateEvent?.Invoke(CurrencyType.Z);
            }
        }
        
        void HardClick(bool justViewUpdate = false)
        {
            SetActiveBack(_hardRecolor);
            SetInactiveBack(_softRecolor);
            SetInactiveBack(_ticketsRecolor);
            SetActiveText(_hardRecolorText);
            SetInActiveText(_softRecolorText);
            SetInActiveText(_ticketRecolorText);
            
            currentCurrency = CurrencyType.USD;
            if (!justViewUpdate)
            {
                onCurrencyClick?.Invoke(CurrencyType.USD);
                onUpdateEvent?.Invoke(CurrencyType.USD);
            }
        }
        
        void TicketClick(bool justViewUpdate = false)
        {
            SetActiveBack(_ticketsRecolor);
            SetInactiveBack(_softRecolor);
            SetInactiveBack(_hardRecolor);
            SetActiveText(_ticketRecolorText);
            SetInActiveText(_softRecolorText);
            SetInActiveText(_hardRecolorText);
            
            currentCurrency = CurrencyType.TICKETS;
            if (!justViewUpdate)
            {
                onCurrencyClick?.Invoke(CurrencyType.TICKETS);
                onUpdateEvent?.Invoke(CurrencyType.TICKETS);
            }
        }
    }
}