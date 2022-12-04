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

        protected sealed override IEnumerator OnCloseProgress(ILifeTime progressLifeTime)
        {
            CanvasGroup.SetBlocksRaycast(hiddenState.BlockRaycasts);
            yield return OnCloseProgressOverride(progressLifeTime);
        }

        protected sealed override IEnumerator OnHidingProgress(ILifeTime progressLifeTime)
        {
            CanvasGroup.SetBlocksRaycast(hiddenState.BlockRaycasts);
            yield return OnHidingProgressOverride(progressLifeTime);
        }

        protected sealed override IEnumerator OnShowProgress(ILifeTime progressLifeTime)
        {
            CanvasGroup.SetBlocksRaycast(visibleState.BlockRaycasts);
            yield return OnShowProgressOverride(progressLifeTime);
        }

        protected virtual IEnumerator OnCloseProgressOverride(ILifeTime progressLifeTime)
        {
            yield return OnHidingProgress(progressLifeTime);
        }

        protected virtual IEnumerator OnHidingProgressOverride(ILifeTime progressLifeTime)
        {
            yield break;
        }

        protected virtual IEnumerator OnShowProgressOverride(ILifeTime progressLifeTime)
        {
            yield break;
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            //canvasGroup.SetState(showByDefault ? visibleState : hiddenState);
        }
    }
}