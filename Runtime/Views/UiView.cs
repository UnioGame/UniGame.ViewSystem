namespace UniGame.UiSystem.Runtime
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniCore.Runtime.ProfilerTools;
    using UniModules.UniGame.UiSystem.Runtime.Extensions;
    using UniModules.UniGame.UISystem.Runtime.Abstract;
    using UniRx;
    using UnityEngine;
    
    public abstract class UiView<TViewModel> :
        ViewBase, 
        IUiView<TViewModel> where TViewModel : class, IViewModel
    {
        /// <summary>
        /// model container
        /// </summary>
        private ReactiveProperty<TViewModel> _viewModel = new ReactiveProperty<TViewModel>();
        private CanvasGroup   _canvasGroup;

        #region public properties

        public TViewModel Model => _viewModel.Value;

        public virtual CanvasGroup CanvasGroup => (_canvasGroup = _canvasGroup ? _canvasGroup : GetComponent<CanvasGroup>());

        #endregion

        #region public methods

        //TODO use async
        /// <summary>
        /// bind source stream to view action
        /// with View LifeTime context
        /// </summary>
        public UiView<TViewModel> BindTo<T>(IObservable<T> source, Action<T> action)
        {
            var result = this.Bind(source, action);
            return result;
        }

        #endregion

        protected sealed override async UniTask OnInitialize(IViewModel model)
        {
            await base.OnInitialize(model);
            
            LifeTime.AddCleanUpAction(() =>
            {
                _viewModel.Value = null;
            });

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
        protected virtual async UniTask OnInitialize(TViewModel model) { }
    }
}