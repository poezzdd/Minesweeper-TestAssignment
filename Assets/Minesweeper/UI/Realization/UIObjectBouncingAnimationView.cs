using DG.Tweening;
using UnityEngine;

namespace Minesweeper.UI.Realization
{
    public class UIObjectBouncingAnimationView : MonoBehaviour
    {
        private Tween _tween;

        private Vector3 _startScale;
        private Vector3 _endScale;
        
        private const float AnimationDuration = 0.2f;
        private const float AnimationModifier = 1.2f;

        private void Awake()
        {
            _startScale = transform.localScale;
            _endScale = _startScale * AnimationModifier;
        }

        private void OnEnable()
        {
            _tween?.Kill();
            
            _tween = transform
                .DOScale(_endScale, AnimationDuration)
                .SetEase(Ease.Linear)
                .SetLoops(1, LoopType.Yoyo)
                .OnComplete(() =>
                {
                    transform.localScale = _startScale;
                });
        }

        private void OnDisable()
        {
            _tween?.Kill();
            transform.localScale = _startScale;
        }
    }
}