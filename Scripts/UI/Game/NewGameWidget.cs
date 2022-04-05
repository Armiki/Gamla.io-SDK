using System;
using Gamla.Scripts.Data;
using UnityEngine;
using UnityEngine.UI;
using Gamla.Scripts.UI.Main;

namespace GamlaSDK.Scripts.UI.Game
{
    public class NewGameWidget : MonoBehaviour
    {
        public event Action onPlayGame;
        
        [SerializeField] private GameObject _difficulty;
        [SerializeField] private GameObject _players;
        
        [SerializeField] private Text _entryTxt;
        [SerializeField] private Image _entryLogo;
        
        [SerializeField] private Text _winTxt;
        [SerializeField] private Image _winLogo;
        [SerializeField] private RecolorItem _winLogoColor;
        [SerializeField] private RecolorItem _winLogoBack;
        
        [SerializeField] private Text _trophieTxt;
        
        [SerializeField] private Text _playersTxt;
        [SerializeField] private Button _playGame;

        public RectTransform rect;
        
        public void Init(BattleInfo data)
        {
            _playGame.onClick.RemoveAllListeners();
            _playGame.onClick.AddListener(() => { onPlayGame?.Invoke(); });
            
            _players.SetActive(data.type == BattleType.Tournament);
            _difficulty.SetActive(data.type != BattleType.Tournament);

            _trophieTxt.text = 2 + ""; //data.trophie.ToString();

            _winTxt.text = data.win.amount.ToString();
            _winLogo.sprite = data.win.type == CurrencyType.Z
                ? GUIConstants.guiSettings.soft_icon
                : GUIConstants.guiSettings.hard_icon;
            
            _entryTxt.text = data.entry.amount.ToString();
            _entryLogo.sprite = data.entry.type == CurrencyType.Z
                ? GUIConstants.guiSettings.soft_icon
                : GUIConstants.guiSettings.hard_icon;

            _playersTxt.text = data.max_gamers.ToString();
            
            _winLogoColor.recolorFilter = data.win.type == CurrencyType.Z
                ? "softColorSecondary"
                : "hardColorSecondary";
            _winLogoBack.recolorFilter = data.win.type == CurrencyType.Z
                ? "softBackColorSecondary"
                : "hardBackColorSecondary";
            _winLogoBack.Recolor();
        }
    }
}