using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Gamla.UI.Carousel
{
    /// <summary>
    /// стандартный ScrollRect не позволяет одновременно изменять позицию content
    /// с помощью скрипта и ивентов от юзера.
    /// Отсюда возникает проблема при поптыке изменить положение контента во время drag event от юзера.
    /// 
    /// Нужно использовать CustomSetVerticalNormalizedPosition и CustomSetHorizontalNormalizedPosition
    /// для решения этой проблемы
    /// </summary>
    public class CarouselScrollRect : ScrollRect
    {

        
        PointerEventData m_beginEventData;
        bool _dragging;
        
        public string itemName => gameObject.name;

        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);
            _dragging = false;
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            base.OnBeginDrag(eventData);
            m_beginEventData = eventData;
            _dragging = true;
        }

        public void CustomSetVerticalNormalizedPosition(float newVerticalNormalizedPosition)
        {
            var velocityBeforeChangingPosition = new Vector2(velocity.x, velocity.y);
            verticalNormalizedPosition = newVerticalNormalizedPosition;
            HandleDraggingAndVelocity(velocityBeforeChangingPosition);
        }

        public void CustomSetHorizontalNormalizedPosition(float newHorizontalNormalizedPosition)
        {
            var velocityBeforeChangingPosition = new Vector2(velocity.x, velocity.y);
            horizontalNormalizedPosition = newHorizontalNormalizedPosition;
            HandleDraggingAndVelocity(velocityBeforeChangingPosition);
        }

        void HandleDraggingAndVelocity(Vector2 velocityBefore)
        {
            if (_dragging)
            {
                //keep dragging but from new position
                OnBeginDrag(m_beginEventData);
            }

            //set original velocity
            velocity = velocityBefore;
        }
    }
}