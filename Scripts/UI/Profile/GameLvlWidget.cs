using Gamla.Data;
using Gamla.Logic;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.UI
{

    public class GameLvlWidget : MonoBehaviour
    {
        [SerializeField] private Text _gameName;
        [SerializeField] private Text _gameLvl;
        [SerializeField] private AvatarComponent _gameLogo;
        [SerializeField] private Image _gameProgress;
        [SerializeField] private Text _progressText;
        
        [SerializeField] private GameObject _isCurrentGame;
        
        public RectTransform rect;

        protected long _id;
        
        public virtual void Init(UserGames game)
        {
            _id = game.id;
            _gameName.text = string.IsNullOrEmpty(game.name) ? $"Game ({game.id})" : game.name;
            var nextLevel = game.expCurLvl + game.expNextLvl;
            var progress = game.expCurLvl / (float)nextLevel;
            _progressText.text = $"{game.expCurLvl}/{nextLevel}";
            _gameProgress.fillAmount = progress;
            _gameLvl.text = string.Format(LocalizationManager.Text("gamla.main.lvl.title"), game.level);
            _gameLogo.Load(game.logo);
            gameObject.SetActive(true);
            
            if(_isCurrentGame != null)
                _isCurrentGame.SetActive(true);

            if (string.IsNullOrEmpty(game.name))
            {
                ServerCommand.GetGameInfo(game.id + "", info =>
                {
                    if (info != null)
                    {
                        game.name = info.name;
                        if (_gameName != null)
                        {
                            _gameName.text = game.name;
                        }
                    }
                });
            }
        }
    }
}
