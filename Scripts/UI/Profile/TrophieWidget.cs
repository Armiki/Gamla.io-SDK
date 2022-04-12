using Gamla.Data;
using Gamla.Logic;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.UI
{
    public class TrophieWidget: MonoBehaviour
    {
        [SerializeField] private RectTransform _progress;
        [SerializeField] private Image _logo;
        [SerializeField] private Image _progressCircle;
        [SerializeField] private RecolorItem _recolorLogo;

        public virtual void Init(ServerTrophiesModel trophie)
        {
            if (_progressCircle != null)
            {
                if (trophie.pivot != null)
                {
                    _progressCircle.fillAmount = (float) trophie.pivot.count_actions / trophie.count_actions;
                }
                else
                    _progressCircle.fillAmount = 0;
            }

            _logo.sprite = GamlaResourceManager.GetTrophyIcon(trophie.id);
            _recolorLogo.recolorFilter = trophie.pivot.count_actions == trophie.count_actions ? "specialColorPrize" : "noActiveColorPrize";
            _recolorLogo.Recolor();
        }

        public void Clear()
        {
            _logo.sprite = GamlaResourceManager.GetTrophyIcon(-1);
            _recolorLogo.recolorFilter = "noActiveColorPrize";
            _recolorLogo.Recolor();
        }
    }
}