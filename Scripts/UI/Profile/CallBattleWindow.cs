using System;
using Gamla.Scripts.Common.UI;
using UnityEngine;
using UnityEngine.UI;
using Gamla.Scripts.UI.Main;


namespace Gamla.Scripts.UI.Profile
{
    public class CallBattleWindow : GUIView
    {
        public event Action onFight;
        
        [SerializeField] private AvatarComponent _avatarUser;
        [SerializeField] private Text _nameUser;
        
        [SerializeField] private CurrencyBadge _winnerCurrencyBadge;
        [SerializeField] private Text _winnnerText;
        [SerializeField] private Text _entryFee;
        
        [SerializeField] private Button _fightBtn;

        public void Start()
        {
            _fightBtn.onClick.RemoveAllListeners();
            _fightBtn.onClick.AddListener(() =>
            {
                onFight?.Invoke();
                Close();
            });
        }
    }
}