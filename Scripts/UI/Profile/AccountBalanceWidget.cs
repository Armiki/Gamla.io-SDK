using System;
using Gamla.Data;
using Gamla.Logic;
using Gamla.UI.Carousel;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.UI
{
    public class AccountBalanceWidget : BaseScrollElement
    {
         public static readonly string LoadPath = "Widgets/AccountBalanceWidget";

        [SerializeField] private Image _logo;
        [SerializeField] private Text _name;
        [SerializeField] private Text _status;
        [SerializeField] private Text _date;
        [SerializeField] private Text _id;
        [SerializeField] private Text _amount;
        [SerializeField] private Image _currencyLogo;
        [SerializeField] private RecolorItem _currencyRecolorLogo;
        [SerializeField] private RecolorItem _currencyRecolorBackLogo;
        
        public RectTransform rect;

        public void Init(AccountBalanceData data)
        {
            _name.text = data.comment;
            _status.text = LocaliseName(data.status);
            _date.text = DateTime.Parse(data.date).ToShortDateString();
            _id.text = data.status;//data.battleId;
            _amount.text = (data.currency.amount * SignValue(data.type)).ToString();
            
            switch (data.currency.type)
            {
                case CurrencyType.USD : _currencyLogo.sprite = GUIConstants.guiSettings.hard_icon; break;
                case CurrencyType.Z : _currencyLogo.sprite = GUIConstants.guiSettings.soft_icon; break;
                case CurrencyType.TICKETS : _currencyLogo.sprite = GUIConstants.guiSettings.ticket_icon; break;
            }
            
            //_currencyLogo.sprite = data.currency.type == CurrencyType.USD ? GUIConstants.guiSettings.hard_icon : GUIConstants.guiSettings.soft_icon;
            
            _currencyRecolorLogo.recolorFilter =  data.currency.type == CurrencyType.USD
                ? "hardColor" : "softColor";
            _currencyRecolorLogo.Recolor();
            
            _currencyRecolorBackLogo.recolorFilter =  data.currency.type == CurrencyType.USD
                ? "hardBackColor" : "softBackColor";
            _currencyRecolorBackLogo.Recolor();

        }

        string LocaliseName(string status)
        {
            switch (status)
            {
                case "replenishment":
                    return LocalizationManager.Text("gamla.withdraw.replenishment");
                case "commission":
                    return LocalizationManager.Text("gamla.withdraw.commission");
                case "debit":
                    return LocalizationManager.Text("gamla.withdraw.debit");
                default :
                    return status;
            }
        }

        int SignValue(string type)
        {
            switch (type)
            {
                case "replenishment": return 1;
                case "commission": return -1;
                case "debit": return -1;
                default :
                    return 1;
            }
        }
    }
}
