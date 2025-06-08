﻿namespace UniGame.UiSystem.Runtime
{
    using System;
    using Cysharp.Threading.Tasks;
    using R3;
    using UniGame.Runtime.Rx.Runtime.Extensions;
    using UniCore.Runtime.ProfilerTools;
    using UniModules.UniGame.UiSystem.Runtime.Extensions;
    using UniModules.UniUiSystem.Runtime.Utils;
    using ViewSystem.Runtime;
     
    using UnityEngine;
    
    public abstract class View<TViewModel> :
        ViewBase, 
        IUiView<TViewModel> where TViewModel : class, IViewModel
    {
        /// <summary>
        /// model container
        /// </summary>
        private readonly ReactiveProperty<TViewModel> _viewModel = new();
        private CanvasGroup   _canvasGroup;
        private Canvas   _canvas;

        #region public properties

        public TViewModel Model => _viewModel.Value;

        public sealed override Type ModelType => typeof(TViewModel);
        
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

            BindViewModel(model);
            
            await OnInitialize(modelData);
            
            await UniTask.Yield();
        }

        /// <summary>
        /// custom initialization methods
        /// </summary>
        protected virtual UniTask OnInitialize(TViewModel model) => UniTask.CompletedTask;
        
        private void BindViewModel(IViewModel model)
        {
            if (model is ICloseableViewModel closeable)
                this.Bind(closeable.CloseCommand, Close);
        }

    }
}