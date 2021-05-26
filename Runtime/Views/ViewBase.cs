using UniModules.UniRoutine.Runtime.Extension;

namespace UniGame.UiSystem.Runtime
{
    using System;
    using System.Collections;
    using Cysharp.Threading.Tasks;
    using UniCore.Runtime.ProfilerTools;
    using UniModules.UniCore.Runtime.Attributes;
    using UniModules.UniCore.Runtime.DataFlow;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using UniModules.UniGame.Core.Runtime.Rx;
    using UniModules.UniGame.UiSystem.Runtime.Extensions;
    using UniModules.UniRoutine.Runtime;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniModules.UniGame.UISystem.Runtime;
    using UniModules.UniGame.UISystem.Runtime.Abstract;
    using UniRx;
    
    using UnityEngine;
    using UnityEngine.EventSystems;

    public abstract class ViewBase : UIBehaviour, 
        IView, ILayoutFactoryView
    {
        #region inspector

        [ReadOnlyValue]
        [SerializeField]
        private bool _isVisible;

        [ReadOnlyValue]
        [SerializeField]
        private BoolRecycleReactiveProperty _isInitialized = new BoolRecycleReactiveProperty();
        
        #endregion

        private RectTransform _rectTransform;
        private Transform _transform;
        
        private readonly LifeTimeDefinition _lifeTimeDefinition = new LifeTimeDefinition();
        private readonly LifeTimeDefinition _progressLifeTime   = new LifeTimeDefinition();
        private readonly LifeTimeDefinition _viewModelLifeTime   = new LifeTimeDefinition();

        /// <summary>
        /// ui element visibility status
        /// </summary>
        private readonly BoolRecycleReactiveProperty _visibility = new BoolRecycleReactiveProperty();

        /// <summary>
        /// view statuses reactions
        /// </summary>
        private readonly RecycleReactiveProperty<ViewStatus> _status = new RecycleReactiveProperty<ViewStatus>();

        private IViewLayoutProvider _viewLayout;

        private bool _isViewOwner;
        
        #region public properties

        public IReadOnlyReactiveProperty<bool> IsInitialized => _isInitialized;
        
        public GameObject Owner => gameObject;
        
        /// <summary>
        /// View LifeTime
        /// </summary>
        public ILifeTime LifeTime => _lifeTimeDefinition;

        /// <summary>
        /// view transform
        /// </summary>
        public RectTransform RectTransform => _rectTransform != null ? 
            _rectTransform:
            _rectTransform = transform as RectTransform;

        
        /// <summary>
        /// view transform
        /// </summary>
        public Transform Transform => _transform = _transform ? _transform : transform;
        
        /// <summary>
        /// views layout
        /// </summary>
        public IViewLayoutProvider Layout => _viewLayout;

        public IReadOnlyReactiveProperty<ViewStatus> Status => _status;
        
        public IObservable<IView> OnHidden  => SelectStatus(ViewStatus.Hidden);
        public IObservable<IView> OnHiding => SelectStatus(ViewStatus.Hiding);
        public IObservable<IView> OnShowing   => SelectStatus(ViewStatus.Showing);

        public IObservable<IView> OnShown => SelectStatus(ViewStatus.Shown);

        public IObservable<IView> OnClosed => SelectStatus(ViewStatus.Closed);

        /// <summary>
        /// Is View Active
        /// </summary>
        public IReadOnlyReactiveProperty<bool> IsVisible => _visibility;

        public bool IsTerminated { get; private set; }

        public IViewModel ViewModel { get; private set; }

        #endregion public properties

        #region public methods

        /// <summary>
        /// complete view lifetime immediately
        /// </summary>
        public void Destroy() => _lifeTimeDefinition.Terminate();
        
        public void BindLayout(IViewLayoutProvider layoutProvider) => _viewLayout = layoutProvider;

        public async UniTask Initialize(IViewModel model, IViewLayoutProvider layoutProvider)
        {
            _isInitialized.Value = false;
            
            BindLayout(layoutProvider);
            await Initialize(model);

            _isInitialized.Value = true;
        }

        public async UniTask Initialize(IViewModel model, bool isViewOwner = false)
        {
            // save current state
            _isViewOwner = isViewOwner;
            
            //restart view lifetime
            _viewModelLifeTime.Release();
            _progressLifeTime.Release();

            //calls one per lifetime
            if (!_isInitialized.Value) {
                InitialSetup();
                OnAwake();
            }

            InitializeHandlers(model);
            
            BindLifeTimeActions(model);
            //custom initialization
            await OnInitialize(model);
        }

        /// <summary>
        /// show active view
        /// </summary>
        public void Show() => StartProgressAction(_progressLifeTime, OnShow);

        /// <summary>
        /// hide view without release it
        /// </summary>
        public void Hide() => StartProgressAction(_progressLifeTime, OnHide);

        /// <summary>
        /// end of view lifetime
        /// </summary>
        public void Close() => StartProgressAction(_progressLifeTime, OnClose);
        
        /// <summary>
        /// bind source stream to view action
        /// with View LifeTime context
        /// </summary>
        public IView BindToView<T>(IObservable<T> source, Action<T> action, int frameThrottle = 0)
        {
            return this.Bind(source, _viewModelLifeTime, action,frameThrottle);
        }

        #endregion public methods

        #region private methods

        public IObservable<IView> SelectStatus(ViewStatus status)
        {
            return _status.
                Where(x => x == status).
                Select(x => this);
        }
        
        /// <summary>
        /// custom initialization methods
        /// </summary>
        protected virtual async UniTask OnInitialize(IViewModel model) { }
        
        /// <summary>
        /// view closing process
        /// windows auto terminated on close complete
        /// </summary>
        /// <returns></returns>
        private IEnumerator OnClose()
        {
            //wait until user defined closing operation complete
            yield return OnCloseProgress(_progressLifeTime);

            _lifeTimeDefinition.Release();
        }
        
        /// <summary>
        /// hide process
        /// </summary>
        private IEnumerator OnHide()
        {
            if(!SetStatus(ViewStatus.Hiding))
                yield break;

            //wait until user defined closing operation complete
            yield return OnHidingProgress(_progressLifeTime);

            SetStatus(ViewStatus.Hidden);
        }
        
        /// <summary>
        /// show process
        /// </summary>
        private IEnumerator OnShow()
        {
            yield return this.WaitForEndOfFrame();

            if(!SetStatus(ViewStatus.Showing))
                yield break;
            
            yield return OnShowProgress(_progressLifeTime);

            SetStatus(ViewStatus.Shown);
        }


        protected virtual bool SetStatus(ViewStatus status)
        {
            if (status == _status.Value)
                return false;

            if (_lifeTimeDefinition.IsTerminated)
            {
                _status.Value = ViewStatus.Closed;
                return false;
            }

            _status.Value = status;
            return true;
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

        private void StartProgressAction(ILifeTime lifeTime,Func<IEnumerator> action)
        {
            if (lifeTime.IsTerminated) 
                return;
            //run animation immediately
            action().Execute(RoutineType.Update,true).AddTo(lifeTime);
        }

        private void InitializeHandlers(IViewModel model)
        {
            ViewModel = model;
            IsTerminated = false;
            
            _isVisible        = _visibility.Value;

            _status.Where(x => x == ViewStatus.Hidden || x == ViewStatus.Closed)
                .Do(x => _visibility.Value = false)
                .Subscribe()
                .AddTo(LifeTime);

            _status.Where(x => x == ViewStatus.Shown || x == ViewStatus.Showing)
                .Do(x => _visibility.Value = true)
                .Subscribe()
                .AddTo(LifeTime);

            _visibility.
                Subscribe(x => _isVisible = x).
                AddTo(_lifeTimeDefinition);
        }
        
        private void BindLifeTimeActions(IViewModel model)
        {
            //bind model lifetime to local
            var modelLifeTime = model.LifeTime;
            if (model.IsDisposeWithModel)
            {
                modelLifeTime.ComposeCleanUp(_viewModelLifeTime, () =>
                {
                    if (Equals(ViewModel, model))
                        Close();
                });
            }

            _viewModelLifeTime.AddCleanUpAction(() =>
            {
                if (_isViewOwner)
                    ViewModel.Cancel();
            });
            
            _viewModelLifeTime.AddCleanUpAction(_progressLifeTime.Terminate);
        }

        private void InitialSetup()
        {
            _isInitialized.Value = true;
            _status.Value  = ViewStatus.None;
            
            _viewModelLifeTime.AddTo(LifeTime);
            _progressLifeTime.AddTo(LifeTime);
            
            LifeTime.AddCleanUpAction(() => 
            {
                _isInitialized.Value = false;
                IsTerminated   = true;
                ViewModel      = null;
                _status.SetValueForce(ViewStatus.Closed);
                _status.Release();
                _visibility.Release();
            });
        }

        protected override void OnDisable()
        {
            _progressLifeTime.Release();
            base.OnDisable();
        }

        protected sealed override void OnDestroy()
        {
            _lifeTimeDefinition.Terminate();
            
            base.OnDestroy();
            GameLog.LogFormat("View {0} Destroyed",name);
        }

        protected virtual void OnAwake()
        {
            
        }

        #endregion
    }
}