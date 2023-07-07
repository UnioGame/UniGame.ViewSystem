namespace UniGame.UiSystem.Runtime
{
    using System;
    using System.Collections;
    using Cysharp.Threading.Tasks;
    using Core.Runtime;
    using UniModules.UniUiSystem.Runtime.Utils;
    using ViewSystem.Runtime;
    using UniRx;
    using UnityEngine;

    [RequireComponent(typeof(CanvasGroup))]
    public class UiCanvasGroupView<TWindowModel> : UiView<TWindowModel> 
        where TWindowModel : class, IViewModel
    {
        #region inspector

        [SerializeField] public bool showByDefault = false;
        
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup(nameof(CanvasGroup),false)]  
#endif
        [SerializeField]
        public CanvasGroupState hiddenState = new CanvasGroupState
        {
            Alpha = 0,
            BlockRaycasts = false,
            Interactable  = false
        };

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup(nameof(CanvasGroup),false)]  
#endif 
        [SerializeField]
        public CanvasGroupState visibleState = new CanvasGroupState
        {
            Alpha = 1,
            BlockRaycasts = true,
            Interactable  = true
        };

        #endregion

        protected IDisposable VisibilityHandler;

        protected sealed override async UniTask OnInitialize(TWindowModel model)
        {
            await base.OnInitialize(model);

            VisibilityHandler = IsVisible
                .Where(x => x)
                .Where(x => CanvasGroup != null)
                .Subscribe(x => CanvasGroup.SetState(visibleState))
                .AddTo(LifeTime);
            
            IsVisible.
                Where(x => !x).
                Subscribe(x => CanvasGroup.SetState(hiddenState)).
                AddTo(LifeTime);

            await OnViewInitialize(model);
            
            if (showByDefault) Show();
        }

        protected virtual UniTask OnViewInitialize(TWindowModel model) => UniTask.CompletedTask;

        protected sealed override async UniTask OnCloseProgress(ILifeTime progressLifeTime)
        {
            CanvasGroup.SetBlocksRaycast(hiddenState.BlockRaycasts);
            await OnCloseProgressOverride(progressLifeTime);
        }

        protected sealed override async UniTask  OnHidingProgress(ILifeTime progressLifeTime)
        {
            CanvasGroup.SetBlocksRaycast(hiddenState.BlockRaycasts);
            await OnHidingProgressOverride(progressLifeTime);
        }

        protected sealed override async UniTask  OnShowProgress(ILifeTime progressLifeTime)
        {
            CanvasGroup.SetBlocksRaycast(visibleState.BlockRaycasts);
            await OnShowProgressOverride(progressLifeTime);
        }

        protected virtual async UniTask OnCloseProgressOverride(ILifeTime progressLifeTime)
        {
            await OnHidingProgress(progressLifeTime);
        }

        protected virtual UniTask OnHidingProgressOverride(ILifeTime progressLifeTime)
        {
            return UniTask.CompletedTask;
        }

        protected virtual UniTask OnShowProgressOverride(ILifeTime progressLifeTime)
        {
            return UniTask.CompletedTask;
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            //canvasGroup.SetState(showByDefault ? visibleState : hiddenState);
        }
    }
}