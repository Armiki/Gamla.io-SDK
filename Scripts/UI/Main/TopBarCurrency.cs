using Gamla.Scripts.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.Scripts.UI.Main
{
    public class TopBarCurrency : MonoBehaviour
    {
        //[SerializeField] private Image _currencyIcon;
        //[SerializeField] private Image _currencyBack;
        [SerializeField] private Text _currencyCount;

        public void Init(Currency currency)
        {
            if (currency != null)
            {
                _currencyCount.text = currency.amount.ToString();
            }
        }
    }
}
