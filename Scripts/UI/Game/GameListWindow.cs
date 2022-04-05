using System.Collections.Generic;
using Gamla.Scripts.Common.UI;
using Gamla.Scripts.Data;
using GamlaSDK;
using UnityEngine;

namespace Gamla.Scripts.UI.Game
{
    public class GameListWindow : GUIView
    {
        [SerializeField] private GameDataWidget _prefab;
        [SerializeField] private Transform _content;

        public void Start()
        {
            _content.ClearChilds();
            if (LocalState.gameApplist.featured_games != null && LocalState.gameApplist.featured_games.data != null)
            {
                ShowContent(LocalState.gameApplist.featured_games.data);
            }
        }

        private void ShowContent(List<GameAppInfo> gameList)
        {
            foreach (var gameApp in gameList)
            {
                var widget = Instantiate(_prefab, _content);
                widget.Init(gameApp);
            }
        }
    }
}