using UnityEngine;

namespace Gamla.UI
{
    public class MainCanvas : MonoBehaviour
    {
        public static MainCanvas Canvas;
        void Awake()
        {
            Canvas = this;
        }

    }
}