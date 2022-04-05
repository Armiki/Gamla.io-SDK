using System;
using System.Linq;
using DG.Tweening;
using Gamla.GUI;
using Gamla.Scripts.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.Scripts.UI.Main
{
    public class RecolorItem : MonoBehaviour
    {
        [SerializeField] private string recolorField;
        [SerializeField] private Image image;
        [SerializeField] private Text simpleText;
        //[SerializeField] private TextMeshProUGUI proText;
        [SerializeField] private UIGradient gradient;
        [SerializeField] private RectTransform animRect;
        [SerializeField] private float angelMultiply = 1;
        [SerializeField] private RawImage rawImage;

        public string recolorFilter
        {
            set => recolorField = value;
        }

        public void Start()
        {
            Recolor();
        }

        public void SetNewRecolor(string code)
        {
            recolorField = code;
            Recolor();
        }

        public void Recolor()
        {
            var currentTemplate =
                GUIConstants.colorTemplate.colors.FirstOrDefault(x => x.name == LocalState.selectColorTemplate);

            Type t = currentTemplate.GetType();
            var item = t.GetField(recolorField);
            object value = item.GetValue(currentTemplate);

            if (value is Color)
            {
                if (image != null)
                {
                    image.color = (Color) value;
                }
                if (rawImage != null)
                {
                    rawImage.color = (Color) value;
                }
                if (simpleText != null)
                {
                    simpleText.color = (Color) value;
                }
                /*if (proText != null)
                {
                    proText.color = (Color) value;
                }*/
            }
            if (value is Sprite)
            {
                if (image != null)
                {
                    image.sprite = (Sprite) value;
                }
            }
            if (value is Font)
            {
                if (simpleText != null)
                {
                    simpleText.font = (Font) value;
                }
            }
            /*if (value is TMP_FontAsset)
            {
                if (simpleText != null)
                {
                    proText.font = (TMP_FontAsset) value;
                }
            }*/
            if (value is RecolorGradient)
            {
                if (image != null)
                {
                    image.color = Color.white;
                    if (gradient == null)
                    {
                        gradient = gameObject.AddComponent<UIGradient>();
                    }
                    var recolorGradient = (RecolorGradient) value;
                    gradient.m_color1 = recolorGradient.color_1;
                    gradient.m_color2 = recolorGradient.color_2;
                    gradient.m_angle = recolorGradient.angel * angelMultiply;
                    if (recolorGradient.isWaveColor)
                    {
                        /*Tween tween;
                        var from = recolorGradient.fromAngel;
                        var to = recolorGradient.toAngel;
                        tween = DOTween.To(() => from, x => from = x, to, 2f)
                            .OnUpdate(() =>
                                {
                                    gradient.scale = from;
                                    Debug.LogError(gradient.scale);
                                }
                              );
                        tween.OnComplete(() => { gradient.scale = to; from = recolorGradient.fromAngel;});
                        tween.SetLoops(int.MaxValue);
                        tween.Play();*/
                        Invoke("InvokeWaveRecolor", 0.1f);
                    }
                }
            }
        }

        private void InvokeWaveRecolor()
        {
            if (animRect != null)
            {
                Sequence sequence = DOTween.Sequence();
                sequence.Append(animRect.transform.DOLocalMoveX(-1 * animRect.rect.width, 2))
                    .Append(animRect.transform.DOLocalMoveX(0, 2))
                    .SetLoops(-1);
                sequence.Play();
            }
        }
    }
}
