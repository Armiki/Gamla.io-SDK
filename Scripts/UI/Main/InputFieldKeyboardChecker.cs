using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Gamla.UI
{
    public class InputFieldKeyboardChecker : MonoBehaviour
    {
        public UnityEvent<int> onKeyboardChange;

        [SerializeField] InputField input;
        
        private int _lastKeyboardHeight;
        private bool _keyboardActive;
        
        void Update()
        {
            if (input.touchScreenKeyboard == null) {
                if (_keyboardActive) {
                    _keyboardActive = false;
                    OnKeyboardInactive();
                }
                return;
            }

            if (input.touchScreenKeyboard.active) {
                _keyboardActive = true;
                CheckKeyboardIdent();
            } 
        }

        void OnKeyboardInactive()
        {
            onKeyboardChange?.Invoke(0);
            _lastKeyboardHeight = 0;
            input.OnDeselect(new BaseEventData(EventSystem.current));
        }
        
        void CheckKeyboardIdent()
        {
            var keyboardHeight = GetKeyboardHeight();
            if (_lastKeyboardHeight != keyboardHeight) {
                Rect itemRect = RectTransformToScreenSpace(input.transform as RectTransform);
                var itemHeight = itemRect.y - itemRect.size.y /2;
                int diff = (int)(itemHeight - keyboardHeight);
                if (diff < 0) {
                    onKeyboardChange?.Invoke(-diff);
                }
                _lastKeyboardHeight = keyboardHeight;
            }
        }
        
        Rect RectTransformToScreenSpace(RectTransform rect)
        {
            var scale = rect.lossyScale;
            var size = rect.rect.size;
            var position = rect.position;
            
            Vector2 resultSize = new Vector2(size.x / scale.x, size.y / scale.y);
            var resultPosition = new Vector2(position.x / scale.x, position.y / scale.y);
            return new Rect(resultPosition, resultSize);
        }

        int GetKeyboardHeight()
        {
#if !UNITY_EDITOR && UNITY_ANDROID
            return (int)((0.4f * Screen.height) / input.transform.lossyScale.y);
#endif
            var height = TouchScreenKeyboard.area.height / input.transform.lossyScale.y;
            return (int) height;
        }
    }
}
