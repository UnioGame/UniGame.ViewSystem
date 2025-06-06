namespace ViewSystem.Modules.LitMotionSupport
{
    using System;
    using Cysharp.Threading.Tasks;
    using LitMotion.Animation;
    using Sirenix.OdinInspector;
    using UniGame.Core.Runtime;
    using UniGame.ViewSystem.Runtime;
    using UniGame.ViewSystem.Runtime.Views.Abstract;
    using UniModules.UniGame.UISystem.Runtime;
    using UniModules.UniUiSystem.Runtime.Utils;
    using UnityEngine;

    [Serializable]
    public class LitMotionViewAnimation : IViewAnimation
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
            
            var canvasGroup = GetGroup(view);
            canvasGroup.SetState(value);
        }

        public async UniTask Show(IView view, ILifeTime lifeTime)
        {
            if(!animateShowing) return;

            await UniTask.WaitForEndOfFrame();
            
            SetCanvasGroupValue(view,1);
            
            await PlayAnimation(view, showAnimation)
                .AttachExternalCancellation(lifeTime.Token);
        }

        public async UniTask Close(IView view, ILifeTime lifeTime)
        {
            if (!animateClosing) return;
            await PlayAnimation(view, hideAnimation)
                .AttachExternalCancellation(lifeTime.Token);
        }

        public async UniTask Hide(IView view, ILifeTime lifeTime)
        {
            if (!animateHiding) return;
            
            await UniTask.WaitForEndOfFrame();
            
            SetCanvasGroupValue(view,1);
            await PlayAnimation(view, hideAnimation)
                .AttachExternalCancellation(lifeTime.Token);
        }
        
        public async UniTask PlayAnimation(IView view, LitMotionAnimation animation)
        {
            showAnimation.Stop();
            hideAnimation.Stop();
            
            animation.Restart();
            
            await UniTask.WaitWhile(animation, x => x.IsPlaying);
        }

    }
}