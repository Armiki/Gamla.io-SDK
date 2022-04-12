using Gamla.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.UI
{
    public class AddWithdrawWindow : GUIView
    {
        [SerializeField] private ValidateInputWidget _cardNum;
        [SerializeField] private ValidateInputWidget _expireDate;
        [SerializeField] private ValidateInputWidget _cvc;
        [SerializeField] private ValidateInputWidget _cardHolder;
        
        [SerializeField] private ValidateInputWidget _payPalEmail;
        
        [SerializeField] private RectTransform _bankCardRect;
        [SerializeField] private RectTransform _payPalRect;
        
        [SerializeField] private Button _bankCardBtn;
        [SerializeField] private Button _payPalBtn;
        [SerializeField] private Button _saveBtn;

        private bool isCard = true;
        private CardHolderList _localData;

        public void Start()
        {
            _localData = JsonUtility.FromJson<CardHolderList>(
                PlayerPrefs.GetString(LocalState.currentUser.innerUserInfo.email + "_cards", ""));
            
            _bankCardBtn.onClick.RemoveAllListeners();
            _bankCardBtn.onClick.AddListener(SetBankCardView);
            
            _payPalBtn.onClick.RemoveAllListeners();
            _payPalBtn.onClick.AddListener(SetPayPalView);
            
            _saveBtn.onClick.RemoveAllListeners();
            _saveBtn.onClick.AddListener(() =>
            {
                CardHolderModel model = new CardHolderModel()
                {
                    platform = isCard ? "Card" : "PayPal",
                    data = isCard ? _cardNum.text : _payPalEmail.text,
                    holder = isCard ? _cardHolder.text : ""
                };
                if (_localData == null)
                    _localData = new CardHolderList();
                _localData.list.Add(model);

                string saveData = JsonUtility.ToJson(_localData);
                Debug.Log(saveData);
                PlayerPrefs.SetString(LocalState.currentUser.innerUserInfo.email + "_cards", saveData);
                PlayerPrefs.Save();
                Close();
            });
            
            _cardNum.keyboardChecker.onKeyboardChange.RemoveAllListeners();
            _cardNum.keyboardChecker.onKeyboardChange.AddListener(RefreshKeyboardSafeZone);

            _expireDate.keyboardChecker.onKeyboardChange.RemoveAllListeners();
            _expireDate.keyboardChecker.onKeyboardChange.AddListener(RefreshKeyboardSafeZone);

            _cvc.keyboardChecker.onKeyboardChange.RemoveAllListeners();
            _cvc.keyboardChecker.onKeyboardChange.AddListener(RefreshKeyboardSafeZone);

            _cardHolder.keyboardChecker.onKeyboardChange.RemoveAllListeners();
            _cardHolder.keyboardChecker.onKeyboardChange.AddListener(RefreshKeyboardSafeZone);

            _payPalEmail.keyboardChecker.onKeyboardChange.RemoveAllListeners();
            _payPalEmail.keyboardChecker.onKeyboardChange.AddListener(RefreshKeyboardSafeZone);
        }

        private void SetBankCardView()
        {
            _bankCardBtn.GetComponent<RecolorItem>().SetNewRecolor("filterOnColor");
            _payPalBtn.GetComponent<RecolorItem>().SetNewRecolor("filterOffColor");
            _bankCardRect.gameObject.SetActive(true);
            _payPalRect.gameObject.SetActive(false);
            isCard = true;
        }
        
        private void SetPayPalView()
        {
            _bankCardBtn.GetComponent<RecolorItem>().SetNewRecolor("filterOffColor");
            _payPalBtn.GetComponent<RecolorItem>().SetNewRecolor("filterOnColor");
            _bankCardRect.gameObject.SetActive(false);
            _payPalRect.gameObject.SetActive(true);
            isCard = false;
        }
        
        void RefreshKeyboardSafeZone(int indentSize)
        {
            var root = GetComponent<RectTransform>();
            root.anchoredPosition = new Vector2(0, indentSize);
        }
    }
}