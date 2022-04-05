using Gamla.Scripts.Data;
using GamlaSDK.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.Scripts.UI.Profile
{

    public class GameLvlWidget : MonoBehaviour
    {
        [SerializeField] private Text _gameName;
        [SerializeField] private Text _gameLvl;
        [SerializeField] private AvatarComponent _gameLogo;
        [SerializeField] private Image _gameProgress;
        
        [SerializeField] private GameObject _isCurrentGame;
        
        public RectTransform rect;

        protected long _id;
        
        public virtual void Init(UserGames game)
        {
            _id = game.id;
            _gameName.text = string.IsNullOrEmpty(game.name) ? "Get Cube" : game.name;
            _gameLvl.text = string.Format(LocalizationManager.Text("gamla.main.lvl.title"), game.level);
            _gameProgress.fillAmount = (game.expCurLvl / game.expNextLvl);
            _gameLogo.Load(game.logo);
            gameObject.SetActive(true);
            
            if(_isCurrentGame != null)
                _isCurrentGame.SetActive(true);
        }
    }
}
