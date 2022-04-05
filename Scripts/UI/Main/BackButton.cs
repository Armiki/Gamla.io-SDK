using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Gamla.Scripts.UI.Main
{
    public class BackButton : MonoBehaviour
    {
        Button _button;

        private void Start() {
            _button = GetComponent<Button>();
        }

#if UNITY_ANDROID
        void Update() {
            if (Input.GetKeyDown(KeyCode.Escape) && _button != null)
            {
                _button.OnPointerClick(new PointerEventData(EventSystem.current));
            }
        }
#endif
    }
}
