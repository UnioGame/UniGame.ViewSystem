namespace UniGame.ViewSystem.Backgrounds
{
    using UiSystem.Runtime;
    using UniGame.UiSystem.Runtime.Backgrounds.Abstract;
    using Core.Runtime;
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    public class DefaultFadeBackgroundView : View<IBackgroundViewModel>, IBackgroundView
    {
        [SerializeField, Range(0.0f, 1.0f)]
        public float duration = 0.3f;
        
        protected override async UniTask OnHidingProgress(ILifeTime progressLifeTime)
        {
            await AnimateFade(1, 0, duration);
        }

        protected override async UniTask OnShowProgress(ILifeTime progressLifeTime)
        {
            await AnimateFade(0, 1, duration);
        }

        private async UniTask AnimateFade(float fromAlpha, float toAlpha, float time)
        {
            if (time <= 0)
            {
                CanvasGroup.alpha = toAlpha;
                return;
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
                await UniTask.Yield();
            }
        }

    }
}