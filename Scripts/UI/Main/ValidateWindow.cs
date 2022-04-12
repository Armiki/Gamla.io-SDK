
using System;
using UnityEngine;
using UnityEngine.UI;


namespace Gamla.UI
{
    public class ValidateWindow : GUIView
    {
        public event Action onActionClick;
        public event Action onCloseAction;
        [SerializeField] private Button _actionBtn;

        public void Start()
        {
            _actionBtn.onClick.RemoveAllListeners();
            _actionBtn.onClick.AddListener(() =>
            {
                onActionClick?.Invoke();
                Close();
            });
        }

        public void CloseInvoke()
        {
            if(transform == null || !gameObject.activeSelf) return;
            onActionClick?.Invoke();
            Close();
        }

        public override void OnDestroy()
        {
            onCloseAction?.Invoke();
            onActionClick = null;
            onCloseAction = null;
            base.OnDestroy();
            //GamlaResourceManager.tabBar.SelectPlay();
        }
    }
    
    
}