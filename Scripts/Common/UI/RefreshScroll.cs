using System;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.UI
{
    [RequireComponent( typeof(ScrollRect))]
    public class RefreshScroll : MonoBehaviour
    {
        public event Action onRefresh;
        public bool isRefreshing { get; private set; }
        
        ScrollRect _view;
        float _cooldown;

        float _normalizedPosition;

        void Start()
        {
            _view = GetComponent<ScrollRect>();
        }

        public void Update()
        {
            _cooldown -= Time.deltaTime;
            if (_cooldown > 0) {
                return;
            }
            
            if (IsNearBorder())
            {
                if (!isRefreshing) {
                    isRefreshing = true;
                    onRefresh?.Invoke();
                } 
            } else {
                if (isRefreshing) {
                    isRefreshing = false;
                    _cooldown = 1;
                }
            }
        }

        bool IsNearBorder()
        {
            var near = _view.normalizedPosition.y < -0.07 || _view.normalizedPosition.y > 1.07;
            return near;
        }
    }
}
