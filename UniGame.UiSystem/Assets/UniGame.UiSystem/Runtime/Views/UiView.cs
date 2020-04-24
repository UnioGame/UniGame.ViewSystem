namespace UniGame.UiSystem.Runtime
{
    using System;
    using Abstracts;
    using UniGreenModules.UniGame.UiSystem.Runtime.Extensions;
    using UniRx;

    public abstract class UiView<TViewModel> :
        ViewBase, 
        IUiView<TViewModel> where TViewModel : class, IViewModel
    {
        /// <summary>
        /// model container
        /// </summary>
        private ReactiveProperty<TViewModel> _viewModel = new ReactiveProperty<TViewModel>();

        #region public properties

        public TViewModel Model => _viewModel.Value;

        #endregion

        #region public methods

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

        protected sealed override void OnInitialize(IViewModel model)
        {
            //save model as context data
            if (model is TViewModel modelData)
            {
                _viewModel.Value = modelData;
            }
            else
            {
                throw new ArgumentException($"VIEW: {name} wrong model type. Target type {typeof(TViewModel).Name} : model Type {model?.GetType().Name}");
            }

            LifeTime.AddCleanUpAction(() => _viewModel.Value = null);
            
            base.OnInitialize(modelData);
        }

        /// <summary>
        /// custom initialization methods
        /// </summary>
        protected virtual void OnInitialize(TViewModel model) { }
        
    }
}