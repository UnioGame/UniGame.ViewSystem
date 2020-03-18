namespace UniGame.UiSystem.Examples.BaseUiManager
{
    using System.Collections;
    using DG.Tweening;
    using Runtime;
    using Runtime.Abstracts;

    public class DemoWindowView : WindowView<IViewModel>
    {
        public float showTime = 3f;
        public float hideTime = 3f;

        private Tween animationTween;

        protected override void OnViewInitialize(IViewModel view)
        {
            LifeTime.AddCleanUpAction(() => animationTween?.Complete());
        }
        
        protected override IEnumerator OnShowProgress()
        {
            yield return PlayFade(0, 1, showTime);
        }
        
        protected override IEnumerator OnHidingProgress()
        {
            yield return PlayFade(1, 0, hideTime);
        }

        private IEnumerator PlayFade(float fromAlpha,float toAlpha, float duration)
        {
            animationTween?.Complete();

            canvasGroup.alpha = fromAlpha;
            
            animationTween = canvasGroup.
                DOFade(toAlpha,duration).
                SetEase(Ease.Linear);

            while (animationTween.IsPlaying()) {
                yield return null;
            }
        }
    }
}
