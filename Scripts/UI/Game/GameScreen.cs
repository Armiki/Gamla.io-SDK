using System;
using System.Collections;
using Gamla.Scripts.Common.UI;
using Gamla.Scripts.Data;
using UnityEngine;

namespace Gamla.Scripts.UI.Game
{
    public class GameScreen : GUIView
    {
        public event Action<BattleInfo> onGameFinish;

        private BattleInfo _battleInfo;
        public void Start()
        {
            StartCoroutine(GameContinue());
        }

        public void Init(BattleInfo info)
        {
            _battleInfo = info;
        }
        
        IEnumerator GameContinue()
        {
            yield return new WaitForSecondsRealtime(2f); 
            onGameFinish?.Invoke(_battleInfo);
        }
    }
}