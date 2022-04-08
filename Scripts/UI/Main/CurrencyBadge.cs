using Gamla.Scripts.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.Scripts.UI.Main
{
    public class CurrencyBadge : MonoBehaviour
    {
        [SerializeField] private Image _currencyImage;
        [SerializeField] private RecolorItem _currencyRecolorImage;
        [SerializeField] private RecolorItem _currencyRecolorBackImage;

        public void Init(string type)
        {
            if (CurrencyType.TryParse(type, true, out CurrencyType _type))
            {
                Init(_type);
            }
        }
        public void Init(CurrencyType type, int q = 1)
        {
            switch (type)
            {
                case CurrencyType.USD : _currencyImage.sprite = GUIConstants.guiSettings.hard_icon; break;
                case CurrencyType.Z : _currencyImage.sprite = GUIConstants.guiSettings.soft_icon; break;
                case CurrencyType.TICKETS : _currencyImage.sprite = GUIConstants.guiSettings.ticket_icon; break;
            }

            var iconSoftColor = "";
            var iconHardColor = "";
            var backSoftColor = "";
            var backHardColor = "";
            
            switch (q)
            {
                case 1:
                    iconSoftColor = "softColor";
                    iconHardColor = "hardColor";
                    
                    backSoftColor = "softBackColor";
                    backHardColor = "hardBackColor";
                    break;
                case 2:
                    iconSoftColor = "softColorSecondary";
                    iconHardColor = "hardColorSecondary";
                    
                    backSoftColor = "softBackColorSecondary";
                    backHardColor = "hardBackColorSecondary";
                    break;
                case 3:
                    iconSoftColor = "softColorSpecial";
                    iconHardColor = "hardColorSpecial";
                    
                    backSoftColor = "softBackColorSpecial";
                    backHardColor = "hardBackColorSpecial";
                    break;
            }

            if (_currencyRecolorImage != null)
            {
                switch (type)
                {
                    case CurrencyType.USD : _currencyRecolorImage.recolorFilter = iconHardColor; break;
                    case CurrencyType.Z : _currencyRecolorImage.recolorFilter = iconSoftColor; break;
                    case CurrencyType.TICKETS : _currencyRecolorImage.recolorFilter = iconSoftColor; break;
                }
                
                _currencyRecolorImage.Recolor();
            }

            if (_currencyRecolorBackImage != null)
            {
                switch (type)
                {
                    case CurrencyType.USD : _currencyRecolorBackImage.recolorFilter = backHardColor; break;
                    case CurrencyType.Z : _currencyRecolorBackImage.recolorFilter = backSoftColor; break;
                    case CurrencyType.TICKETS : _currencyRecolorBackImage.recolorFilter = backSoftColor; break;
                }
                
                _currencyRecolorBackImage.Recolor();
            }
        }
    }
}