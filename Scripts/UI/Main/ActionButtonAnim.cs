using DG.Tweening;
using UnityEngine;

namespace Gamla.UI
{
    public class ActionButtonAnim : MonoBehaviour
    {
        [SerializeField] private RectTransform star1;
        [SerializeField] private RectTransform star2;
        [SerializeField] private RectTransform star3;
        [SerializeField] private RectTransform star4;
        [SerializeField] private RectTransform star5;
        [SerializeField] private RectTransform star6;
        [SerializeField] private RectTransform star7;
        [SerializeField] private RectTransform star8;
        [SerializeField] private RectTransform star9;

        private Sequence sequence1;
        private Sequence sequence2;
        
        public void Start()
        {
            sequence1 = DOTween.Sequence();
            sequence1.Append(star1.DOPunchScale(new Vector3(0.3f, 0.3f, 0.3f), 1, 1));
            sequence1.SetLoops(-1);
            sequence1.Play();
            
            sequence2 = DOTween.Sequence();
            sequence2.AppendInterval(0.3f);
            sequence2.Append(star2.DOPunchScale(new Vector3(0.6f, 0.6f, 0.6f), 0.7f, 7));
            sequence2.SetLoops(-1);
            sequence2.Play();
        }
        
        public void OnDisable()
        {
            DOTween.CompleteAll();
        }
        public void OnDestroy()
        {
            DOTween.CompleteAll();
        }
    }
}