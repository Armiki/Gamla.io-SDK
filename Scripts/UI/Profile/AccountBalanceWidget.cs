using Gamla.Scripts.Common.Carousel.ScrollElements;
using Gamla.Scripts.Data;
using Gamla.Scripts.UI.Main;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.Scripts.UI.Profile
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
            _name.text = data.game.name;
            _status.text = data.status.ToString();
            _date.text = data.date;
            _id.text = data.battleId;
            _amount.text = data.currency.amount.ToString();
            
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
    }
}
