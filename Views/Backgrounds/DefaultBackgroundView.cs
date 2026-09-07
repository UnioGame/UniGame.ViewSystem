namespace UniGame.Views.Backgrounds
{
    using UiSystem.Runtime;
    using UniGame.UiSystem.Runtime.Backgrounds.Abstract;
    using Core.Runtime;
    using Cysharp.Threading.Tasks;
    using UnityEngine;

#if ENABLE_DOTWEEN
    using DG.Tweening;
#endif
    
    public class DefaultBackgroundView : View<IBackgroundViewModel>, IBackgroundView
    {
        [SerializeField, Range(0.0f, 1.0f)]
        public float _duration = 0.3f;

        public bool enableBlur = true;
        
#if ENABLE_DOTWEEN
        private Sequence _hideSequence;
        private Sequence _showSequence;
#endif

        protected override async UniTask OnHideProgress(ILifeTime progressLifeTime)
        {
#if ENABLE_DOTWEEN
            _showSequence?.Kill();
            _showSequence = null;
            _hideSequence?.Kill();
            _hideSequence = null;
            
            _hideSequence = GetHideSequence();
            
            await _hideSequence.AsyncWaitForCompletion();
#endif
        }

        protected override async UniTask OnShowProgress(ILifeTime progressLifeTime)
        {
#if ENABLE_DOTWEEN
            _showSequence?.Kill();
            _showSequence = null;
            _hideSequence?.Kill();
            _hideSequence = null;
            
            _showSequence = GetShowSequence();
            await _showSequence.AsyncWaitForCompletion();
#endif
        }

#if ENABLE_DOTWEEN
        private Sequence GetShowSequence()
        {
            var sequence  = DOTween.Sequence();
            var fadeTween = CanvasGroup.DOFade(1.0f, _duration);

            sequence.Join(fadeTween);
            
            return sequence;
        }

        private Sequence GetHideSequence()
        {
            var sequence  = DOTween.Sequence();
            var fadeTween = CanvasGroup.DOFade(0.0f, _duration);

            sequence.Join(fadeTween);
            
            return sequence;
        }
#endif
    }
}