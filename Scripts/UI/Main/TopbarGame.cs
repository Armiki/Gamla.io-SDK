using System.Collections;
using Gamla.Scripts.Data;
using Gamla.Scripts.Logic;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.Scripts.UI.Main
{
    public class TopbarGame : MonoBehaviour
    {
        [SerializeField] private Image _backTopImg;
        [SerializeField] private Text _gameTitle;

        public void Init(GameInfo info)
        {
            
        }

        public IEnumerator Start()
        {
            if (LocalState.gameAppInfo == null)
                yield return new WaitForSeconds(2);

            if (LocalState.gameAppInfo != null)
                _gameTitle.text = LocalState.gameAppInfo.name;
            
            EventManager.OnGameInfoUpdate.Subscribe(() =>
            {
                _gameTitle.text = LocalState.gameAppInfo.name;
            });
            yield return null;
        }
    }
}