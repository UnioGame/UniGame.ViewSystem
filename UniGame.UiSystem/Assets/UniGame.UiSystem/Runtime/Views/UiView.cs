using System.Collections;
using UniGreenModules.UniRoutine.Runtime;

namespace UniGame.UiSystem.Runtime
{
    using System;
    using System.Collections.Generic;
    using Abstracts;
    using UniCore.Runtime.ProfilerTools;
    using UniGreenModules.UniCore.Runtime.Attributes;
    using UniGreenModules.UniCore.Runtime.DataFlow;
    using UniGreenModules.UniCore.Runtime.DataFlow.Interfaces;
    using UniGreenModules.UniCore.Runtime.Rx.Extensions;
    using UniGreenModules.UniGame.Core.Runtime.Rx;
    using UniGreenModules.UniGame.UiSystem.Runtime.Extensions;
    using UniRx;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public abstract class UiView<TViewModel> :
        UIBehaviour, IUiContainer,
        IUiView<TViewModel> where TViewModel : class, IViewModel
    {
        #region inspector

        [ReadOnlyValue]
        [SerializeField]
        private bool _isVisible;

        #endregion
        
        private IViewProvider _viewLayout;
        private readonly LifeTimeDefinition _lifeTimeDefinition = new LifeTimeDefinition();
        private readonly LifeTimeDefinition _progressLifeTime = new LifeTimeDefinition();
        
        /// <summary>
        /// view statuses reactions
        /// </summary>
        private readonly RecycleReactiveProperty<IView> _closeReactiveValue = new RecycleReactiveProperty<IView>();
        private readonly RecycleReactiveProperty<IView> _viewShown = new RecycleReactiveProperty<IView>();
        private readonly RecycleReactiveProperty<IView> _viewHidden = new RecycleReactiveProperty<IView>();
        
        /// <summary>
        /// ui element visibility status
        /// </summary>
        private readonly BoolRecycleReactiveProperty _visibility = new BoolRecycleReactiveProperty();
        /// <summary>
        /// model container
        /// </summary>
        private readonly ReactiveProperty<TViewModel> _viewModel = new ReactiveProperty<TViewModel>();
        
        private readonly IList<IView> _containViews = new List<IView>();

        #region public properties

        public bool IsDestroyed { get; private set; }

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
        public IViewProvider Layouts => _viewLayout;
        
        public IObservable<IView> OnHidden => _viewHidden;

        public IObservable<IView> OnShown => _viewShown;

        public IObservable<IView> OnClosed => _closeReactiveValue;
        
        #endregion

        #region public methods

        public void Initialize(IViewModel model, IViewProvider layouts)
        {
            //restart view lifetime
            _lifeTimeDefinition.Release();
            _progressLifeTime.Release();
            
            IsDestroyed = false;

            //save model as context data
            if (model is TViewModel modelData)
            {
                _viewModel.Value = modelData;
            }
            else
            {
                throw new ArgumentException($"VIEW: {name} wrong model type. Target type {typeof(TViewModel).Name} : model Type {model?.GetType().Name}");
            }

            _viewLayout = layouts;

            InitializeHandlers(model);
            BindLifeTimeActions(model);
            
            //custom initialization
            OnInitialize(modelData);
        }

        /// <summary>
        /// show active view
        /// </summary>
        public virtual void Show() => StartProgressAction(_progressLifeTime, OnShow);

        /// <summary>
        /// hide view without release it
        /// </summary>
        public void Hide() => StartProgressAction(_progressLifeTime, OnHiding);

        /// <summary>
        /// end of view lifetime
        /// </summary>
        public void Close() => StartProgressAction(_progressLifeTime, OnClose);
        
        /// <summary>
        /// complete view lifetime immediately
        /// </summary>
        public void Destroy() => _lifeTimeDefinition.Terminate();

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
            yield return OnCloseProgress(_progressLifeTime);
            _lifeTimeDefinition.Terminate();
        }
        
        /// <summary>
        /// hide process
        /// </summary>
        private IEnumerator OnHiding()
        {
            //set view as inactive
            _visibility.SetValueForce(false);
            //wait until user defined closing operation complete
            yield return OnHidingProgress(_progressLifeTime);
        }
        
        /// <summary>
        /// hide process
        /// </summary>
        private IEnumerator OnShow()
        {
            //set view as active
            _visibility.SetValueForce(true);
            yield return OnShowProgress(_progressLifeTime);
        }

        /// <summary>
        /// close continuation
        /// use hiding progress by default
        /// </summary>
        protected virtual IEnumerator OnCloseProgress(ILifeTime progressLifeTime)
        {
            yield return OnHidingProgress(progressLifeTime);
        }

        /// <summary>
        /// showing continuation
        /// </summary>
        protected virtual IEnumerator OnShowProgress(ILifeTime progressLifeTime)
        {
            yield break;
        }
        
        /// <summary>
        /// hiding continuation
        /// </summary>
        protected virtual IEnumerator OnHidingProgress(ILifeTime progressLifeTime)
        {
            yield break;
        }
        
        protected void OnDestroy()
        {
            GameLog.LogFormat("View {0} Destroyed",name);
            Close();
        }

        private void StartProgressAction(LifeTimeDefinition lifeTime,Func<IEnumerator> action)
        {
            if (lifeTime.IsTerminated) 
                return;
            
            lifeTime.Release();
            action().Execute().
                AddTo(lifeTime);
        }

        private void InitializeHandlers(IViewModel model)
        {
            _isVisible = _visibility.Value;
            _visibility.
                Subscribe(x => _isVisible = x).
                AddTo(_lifeTimeDefinition);

            _visibility.Where(x => x).
                Subscribe(x => _viewShown.SetValueForce(this)).
                AddTo(_lifeTimeDefinition);
            
            _visibility.Where(x => !x).
                Subscribe(x => _viewHidden.SetValueForce(this)).
                AddTo(_lifeTimeDefinition);
        }

        private void BindLifeTimeActions(IViewModel model)
        {
            //bind model lifetime to local
            var modelLifeTime = model.LifeTime;
            modelLifeTime.AddCleanUpAction(Close);
            
            //terminate model when view closed
            _lifeTimeDefinition.AddDispose(model);

            _lifeTimeDefinition.AddCleanUpAction(() => {
                _viewLayout = null;
                _visibility.Release();
                _viewHidden.Release();
                _viewShown.Release();
            });

            _lifeTimeDefinition.AddCleanUpAction(_progressLifeTime.Terminate);

            //clean up view and notify observers
            _lifeTimeDefinition.AddCleanUpAction(() => {
                _closeReactiveValue.SetValueForce(this);
                _closeReactiveValue.Release();
                IsDestroyed = true;
            });

            _lifeTimeDefinition.AddCleanUpAction(() => {
                foreach (var view in _containViews) {
                    view.Close();
                }
            });
        }

        public void Add(IView view)
        {
            if (!_containViews.Contains(view)) {
                if (view is MonoBehaviour monoBehaviourView) {
                    _containViews.Add(view);
                    monoBehaviourView.transform.SetParent(transform, false);
                    
                    view.OnClosed.Subscribe(_ => Remove(view)).AddTo(view.LifeTime);
                }
            }
        }

        public void Remove(IView view)
        {
            if (_containViews.Contains(view)) {
                _containViews.Remove(view);
            }
        }
    }
}