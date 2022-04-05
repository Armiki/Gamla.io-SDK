using Gamla.Scripts.Common.Carousel.ScrollElements;
using Gamla.Scripts.Data;
using Gamla.Scripts.UI.Main;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.Scripts.UI.Ladder
{
    public class LadderWidget : BaseScrollElement
    {
        public static readonly string LoadPath = "Widgets/LadderWidget";

        [SerializeField] private Text _place;
        [SerializeField] private UserProfileWidget _user;
        [SerializeField] private Text _count;
        [SerializeField] private Text _amountCount;
        [SerializeField] private Image _amountLogo;
        [SerializeField] private RecolorItem _amountLogoColor;
        [SerializeField] private RecolorItem _amountLogoBack;
        
        public RectTransform rect;

        public void Init(LadderInfo info)
        {
            _place.text = info.place.ToString();
            _count.text = info.user.trophie.ToString();
            _amountCount.text = info.amount.ToString();
            _user.Init(info.user);

            if (info.currency == CurrencyType.USD)
            {
                _amountLogo.sprite = GUIConstants.guiSettings.hard_icon;
                _amountLogoColor.recolorFilter = "hardColor";
                _amountLogoBack.recolorFilter = "hardBackColor";
            }
            else if (info.currency == CurrencyType.TICKETS)
            {
                _amountLogo.sprite = GUIConstants.guiSettings.ticket_icon;
                _amountLogoColor.recolorFilter = "softColor";
                _amountLogoBack.recolorFilter = "softBackColor";
            }
            else if (info.currency == CurrencyType.Z)
            {
                _amountLogo.sprite = GUIConstants.guiSettings.soft_icon;
                _amountLogoColor.recolorFilter = "softColor";
                _amountLogoBack.recolorFilter = "softBackColor";
            }
            
            _amountLogoColor.Recolor();
            _amountLogoBack.Recolor();
        }
    }
}
