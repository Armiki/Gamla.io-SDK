using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gamla.UI
{
    public class StarAnim : MonoBehaviour
    {
        private bool _inAnim = false;
        public void LateUpdate()
        {
            if(_inAnim || transform == null) return;
            _inAnim = true;
            float randScale = (Random.Range(0.1f, 1f));
            var randTime = Random.Range(0.5f, 1f);
            transform.DOPunchScale(new Vector3(randScale,randScale,randScale),randTime, 1, 5).OnComplete(() =>
            {
                _inAnim = false;
            });
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