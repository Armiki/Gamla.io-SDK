using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamla.Scripts.UI.Main
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