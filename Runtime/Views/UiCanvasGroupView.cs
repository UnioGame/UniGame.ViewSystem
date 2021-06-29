namespace UniGame.UiSystem.Runtime
{
    using Cysharp.Threading.Tasks;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using UniModules.UniUiSystem.Runtime.Utils;
    using UniModules.UniGame.UISystem.Runtime.Abstract;
    using UniRx;
    
    using UnityEngine;

    [RequireComponent(typeof(CanvasGroup))]
    public class UiCanvasGroupView<TWindowModel> : UiView<TWindowModel> where TWindowModel : class, IViewModel
    {
             
        #region inspector
        
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Required]
#endif
        [SerializeField] public CanvasGroup canvasGroup;

        [SerializeField] public bool showByDefault = false;
        
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup(nameof(canvasGroup),false)]  
#endif
        [SerializeField]
        public CanvasGroupState hiddenState = new CanvasGroupState() {
            Alpha = 0,
            BlockRaycasts = false,
            Interactable  = false
        };
        
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup(nameof(canvasGroup),false)]  
#endif 
        [SerializeField]
        public CanvasGroupState visibleState = new CanvasGroupState() {
            Alpha = 1,
            BlockRaycasts = true,
            Interactable  = true
        };
        
        #endregion

        public sealed override CanvasGroup CanvasGroup => canvasGroup;
        

        protected sealed override async UniTask OnInitialize(TWindowModel model)
        {
            await base.OnInitialize(model);

            IsVisible.
                Where(x => x).
                Subscribe(x => canvasGroup.SetState(visibleState)).
                AddTo(LifeTime);
            
            IsVisible.
                Where(x => !x).
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
            
            canvasGroup.SetState(showByDefault ? visibleState : hiddenState);
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