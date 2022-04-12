using System;
using Gamla.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.UI
{
    public class GameLvlWidgetExtra : GameLvlWidget
    {
        public event Action<long> onPlayGame;
        
        [SerializeField] private Button _play;
        
        public void Start()
        {
            _play.onClick.RemoveAllListeners();
            _play.onClick.AddListener(()=> onPlayGame?.Invoke(_id));
        }

        public override void Init(UserGames game)
        {
            base.Init(game);
            _play.gameObject.SetActive(_id > 0);
        }
    }
}