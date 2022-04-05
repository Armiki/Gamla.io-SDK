using UnityEngine;
using UnityEngine.UI;

namespace Gamla.GUI
{
    [AddComponentMenu("UI/Effects/Gradient")]
    public class UIGradient : BaseMeshEffect
    {
        public Color m_color1 = Color.white;
        public Color m_color2 = Color.white;
        [Range(-180f, 180f)] public float m_angle = 0f;
        public bool m_ignoreRatio = true;
        public float scale = 0.5f;
        
        private Color _color1 = Color.white;
        private Color _color2 = Color.white;

        public override void ModifyMesh(VertexHelper vh)
        {
            if (enabled)
            {
                _color1 = Color.Lerp(m_color1, m_color2, scale - 0.5f);
                _color2 = Color.Lerp(m_color2, m_color1, scale - 0.5f);
                
                Rect rect = graphic.rectTransform.rect;
                Vector2 dir = UIGradientUtils.RotationDir(m_angle);

                if (!m_ignoreRatio)
                    dir = UIGradientUtils.CompensateAspectRatio(rect, dir);

                UIGradientUtils.Matrix2x3 localPositionMatrix = UIGradientUtils.LocalPositionMatrix(rect, dir);

                UIVertex vertex = default(UIVertex);
                for (int i = 0; i < vh.currentVertCount; i++)
                {
                    vh.PopulateUIVertex(ref vertex, i);
                    Vector2 localPosition = localPositionMatrix * vertex.position;
                    vertex.color *= Color.Lerp(_color2, _color1, localPosition.y);
                    vh.SetUIVertex(vertex, i);
                }
            }
        }
    }
}