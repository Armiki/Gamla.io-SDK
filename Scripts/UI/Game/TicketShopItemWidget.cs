using System;
using Gamla.Data;
using Gamla.Logic;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.UI
{
    public class TicketShopItemWidget : MonoBehaviour
    {
        private event Action onClickItem;

        [SerializeField] private Button _buyBtn;
        [SerializeField] private Text _itemName;
        [SerializeField] private CurrencyBadge _itemPriceBadge;
        [SerializeField] private Text _itemPrice;
        [SerializeField] private AvatarComponent _itemLogo;

        public void Start()
        {
            _buyBtn.onClick.RemoveAllListeners();
            _buyBtn.onClick.AddListener(() => onClickItem?.Invoke());
        }

        public void Init(TicketShopItemModel item)
        {
            _itemName.text = item.name;
            _itemLogo.Load(item.imageUrl);
            onClickItem += () => { ServerCommand.BuyItem(item); };
            _itemPriceBadge.Init(CurrencyType.TICKETS, 1);
            _itemPrice.text = item.price + "";
        }

    }
}