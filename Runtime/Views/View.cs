namespace UniGame.UiSystem.Runtime
{
    using Cysharp.Threading.Tasks;
    using UniCore.Runtime.ProfilerTools;
    using ViewSystem.Runtime;
    using UniRx;
    using UnityEngine;
    
    public abstract class View<TViewModel> :
        ViewBase, 
        IUiView<TViewModel> where TViewModel : class, IViewModel
    {
        /// <summary>
        /// model container
        /// </summary>
        private readonly ReactiveProperty<TViewModel> _viewModel = new ReactiveProperty<TViewModel>();
        private CanvasGroup   _canvasGroup;
        private Canvas   _canvas;

        #region public properties

        public TViewModel Model => _viewModel.Value;

        public virtual CanvasGroup CanvasGroup => (_canvasGroup = _canvasGroup 
            ? _canvasGroup 
            : GetComponent<CanvasGroup>());
        
        public virtual Canvas Canvas  => (_canvas = _canvas 
            ? _canvas 
            : GetComponent<Canvas>());

        #endregion

        protected sealed override async UniTask OnInitialize(IViewModel model)
        {
            await base.OnInitialize(model);
            
            LifeTime.AddCleanUpAction(() => _viewModel.Value = null);

            var modelData = model as TViewModel;
            _viewModel.Value = modelData;
            
            //save model as context data
            if (modelData==null)
            {
                GameLog.LogError($"VIEW: {name} wrong model type. Target type {typeof(TViewModel).Name} : model Type {model?.GetType().Name}");
            }
            
            await OnInitialize(modelData);
        }

        /// <summary>
        /// custom initialization methods
        /// </summary>
        protected virtual UniTask OnInitialize(TViewModel model) => UniTask.CompletedTask;
    }
}