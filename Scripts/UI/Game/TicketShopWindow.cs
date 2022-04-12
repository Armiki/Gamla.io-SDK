using System.Collections.Generic;
using Gamla.Data;
using UnityEngine;

namespace Gamla.UI
{
    public class TicketShopWindow : GUIView
    {
        [SerializeField] private TicketShopItemWidget _prefab;
        [SerializeField] private Transform _content;

        public void Start()
        {
            _content.ClearChilds();
            if (LocalState.ticketShop != null)
            {
                ShowContent(LocalState.ticketShop);
            }
        }

        private void ShowContent(List<TicketShopItemModel> shop)
        {
            foreach (var item in shop)
            {
                var widget = Instantiate(_prefab, _content);
                widget.Init(item);
            }
        }
    }
}