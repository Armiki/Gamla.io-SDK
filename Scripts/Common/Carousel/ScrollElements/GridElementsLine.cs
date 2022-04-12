using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.UI.Carousel
{
    [RequireComponent(typeof(HorizontalLayoutGroup))]
    public class GridElementsLine : BaseScrollElement
    {
        public int size => _cells.Count;
        public BaseScrollElement this[int i] => _cells[i];

        readonly List<BaseScrollElement> _cells = new List<BaseScrollElement>();
        HorizontalLayoutGroup _layout;
        int _cachedVerticalSpacing;

        public float horizontalSpacing
        {
            set
            {
                if (_layout != null){
                    _layout.spacing = value;
                }
            }
        }

        protected override void Awake()
        {
            _layout = GetComponent<HorizontalLayoutGroup>();
            base.Awake();
        }

        public float verticalSpacing
        {
            set
            {
                _cachedVerticalSpacing = (int) value;
                if (_layout != null){
                    _layout.padding.bottom = (int) value;
                }
            }
        }

        public void SetEnableDynamicHeight(bool state)
        {
        }

        public void AddCell(BaseScrollElement view)
        {
            view.transform.SetParent(this.transform, false);
            var canvasGroup = view.gameObject.GetComponent<CanvasGroup>();
            if (canvasGroup == null) view.gameObject.AddComponent<CanvasGroup>();
            view.AddSelfCanvasGroupReference(canvasGroup);
            _cells.Add(view);

            if (_cells.Count == 1) { //first adding subview
                //do not support horizontal
                var subVeiwRect = view.GetComponent<RectTransform>().sizeDelta;
                _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, subVeiwRect.y + _cachedVerticalSpacing);
            }
        }

        public void ClearCells()
        {
            _cells.Clear();
        }

        public void DisposeCells()
        {
            foreach (var subView in _cells)
            {
                subView.OnDispose();
            }
        }
    }
}