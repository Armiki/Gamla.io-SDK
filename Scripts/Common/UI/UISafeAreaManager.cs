using System;
using Gamla.Scripts.Logic;
using UnityEngine;

namespace Gamla.Scripts.Common.UI
{
    public class UISafeAreaManager : MonoBehaviour
    {
        public int Indent { get; private set; }
        public event Action<int> IndentChange;
        int _lastHeight, _lastSafeZone;

        public void LateUpdate()
        {
            if (_lastHeight == Screen.height && _lastSafeZone == (int)Screen.safeArea.height) {
                return;
            }
            int cutouts = 0;
            foreach (var cutout in Screen.cutouts) {
                if (Math.Abs(cutout.position.x) < float.Epsilon) {
                    continue;
                }

                if (Math.Abs(cutout.position.x + cutout.size.x - Screen.width) < float.Epsilon) {
                    continue;
                }

                cutouts = -(int) (Screen.height - cutout.min.y);
                break;
            }

            if (cutouts != 0) {
                Indent = cutouts;
                ChangeIndent();
                return;
            }
            
            var topPosY = Screen.safeArea.max.y;
            
            int safeZone = -(int) ((Screen.height - topPosY)/GamlaResourceManager.canvas.scaleFactor);
            Indent = Math.Min(safeZone, cutouts);
            ChangeIndent();
            Debug.Log(
                $"UISafeAreaManager init. Indent = {Indent}" +
                $" Screen.height {Screen.height} " +
                $"Screen.safe {Screen.safeArea.height} " +
                $"cutouts {Screen.cutouts.Length} " +
                $"GUICanvasGlobalContainer.canvas.scaleFactor.ToString() {GamlaResourceManager.canvas.scaleFactor.ToString()}");
        }

        void ChangeIndent()
        {
            if (Indent > 0) {
                return;
            }
            
            IndentChange?.Invoke(Indent);
            _lastHeight = Screen.height;
            _lastSafeZone = (int)Screen.safeArea.height;
        }
    }
}