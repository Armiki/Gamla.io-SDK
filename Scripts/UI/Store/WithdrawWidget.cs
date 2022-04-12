using System;
using Gamla.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.UI
{
    public class WithdrawWidget : MonoBehaviour
    {
        [SerializeField] private Text _title;
        [SerializeField] private Text _desc;
        [SerializeField] private Toggle _toggle;
        private CardHolderModel _model;

        public void Init(CardHolderModel model, Action<CardHolderModel> callback)
        {
            _model = model;
            _title.text = _model.platform;
            _desc.text = _model.data;
            _toggle.onValueChanged.RemoveAllListeners();
            _toggle.onValueChanged.AddListener((arg0 =>
            {
                if (arg0)
                {
                    Debug.Log("WithdrawWidget " + _model.platform);
                    callback?.Invoke(_model);
                }
            }));
        }

        public void UpdateView(CardHolderModel model)
        {
            if (_model.data != model.data)
                _toggle.isOn = false;
        }
    }
}