using DG.Tweening;
using UnityEngine;

namespace Gamla.UI
{ 
    public enum AnimationPatternType
    {
        None,
        WinShiftBottom,
        WinShiftLeft
    }
    public static class AnimationPattern
    {
        public static Sequence WindowShiftFromBottom(Transform animContent)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(animContent.DOLocalMoveY(-2500, 0));
            sequence.Append(animContent.DOLocalMoveY(40, 0.5f));
            sequence.Append(animContent.DOLocalMoveY(0, 0.2f));
            sequence.Play();
            return sequence;
        }
        
        public static Sequence WindowShiftFromLeft(Transform animContent)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(animContent.DOLocalMoveX(-2500, 0));
            sequence.Append(animContent.DOLocalMoveX(0, 0.5f));
            sequence.Play();
            return sequence;
        }
    }
}