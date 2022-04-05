using System;
using Gamla.Scripts.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.Scripts.UI.Store
{
    public class StoreWidget : MonoBehaviour
    {
        public event Action<Pack> onBuyClick;

        [SerializeField] private Image _icon;
        [SerializeField] private Text _count;
        [SerializeField] private Text _bonus;
        [SerializeField] private Button _button;
        [SerializeField] private GameObject _extraGO;
        [SerializeField] private GameObject _bonusGo;

        private Pack _pack;
        public void Start()
        {
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(() =>
            {
                onBuyClick?.Invoke(_pack);
            });
        }

        public void OnDestroy()
        {
            onBuyClick = null;
        }

        public void Init(Pack pack)
        {
            _pack = pack;
            _icon.sprite = GUIConstants.guiSettings.storeIcons.Find(i => i.name == pack.name).flag;
            _count.text = "$" + pack.main.amount;
            _extraGO.SetActive(_bonus != null && pack.bonus.amount > 0);
            _bonusGo.SetActive(_bonus != null && pack.bonus.amount > 0);
            if (_bonus != null)
            {
                _bonus.text = "+" + pack.bonus.amount + "$";
            }
        }
    }
}
