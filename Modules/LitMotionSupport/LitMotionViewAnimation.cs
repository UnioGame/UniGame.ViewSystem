namespace ViewSystem.Modules.LitMotionSupport
{
    using System;
    using Cysharp.Threading.Tasks;
    using LitMotion.Animation;
    using UniGame.Core.Runtime;
    using UniGame.ViewSystem.Runtime;
    using UniGame.ViewSystem.Runtime.Views.Abstract;
    using UniModules.UniUiSystem.Runtime.Utils;
    using UnityEngine;

#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif
    
    [Serializable]
    public class LitMotionViewAnimation : IViewAnimation,IDisposable
    {
        #region inspector
        
#if ODIN_INSPECTOR
        [TitleGroup("Animation Settings")] 
#endif
        public bool enabled = false;

#if ODIN_INSPECTOR
        [ShowIf(nameof(enabled))] [TitleGroup("Animation Settings")]
#endif
        public bool animateShowing = true;
        
#if ODIN_INSPECTOR
        [ShowIf(nameof(enabled))] [TitleGroup("Animation Settings")]
#endif
        public bool animateHiding = true;
        
#if ODIN_INSPECTOR
        [ShowIf(nameof(enabled))] [TitleGroup("Animation Settings")]
#endif
        public bool animateClosing = true;

        public bool controlCanvasGroup = true;
        
        public bool stopWhenFinished = true;
        
#if ODIN_INSPECTOR
        [ShowIf(nameof(controlCanvasGroup))]
#endif
        public CanvasGroup group;
        public LitMotionAnimation showAnimation;
        public LitMotionAnimation hideAnimation;
        
        #endregion

        public CanvasGroup GetGroup(IView view)
        {
            var gameObject = view.GameObject;
            var canvasGroup = group ?? gameObject.GetComponent<CanvasGroup>();
            canvasGroup ??= gameObject.AddComponent<CanvasGroup>(); 
            return canvasGroup;
        }

        public bool IsEnabled => enabled;

        public async UniTask PlayAnimation(IView view, ViewStatus status,ILifeTime lifeTime)
        {
            if (!enabled) return;
            
            switch (status)
            {
                case ViewStatus.Showing:
                    await Show(view,lifeTime);
                    break;
                case ViewStatus.Hiding:
                    await Hide(view,lifeTime);
                    break;
                case ViewStatus.Shown:
                    SetCanvasGroupValue(view,1);
                    break;
                case ViewStatus.None:
                case ViewStatus.Closed:
                case ViewStatus.Hidden:
                    SetCanvasGroupValue(view,0);
                    break;
            }
        }

        public void SetCanvasGroupValue(IView view, float value)
        {
            if (!controlCanvasGroup) return;
            if (!IsViewAlive(view)) return;
            
            var canvasGroup = GetGroup(view);
            canvasGroup.SetState(value);
        }

        public async UniTask Show(IView view, ILifeTime lifeTime)
        {
            if(!animateShowing) return;

            var canceled = await UniTask.WaitForEndOfFrame(lifeTime.Token)
                .SuppressCancellationThrow();

            if (canceled || !IsViewAlive(view))
                return;
            
            SetCanvasGroupValue(view,1);
            
            await PlayAnimation(view, showAnimation, lifeTime);
        }

        public async UniTask Close(IView view, ILifeTime lifeTime)
        {
            if (!animateClosing) return;
            await PlayAnimation(view, hideAnimation, lifeTime);
        }

        public async UniTask Hide(IView view, ILifeTime lifeTime)
        {
            if (!animateHiding) return;
            
            var canceled = await UniTask.WaitForEndOfFrame(lifeTime.Token)
                .SuppressCancellationThrow();

            if (canceled || !IsViewAlive(view))
                return;
            
            SetCanvasGroupValue(view,1);
            
            await PlayAnimation(view, hideAnimation, lifeTime);
        }

        public void Stop()
        {
            showAnimation?.Stop();
            hideAnimation?.Stop();
        }
        
        public async UniTask PlayAnimation(IView view, LitMotionAnimation animation, ILifeTime lifeTime)
        {
            Stop();

            if (animation == null || lifeTime.IsTerminated || !IsViewAlive(view))
                return;

            lifeTime.AddCleanUpAction(Stop);
            
            animation.Restart();
            
            var context = new LitMotionViewAnimationContext()
            {
                animation = animation,
                view = view,
                lifeTime = lifeTime,
            };

            var canceled = false;

            try
            {
                canceled = await UniTask.WaitWhile(context,static x => 
                    !x.lifeTime.IsTerminated &&
                    IsViewAlive(x.view) &&
                    x.animation.IsPlaying,cancellationToken:lifeTime.Token)
                    .SuppressCancellationThrow();
            }
            finally
            {
                if (canceled || lifeTime.IsTerminated || !IsViewAlive(view) || stopWhenFinished)
                    animation.Stop();
            }
        }

        private static bool IsViewAlive(IView view)
        {
            return view != null &&
                   !view.LifeTime.IsTerminated &&
                   view.GameObject != null;
        }

        public void Dispose()
        {
            Stop();
        }
    }

    public struct LitMotionViewAnimationContext
    {
        public LitMotionAnimation animation;
        public IView view;
        public ILifeTime lifeTime;
    }
}
