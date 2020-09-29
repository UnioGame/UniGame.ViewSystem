namespace UniGame.UiSystem.Examples.BaseUiManager
{
    using System.Collections;
    using Cysharp.Threading.Tasks;
    using Runtime;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniModules.UniGame.UISystem.Examples.Scripts;
    using UniModules.UniGame.UISystem.Runtime.Abstract;
    using UnityEngine;

    public class DemoWindowView : WindowView<IViewModel>
    {
        public float showTime = 3f;
        public float hideTime = 3f;

        public RectTransform demoControlParent;

        protected override async UniTask OnViewInitialize(IViewModel view)
        {
            await base.OnViewInitialize(view);
            
            Layout.Create<DemoControlView>(new DemoControlViewModel(),parent:demoControlParent);
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
            canvasGroup.alpha = fromAlpha;

            var animationTime = 0f;
            while (animationTime < duration)
            {
                var timeProgression = duration <= 0 ? 1 : animationTime / duration;
                canvasGroup.alpha =  Mathf.Lerp(fromAlpha, toAlpha, timeProgression);
                animationTime     += Time.deltaTime;
                yield return null;
            }

        }
    }
}
