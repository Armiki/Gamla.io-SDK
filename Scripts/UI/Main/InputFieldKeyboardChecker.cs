using Gamla.Scripts.Logic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Gamla.Scripts.UI.Main
{
    public class InputFieldKeyboardChecker : MonoBehaviour
    {
        public InputField input;
        public UnityEvent<int> onKeyboardChange;

        [SerializeField] bool resetToSafeArea = true;
        
        private int _lastKeyboardHeight;
        private TouchScreenKeyboard.Status _keyboardStatus;

        public void Start()
        {
            _keyboardStatus = TouchScreenKeyboard.Status.Canceled;
        }
        
        void Update()
        {
            if (input.touchScreenKeyboard == null) {
                return;
            }

            if (input.touchScreenKeyboard.status == TouchScreenKeyboard.Status.Visible) {
                CheckScreenOffset();
            } else if (input.touchScreenKeyboard.status != _keyboardStatus) {
                if (resetToSafeArea) {
                    onKeyboardChange?.Invoke(GamlaResourceManager.safeAreaManager.Indent);
                } else {
                    onKeyboardChange?.Invoke(0);
                }

                _lastKeyboardHeight = 0;
                input.OnDeselect(new BaseEventData(EventSystem.current));
            }

            if(input.touchScreenKeyboard != null){
                _keyboardStatus = input.touchScreenKeyboard.status;
            }
        }
        
        public Bounds GetRectTransformBounds(RectTransform transform)
        {
            Vector3[] corners = new Vector3[4];
            transform.GetWorldCorners(corners);
            Bounds bounds = new Bounds(corners[0], Vector3.zero);
            for(int i = 1; i < 4; ++i) {
                bounds.Encapsulate(corners[i]);
            }
            return bounds;
        }

        void CheckScreenOffset()
        {
            var keyboardHeight = GetKeyboardHeight();
            if (_lastKeyboardHeight != keyboardHeight) {
                Bounds bounds = GetRectTransformBounds(input.transform as RectTransform);
                var itemHeight = bounds.min.y;
                int diff = (int)(itemHeight - keyboardHeight);
                if (diff < 0) {
                    onKeyboardChange?.Invoke(-diff);
                }
                _lastKeyboardHeight = keyboardHeight;
            }
        }

        int GetKeyboardHeight()
        {
#if !UNITY_EDITOR && UNITY_ANDROID
            return (int)(0.4f * Screen.height);
#endif
            return (int)TouchScreenKeyboard.area.height;
        }
    }
}
