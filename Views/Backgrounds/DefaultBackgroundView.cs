namespace UniGame.Views.Backgrounds
{
    using UniCore.Runtime.ProfilerTools;
    using UiSystem.Runtime;
    using UniGame.UiSystem.Runtime.Backgrounds.Abstract;
    using Core.Runtime;
    using Cysharp.Threading.Tasks;
    using UnityEngine;

#if ENABLE_DOTWEEN
    using DoTween.Runtime.Extensions;
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
        
        protected override async UniTask OnCloseProgress(ILifeTime progressLifeTime)
        {
#if ENABLE_DOTWEEN
            DoTweenExtension.KillSequence(ref _showSequence);
            DoTweenExtension.KillSequence(ref _hideSequence);
            
            _hideSequence = GetHideSequence();
             await _hideSequence.WaitForCompletionTweenAsync();
#endif
            return;
        }

        protected override async UniTask OnHidingProgress(ILifeTime progressLifeTime)
        {
#if ENABLE_DOTWEEN
            DoTweenExtension.KillSequence(ref _showSequence);
            DoTweenExtension.KillSequence(ref _hideSequence);
            
            _hideSequence = GetHideSequence();
            
            await _hideSequence.WaitForCompletionTweenAsync();
#endif
        }

        protected override async UniTask OnShowProgress(ILifeTime progressLifeTime)
        {
#if ENABLE_DOTWEEN
            DoTweenExtension.KillSequence(ref _showSequence);
            DoTweenExtension.KillSequence(ref _hideSequence);
            
            _showSequence = GetShowSequence();
            await _showSequence.WaitForCompletionTweenAsync();
#endif
        }

#if ENABLE_DOTWEEN
        private Sequence GetShowSequence()
        {
            GameLog.LogRuntime("SHOW BACKGROUND AND ENABLE BLUR");

            if (enableBlur)
            {
                
            }

            var sequence  = DOTween.Sequence();
            var fadeTween = CanvasGroup.DOFade(1.0f, _duration);

            sequence.Join(fadeTween);
            
            return sequence;
        }

        private Sequence GetHideSequence()
        {
            var sequence  = DOTween.Sequence();
            var fadeTween = CanvasGroup.DOFade(0.0f, _duration);

            sequence.Join(fadeTween)
                .OnComplete(() =>
                {
                    GameLog.LogRuntime("HIDE BACKGROUND AND DISABLE BLUR");
                });
            
            return sequence;
        }
#endif
    }
}