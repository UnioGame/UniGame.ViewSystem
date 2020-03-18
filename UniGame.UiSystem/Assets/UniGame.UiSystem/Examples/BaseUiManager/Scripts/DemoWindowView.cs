namespace UniGame.UiSystem.Examples.BaseUiManager
{
    using System.Collections;
    using DG.Tweening;
    using Runtime;
    using Runtime.Abstracts;
    using UniGreenModules.UniCore.Runtime.DataFlow.Interfaces;
    using UnityEngine;

    public class DemoWindowView : WindowView<IViewModel>
    {
        public float showTime = 3f;
        public float hideTime = 3f;

        private Tween animationTween;

        protected override void OnViewInitialize(IViewModel view)
        {
            LifeTime.AddCleanUpAction(() => animationTween?.Complete());
        }
        
        protected override IEnumerator OnShowProgress(ILifeTime progressLifeTime)
        {
            yield return PlayFade(progressLifeTime,0, 1, showTime);
        }
        
        protected override IEnumerator OnHidingProgress(ILifeTime progressLifeTime)
        {
            yield return PlayFade(progressLifeTime,1, 0, hideTime);
        }

        private IEnumerator PlayFade(ILifeTime progress,float fromAlpha,float toAlpha, float duration)
        {
            animationTween?.Complete();

            var length = toAlpha - fromAlpha;
            var alpha = canvasGroup.alpha;
            var animationDuration = length > 0 ? 1 - alpha : alpha;
            animationDuration *= duration;
            
            animationTween = canvasGroup.
                DOFade(toAlpha,animationDuration).
                SetEase(Ease.Linear);

            while (progress.IsTerminated == false && animationTween.IsPlaying()) {
                yield return null;
            }
        }
    }
}
