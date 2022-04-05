using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace GamlaSDK.Scripts.UI.Main
{
    public class Toggle : MonoBehaviour
    {
        public event Action<bool> changte;
        [SerializeField] private Button _btn;

        public void Start()
        {
            /*_btn.onClick.RemoveAllListeners();
            _btn.onClick.AddListener(() =>
            {
                toBattle?.Invoke(_id);
                Close();
            });*/
        }
    }
}