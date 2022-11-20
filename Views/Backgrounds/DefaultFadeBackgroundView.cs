namespace UniGame.ViewSystem.Backgrounds
{
    using System.Collections;
    using UiSystem.Runtime;
    using UniGame.UiSystem.Runtime.Backgrounds.Abstract;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UnityEngine;

    public class DefaultFadeBackgroundView : UiCanvasGroupView<IBackgroundViewModel>, IBackgroundView
    {
        [SerializeField, Range(0.0f, 1.0f)]
        public float duration = 0.3f;
        
        protected override IEnumerator OnHidingProgressOverride(ILifeTime progressLifeTime)
        {
            yield return AnimateFade(1, 0, duration);
        }

        protected override IEnumerator OnShowProgressOverride(ILifeTime progressLifeTime)
        {
            yield return AnimateFade(0, 1, duration);
        }

        private IEnumerator AnimateFade(float fromAlpha, float toAlpha, float time)
        {
            if (time <= 0)
            {
                CanvasGroup.alpha = toAlpha;
                yield break;
            }
            
            var currentAlpha = fromAlpha;
            var timePassed = 0f;
            CanvasGroup.alpha = fromAlpha;

            while (timePassed < time)
            {
                var stage = timePassed / time;
                currentAlpha = Mathf.Lerp(fromAlpha, toAlpha, stage);
                CanvasGroup.alpha = currentAlpha;
                timePassed += Time.deltaTime;
                yield return null;
            }
        }

    }
}