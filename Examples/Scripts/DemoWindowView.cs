namespace UniGame.UiSystem.Examples.BaseUiManager
{
    using System.Collections;
    using DG.Tweening;
    using Runtime;
    using Runtime.Abstracts;
    using UniGreenModules.UniCore.Runtime.DataFlow.Interfaces;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniModules.UniGame.UISystem.Examples.Scripts;
    using UniRx.Async;
    using UnityEngine;

    public class DemoWindowView : WindowView<IViewModel>
    {
        public float showTime = 3f;
        public float hideTime = 3f;

        public RectTransform demoControlParent;
        
        private Tween animationTween;

        protected override async UniTask OnViewInitialize(IViewModel view)
        {
            await base.OnViewInitialize(view);
            
            LifeTime.AddCleanUpAction(() => animationTween?.Complete());
            Layouts.Create<DemoControlView>(new DemoControlViewModel(),parent:demoControlParent);
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
            
            canvasGroup.alpha = fromAlpha;
            
            animationTween = canvasGroup.
                DOFade(toAlpha,duration).
                SetEase(Ease.Linear);

            while (progress.IsTerminated == false && animationTween.IsPlaying()) {
                yield return null;
            }
        }
    }
}
