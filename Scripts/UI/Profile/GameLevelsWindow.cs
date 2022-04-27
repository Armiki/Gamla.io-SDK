using System;
using System.Collections.Generic;
using Gamla.Data;
using UnityEngine;

namespace Gamla.UI
{
    public class GameLevelsWindow : GUIView
    {
        public event Action<long> onPlayGame;
        
        [SerializeField] private RectTransform _gamesContent;
        [SerializeField] private GameLvlWidgetExtra _gameLvlWidgetExtra;
        
        private List<GameLvlWidgetExtra> _gameWidgets = new List<GameLvlWidgetExtra>();
        
        public void InitGameProgress(List<UserGames> games)
        {
            if (games == null) {
                Debug.LogError("GameLevelsWindow.InitGameProgress: Empty games list");
                return;
            }
            
            foreach (var existGame in _gameWidgets)
            {
                Destroy(existGame.gameObject);
            }
            _gameWidgets.Clear();

            _gamesContent.sizeDelta = new Vector2(_gamesContent.sizeDelta.x, (games.Count * _gameLvlWidgetExtra.rect.sizeDelta.y + games.Count * 30));
            foreach (var data in games)
            {
                if (data == null) {
                    Debug.LogError("GameLevelsWindow.InitGameProgress: game item data is null");
                    continue;
                }
                
                var item = Instantiate(_gameLvlWidgetExtra, _gamesContent);
                item.Init(data);
                item.onPlayGame += onPlayGame;
                _gameWidgets.Add(item);
            }
        }
    }
}