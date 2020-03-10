namespace UniGreenModules.UniGame.UiSystem.Runtime
{
    using Abstracts;
    using UniCore.Runtime.DataFlow.Interfaces;
    using UniCore.Runtime.Rx.Extensions;
    using UniRx;
    using UnityEngine;
    using UniUiSystem.Runtime.Utils;

    [RequireComponent(typeof(CanvasGroup))]
    public class WindowView<TWindowModel> : 
        UiView<TWindowModel> 
        where TWindowModel : class, IViewModel
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
        
        protected sealed override void OnInitialize(TWindowModel model, ILifeTime lifeTime)
        {
            
            IsActive.
                Where(x => x).
                Subscribe(x => canvasGroup.SetState(visibleState)).
                AddTo(LifeTime);
            
            IsActive.
                Where(x => !x).
                Subscribe(x => canvasGroup.SetState(hiddenState)).
                AddTo(LifeTime);

            OnWindowInitialize(model, lifeTime);
        }

        protected virtual void OnWindowInitialize(TWindowModel model, ILifeTime lifeTime)
        {
            
        }
        
        protected virtual void OnAwake() { }
    
        private void Awake()
        {
            canvasGroup = canvasGroup == null ? GetComponent<CanvasGroup>() : canvasGroup;
            OnAwake();
        }
    }
}