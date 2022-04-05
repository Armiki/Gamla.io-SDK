using Gamla.Scripts.Common.UI;
using Gamla.Scripts.Data;
using Gamla.Scripts.UI.Main;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.Scripts.UI.Profile
{
    public class TrophieWidgetExtra : TrophieWidget
    {
        [SerializeField] private Text _name;
        [SerializeField] private Text _progressTxt;
        [SerializeField] private Image _progressLine;
        [SerializeField] private RectTransform _rect;

        public RectTransform rect => _rect;

        public override void Init(ServerTrophiesModel trophie)
        {
            _name.text = trophie.name;
            if (trophie.pivot != null)
            {
                _progressTxt.text = "Done: " + trophie.pivot.count_actions + "/" + trophie.count_actions;
                _progressLine.fillAmount = (float) trophie.pivot.count_actions / trophie.count_actions;
            }  
            base.Init(trophie);
        }
    }
}