using System;
using System.Linq;
using Gamla.Scripts.Data;
using Gamla.Scripts.Logic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.Scripts.UI.Main
{
    public class TopBar : MonoBehaviour
    {
        public event Action onProfileClick;
        public event Action onAddCurrencyClick;
        
        //public event Action onRequestNewData;
        
        //[SerializeField] private UserProfileWidget _user;
        [SerializeField] private TopBarCurrency _soft;
        [SerializeField] private TopBarCurrency _hard;
        [SerializeField] private TopBarCurrency _tickets;
        
        //[SerializeField] private Button _profileBtn;
        //[SerializeField] private Button _addCurrencyBtn;

        public void Start()
        {
            //_profileBtn.onClick.RemoveAllListeners();
            //_profileBtn.onClick.AddListener(() => onProfileClick?.Invoke());
            //_addCurrencyBtn.onClick.RemoveAllListeners();
            //_addCurrencyBtn.onClick.AddListener(() => onAddCurrencyClick?.Invoke());
        }

        public void Init(UserInfo current_user)
        {
            if (current_user != null)
            {
                //_user.Init(current_user);
                _soft.Init(current_user.wallet.currencies.Find(c => c.type == CurrencyType.Z));
                _hard.Init(current_user.wallet.currencies.Find(c => c.type == CurrencyType.USD));
                _tickets.Init(current_user.wallet.currencies.Find(c => c.type == CurrencyType.TICKETS));
            }
        }
        
        public void OnDestroy()
        {
           
        }
        
        public void RefreshSafeZone(int indentSize)
        {
            var root = GetComponent<RectTransform>();
            //root.localPosition = new Vector2(0, indentSize/2.0f);
            root.anchoredPosition = new Vector2(0, indentSize);
            //root.sizeDelta = new Vector2(0, indentSize);
        }
    }
}
