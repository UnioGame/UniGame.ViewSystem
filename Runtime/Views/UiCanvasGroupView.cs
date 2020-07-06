namespace UniGame.UiSystem.Runtime
{
    using Abstracts;
    using UniGreenModules.UniCore.Runtime.Rx.Extensions;
    using UniGreenModules.UniUiSystem.Runtime.Utils;
    using UniRx;
    using UniRx.Async;
    using UnityEngine;

    
    [RequireComponent(typeof(CanvasGroup))]
    public class UiCanvasGroupView<TWindowModel> : UiView<TWindowModel> where TWindowModel : class, IViewModel
    {
             
        #region inspector
        
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Required]
#endif
        [SerializeField] protected CanvasGroup canvasGroup;
        
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup(nameof(canvasGroup),false)]  
#endif
        [SerializeField]
        private CanvasGroupState hiddenState = new CanvasGroupState() {
            Alpha         = 0,
            BlockRaycasts = false,
            Interactable  = false
        };
        
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup(nameof(canvasGroup),false)]  
#endif 
        [SerializeField]
        private CanvasGroupState visibleState = new CanvasGroupState() {
            Alpha         = 1,
            BlockRaycasts = true,
            Interactable  = true
        };
        
        #endregion

        protected sealed override async UniTask OnInitialize(TWindowModel model)
        {
            await base.OnInitialize(model);
            
            IsActive.Where(x => x).
                Subscribe(x => canvasGroup.SetState(visibleState)).
                AddTo(LifeTime);
            
            IsActive.Where(x => !x).
                Subscribe(x => canvasGroup.SetState(hiddenState)).
                AddTo(LifeTime);

            await OnViewInitialize(model);
        }

        protected virtual async UniTask OnViewInitialize(TWindowModel model) {}

        protected override void Awake()
        {
            base.Awake();
            canvasGroup = canvasGroup == null ? 
                GetComponent<CanvasGroup>() : 
                canvasGroup;
        }
        
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            canvasGroup = canvasGroup == null ? 
                GetComponent<CanvasGroup>() : 
                canvasGroup;
        }
#endif
        
    }
}