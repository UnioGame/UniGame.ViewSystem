namespace UniGame.ViewSystem.Runtime.Animations
{
    using System;
    using Cysharp.Threading.Tasks;
    using Views.Abstract;
    using global::UniGame.Core.Runtime;
    using Runtime;
    using global::UniModules.UniUiSystem.Runtime.Utils;
    using UnityEngine;

#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif
    
    [Serializable]
    public class SimpleFadeViewAnimation : IViewAnimation
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

       
#if ODIN_INSPECTOR
        [ShowIf(nameof(enabled))]
        [TitleGroup("Animation Settings")]
#endif
        public float duration = 0.2f;
        
#if ODIN_INSPECTOR
        [ShowIf(nameof(enabled))]
        [TitleGroup("Animation Settings")]
#endif
        public bool unscaledTime;

        [SerializeField]
#if ODIN_INSPECTOR
        [ShowIf(nameof(enabled))] [FoldoutGroup("CanvasGroup")] 
#endif
        public CanvasGroupState hiddenState = new()
        {
            Alpha = 0,
            BlockRaycasts = false,
            Interactable = false
        };

        [SerializeField]
#if ODIN_INSPECTOR
        [ShowIf(nameof(enabled))] [FoldoutGroup("CanvasGroup")] 
#endif
        public CanvasGroupState visibleState = new()
        {
            Alpha = 1,
            BlockRaycasts = true,
            Interactable = true
        };

        #endregion

        public bool IsEnabled => enabled;

        public async UniTask PlayAnimation(IView view, ViewStatus status,ILifeTime lifeTime)
        {
            if (!enabled) return;
            var group = GetGroup(view);

            switch (status)
            {
                case ViewStatus.Showing:
                    await Show(view,lifeTime);
                    break;
                case ViewStatus.Hiding:
                    await Hide(view,lifeTime);
                    break;
                case ViewStatus.Shown:
                    group.SetState(visibleState);
                    break;
                case ViewStatus.None:
                case ViewStatus.Closed:
                case ViewStatus.Hidden:
                    group.SetState(hiddenState);
                    break;
            }
        }
        
        public async UniTask Show(IView view, ILifeTime lifeTime)
        {
            if (!animateShowing)
                return;

            if (!enabled)
                return;

            await AnimateFade(view, hiddenState, visibleState, duration, lifeTime);
        }

        public async UniTask Close(IView view, ILifeTime lifeTime)
        {
            if (!animateClosing) return;
            await HideAnimation(view, lifeTime);
        }

        public async UniTask Hide(IView view, ILifeTime lifeTime)
        {
            if (!animateHiding) return;
            await HideAnimation(view, lifeTime);
        }

        public async UniTask HideAnimation(IView view, ILifeTime lifeTime)
        {
            if (!enabled)
                return;

            await AnimateFade(view, visibleState, hiddenState, duration, lifeTime);
        }

        public CanvasGroup GetGroup(IView view)
        {
            var gameObject = view.GameObject;

            var canvasGroup = gameObject.GetComponent<CanvasGroup>();
            if (!canvasGroup) canvasGroup = gameObject.AddComponent<CanvasGroup>(); 
            return canvasGroup;
        }

        private async UniTask AnimateFade(IView view, CanvasGroupState from, CanvasGroupState to, float duration,
            ILifeTime progressLifeTime)
        {
            var canvasGroup = GetGroup(view);
            if (canvasGroup == null) return;

            if (duration <= 0)
            {
                canvasGroup.SetState(to);
                return;
            }

            canvasGroup.alpha              = from.Alpha;
            canvasGroup.interactable       = to.Interactable;
            canvasGroup.ignoreParentGroups = to.IgnoreParent;
            canvasGroup.blocksRaycasts     = to.BlockRaycasts;

            var fromAlpha = from.Alpha;
            var toAlpha = to.Alpha;

            var token = progressLifeTime.Token;
            var startTime = unscaledTime ? Time.unscaledTime : Time.time;
            var finishTime = startTime + duration;

            try
            {
                var time = unscaledTime ? Time.unscaledTime : Time.time;
                while (!token.IsCancellationRequested && time < finishTime)
                {
                    var timePassed = (time - startTime);
                    var progress = timePassed / duration;
                    var alpha = Mathf.Lerp(fromAlpha, toAlpha, progress);
                    canvasGroup.alpha = alpha;
                    time = unscaledTime ? Time.unscaledTime : Time.time;
                    await UniTask.Yield();
                }
            }
            finally
            {
                canvasGroup.SetState(to);
            }
        }

    }
}