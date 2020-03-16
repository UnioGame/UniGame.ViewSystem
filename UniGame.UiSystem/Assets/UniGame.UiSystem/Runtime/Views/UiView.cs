using System.Collections;
using UniGreenModules.UniRoutine.Runtime;

namespace UniGame.UiSystem.Runtime
{
    using System;
    using Abstracts;
    using UniGreenModules.UniCore.Runtime.DataFlow;
    using UniGreenModules.UniCore.Runtime.DataFlow.Interfaces;
    using UniGreenModules.UniCore.Runtime.ProfilerTools;
    using UniGreenModules.UniGame.Core.Runtime.Rx;
    using UniGreenModules.UniGame.UiSystem.Runtime.Extensions;
    using UniRx;
    using UnityEngine;

    public abstract class UiView<TViewModel> :
        MonoBehaviour, IView
        where TViewModel : class, IViewModel
    {
        private IViewElementFactory _viewFactory;
        private LifeTimeDefinition _lifeTimeDefinition = new LifeTimeDefinition();
        private RecycleReactiveProperty<IView> _closeReactiveValue = new RecycleReactiveProperty<IView>();
        
        private RecycleReactiveProperty<IView> _viewShown = new RecycleReactiveProperty<IView>();
        private RecycleReactiveProperty<IView> _viewHidden = new RecycleReactiveProperty<IView>();
        /// <summary>
        /// ui element visibility status
        /// </summary>
        private BoolRecycleReactiveProperty _visibility = new BoolRecycleReactiveProperty();
        /// <summary>
        /// model container
        /// </summary>
        private ReactiveProperty<TViewModel> _viewModel = new ReactiveProperty<TViewModel>();

        #region public properties

        public TViewModel Model => _viewModel.Value;

        /// <summary>
        /// Is View Active
        /// </summary>
        public IReadOnlyReactiveProperty<bool> IsActive => _visibility;

        /// <summary>
        /// View LifeTime
        /// </summary>
        public ILifeTime LifeTime => _lifeTimeDefinition.LifeTime;

        /// <summary>
        /// views factor
        /// </summary>
        public IViewElementFactory ViewFactory => _viewFactory;
        
        public IObservable<IView> OnHidden => _viewHidden;

        public IObservable<IView> OnShown => _viewHidden;

        public IObservable<IView> OnClosed => _closeReactiveValue;
        
        #endregion

        #region public methods

        public void Initialize(IViewModel model, IViewElementFactory factory)
        {
            //restart view lifetime
            _lifeTimeDefinition.Release();

            //save model as context data
            if (model is TViewModel modelData)
            {
                this._viewModel.Value = modelData;
            }
            else
            {
                throw new ArgumentException($"VIEW: {name} wrong model type. Target type {typeof(TViewModel).Name} : model Type {model?.GetType().Name}");
            }

            this._viewFactory = factory;

            //bind model lifetime to local
            var modelLifeTime = model.LifeTime;
            modelLifeTime.AddCleanUpAction(Close);

            //terminate model when view closed
            _lifeTimeDefinition.AddDispose(model);
            _lifeTimeDefinition.AddCleanUpAction(() => factory = null);
            _lifeTimeDefinition.AddCleanUpAction(() => {
                _visibility.Release();
                _viewHidden.Release();
                _viewShown.Release();
            });
            //clean up view and notify observers
            _lifeTimeDefinition.AddCleanUpAction(() => {
                _closeReactiveValue.Value = this;
                _closeReactiveValue.Release();
            });

            //custom initialization
            OnInitialize(modelData);

        }

        /// <summary>
        /// show active view
        /// </summary>
        public virtual void Show() => _visibility.Value = true;

        /// <summary>
        /// hide view without release it
        /// </summary>
        public virtual void Hide() => _visibility.Value = false;

        /// <summary>
        /// end of view lifetime
        /// </summary>
        public void Close()
        {
            if (_lifeTimeDefinition.IsTerminated) return;
            OnClose().Execute().
                AddTo(LifeTime);
        }
        
        public void Destroy() => _lifeTimeDefinition.Terminate();

        /// <summary>
        /// bind source stream to view action
        /// with View LifeTime context
        /// </summary>
        public UiView<TViewModel> BindTo<T>(IObservable<T> source, Action<T> action) => this.Bind(source, action);

        #endregion

        /// <summary>
        /// custom initialization methods
        /// </summary>
        protected virtual void OnInitialize(TViewModel model) { }

        /// <summary>
        /// view closing process
        /// windows auto terminated on close complete
        /// </summary>
        /// <returns></returns>
        private IEnumerator OnClose()
        {
            //wait until user defined closing operation complete
            yield return OnCloseProgress();
            _lifeTimeDefinition.Terminate();
        }
        
        /// <summary>
        /// hide process
        /// </summary>
        private IEnumerator OnHiding()
        {
            //wait until user defined closing operation complete
            yield return OnHidingProgress();
            
        }

        /// <summary>
        /// close continuation
        /// </summary>
        protected virtual IEnumerator OnCloseProgress()
        {
            yield break;
        }

        /// <summary>
        /// showing continuation
        /// </summary>
        protected virtual IEnumerator OnShowProgress()
        {
            yield break;
        }
        
        /// <summary>
        /// hiding continuation
        /// </summary>
        protected virtual IEnumerator OnHidingProgress()
        {
            yield break;
        }
        
        private void OnDestroy()
        {
            GameLog.Log($"View {name} Destroyed");
            Close();
        }



    }
}