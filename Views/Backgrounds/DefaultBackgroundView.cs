namespace UniGame.Views.Backgrounds
{
    using System.Collections;
    using UniGame.Rendering.Runtime.Blur.KawaseBlur;
    using UniCore.Runtime.ProfilerTools;
    using UiSystem.Runtime;
    using UniGame.UiSystem.Runtime.Backgrounds.Abstract;
    using Core.Runtime;
    using UnityEngine;

#if ENABLE_DOTWEEN
    using DG.Tweening;
    using UniModules.UniGame.DoTweenRoutines.Runtime;
#endif
    
    public class DefaultBackgroundView : UiCanvasGroupView<IBackgroundViewModel>, IBackgroundView
    {
        [SerializeField, Range(0.0f, 1.0f)]
        public float _duration = 0.3f;

        public bool enableBlur = true;
        
#if ENABLE_DOTWEEN
        private Sequence _hideSequence;
        private Sequence _showSequence;
#endif
        
        protected override IEnumerator OnCloseProgressOverride(ILifeTime progressLifeTime)
        {
#if ENABLE_DOTWEEN
            DoTweenExtension.KillSequence(ref _showSequence);
            DoTweenExtension.KillSequence(ref _hideSequence);
            
            _hideSequence = GetHideSequence();
            yield return _hideSequence.WaitForCompletionTween();
#endif
            yield break;
        }

        protected override IEnumerator OnHidingProgressOverride(ILifeTime progressLifeTime)
        {
#if ENABLE_DOTWEEN
            DoTweenExtension.KillSequence(ref _showSequence);
            DoTweenExtension.KillSequence(ref _hideSequence);
            
            _hideSequence = GetHideSequence();
            
            yield return _hideSequence.WaitForCompletionTween();
#endif
            yield break;
        }

        protected override IEnumerator OnShowProgressOverride(ILifeTime progressLifeTime)
        {
#if ENABLE_DOTWEEN
            DoTweenExtension.KillSequence(ref _showSequence);
            DoTweenExtension.KillSequence(ref _hideSequence);
            
            _showSequence = GetShowSequence();
            yield return _showSequence.WaitForCompletionTween();
#endif
            yield break;
        }

#if ENABLE_DOTWEEN
        private Sequence GetShowSequence()
        {
            GameLog.LogRuntime("SHOW BACKGROUND AND ENABLE BLUR");
            
            if(enableBlur)
                KawaseBlurGlobalSettings.EnableBlur();

            var sequence  = DOTween.Sequence();
            var fadeTween = canvasGroup.DOFade(1.0f, _duration);

            sequence.Join(fadeTween);
            
            return sequence;
        }

        private Sequence GetHideSequence()
        {
            var sequence  = DOTween.Sequence();
            var fadeTween = canvasGroup.DOFade(0.0f, _duration);

            sequence.Join(fadeTween)
                .OnComplete(() =>
                {
                    GameLog.LogRuntime("HIDE BACKGROUND AND DISABLE BLUR");
                    KawaseBlurGlobalSettings.DisableBlur();
                });
            
            return sequence;
        }
#endif
    }
}