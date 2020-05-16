namespace UniGame.UiSystem.Runtime
{
    using System;
    using System.Collections;
    using Abstracts;
    using UniCore.Runtime.ProfilerTools;
    using UniGreenModules.UniCore.Runtime.Attributes;
    using UniGreenModules.UniCore.Runtime.DataFlow;
    using UniGreenModules.UniCore.Runtime.DataFlow.Interfaces;
    using UniGreenModules.UniCore.Runtime.Rx.Extensions;
    using UniGreenModules.UniGame.Core.Runtime.Rx;
    using UniGreenModules.UniGame.UiSystem.Runtime.Extensions;
    using UniGreenModules.UniRoutine.Runtime;
    using UniRx;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public abstract class ViewBase : UIBehaviour, IView
    {
        #region inspector

        [ReadOnlyValue]
        [SerializeField]
        private bool _isVisible;

        #endregion

        private RectTransform rectTransform;
        
        private LifeTimeDefinition _lifeTimeDefinition = new LifeTimeDefinition();
        private LifeTimeDefinition _progressLifeTime = new LifeTimeDefinition();
        
        /// <summary>
        /// ui element visibility status
        /// </summary>
        private BoolRecycleReactiveProperty _visibility = new BoolRecycleReactiveProperty();

        /// <summary>
        /// view statuses reactions
        /// </summary>
        private RecycleReactiveProperty<IView> _closeReactiveValue = new RecycleReactiveProperty<IView>();
        private RecycleReactiveProperty<IView> _viewShown  = new RecycleReactiveProperty<IView>();
        private RecycleReactiveProperty<IView> _viewHidden = new RecycleReactiveProperty<IView>();

        private IViewProvider _viewLayout;
        
        #region public properties

        /// <summary>
        /// View LifeTime
        /// </summary>
        public ILifeTime LifeTime => _lifeTimeDefinition.LifeTime;

        /// <summary>
        /// view transform
        /// </summary>
        public RectTransform RectTransform => rectTransform != null ? 
            rectTransform:
            (rectTransform = transform as RectTransform);

        /// <summary>
        /// views layout
        /// </summary>
        public IViewProvider Layouts => _viewLayout;
        
        public IObservable<IView> OnHidden => _viewHidden;

        public IObservable<IView> OnShown => _viewShown;

        public IObservable<IView> OnClosed => _closeReactiveValue;

        /// <summary>
        /// Is View Active
        /// </summary>
        public IReadOnlyReactiveProperty<bool> IsActive => _visibility;

        public bool IsTerminated { get; private set; }

        public IViewModel Context { get; private set; }

        #endregion

        /// <summary>
        /// complete view lifetime immediately
        /// </summary>
        public void Destroy() => _lifeTimeDefinition.Terminate();
        
        public void Initialize(IViewModel model, IViewProvider layouts)
        {
            //restart view lifetime
            _lifeTimeDefinition.Release();
            _progressLifeTime.Release();

            _viewLayout = layouts;

            InitializeHandlers(model);
            BindLifeTimeActions(model);
            
            //custom initialization
            OnInitialize(model);
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
        /// bind source stream to view action
        /// with View LifeTime context
        /// </summary>
        public IView BindToView<T>(IObservable<T> source, Action<T> action)
        {
            var result = this.Bind(source, action);
            return result;
        }
        
        
        #region private methods
        
        /// <summary>
        /// custom initialization methods
        /// </summary>
        protected virtual void OnInitialize(IViewModel model) { }
        
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

        private void StartProgressAction(LifeTimeDefinition lifeTime,Func<IEnumerator> action)
        {
            if (lifeTime.IsTerminated) return;
            lifeTime.Release();
            action().Execute().
                AddTo(lifeTime);
        }

        private void InitializeHandlers(IViewModel model)
        {
            Context = model;
            IsTerminated = false;

            _isVisible = _visibility.Value;
            _visibility.
                Subscribe(x => this._isVisible = x).
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
                Context = null;
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
                IsTerminated = true;
            });
        }
        
        protected void OnDestroy()
        {
            GameLog.LogFormat("View {0} Destroyed",name);
            Close();
        }

        #endregion
        
    }
}