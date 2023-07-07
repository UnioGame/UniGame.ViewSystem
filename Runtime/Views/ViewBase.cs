using UniModules.UniCore.Runtime.Extension;

namespace UniGame.UiSystem.Runtime
{
    using System;
    using System.Collections;
    using Cysharp.Threading.Tasks;
    using JetBrains.Annotations;
    using UniCore.Runtime.ProfilerTools;
    using UniCore.Runtime.Attributes;
    using UniModules.UniCore.Runtime.DataFlow;
    using UniModules.UniGame.Core.Runtime.Rx;
    using UniModules.UniRoutine.Runtime;
    using Core.Runtime;
    using UniModules.UniGame.UISystem.Runtime;
    using ViewSystem.Runtime;
    using UniRx;
    using UniModules.UniRoutine.Runtime.Extension;
    using UnityEngine;

#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif
    
    public abstract class ViewBase : 
        MonoBehaviour,
        ILayoutView
    {
        private const string NullViewName = "Null";
        private const string RuntimeInfo = "runtime info";
        
        #region inspector

#if ODIN_INSPECTOR
        [ShowIf(nameof(IsCommandsAction))]
        [BoxGroup(RuntimeInfo)]
#endif
        [ReadOnlyValue]
        [SerializeField]
        [UsedImplicitly]
        private bool _isVisible;

        [HideInInspector]
        [ReadOnlyValue]
        [SerializeField]
        private BoolRecycleReactiveProperty _isInitialized = new BoolRecycleReactiveProperty();

#if ODIN_INSPECTOR
        [ShowIf(nameof(IsCommandsAction))]
        [BoxGroup(RuntimeInfo)]
#endif
        [ReadOnlyValue]
        [SerializeField]
        private ViewStatus _editorViewStatus = ViewStatus.None;

        [HideInInspector]
        [ReadOnlyValue]
        [SerializeField]
        private ViewStatus _internalViewStatus = ViewStatus.None;
        
        [HideInInspector]
        [SerializeField]
        public string skinTag = string.Empty;

        public string sourceName;
        
        /// <summary>
        /// if value enabled, then model will not be updated when changed
        /// OnModelChanged not be called
        /// </summary>
        public bool disableModelUpdate = false;
        
        #endregion

        private RectTransform _rectTransform;
        private Transform _transform;
        private RecycleReactiveProperty<bool> _isModelChanged = new RecycleReactiveProperty<bool>();

        private readonly LifeTimeDefinition _lifeTimeDefinition = new LifeTimeDefinition();
        private readonly LifeTimeDefinition _progressLifeTime   = new LifeTimeDefinition();
        private readonly LifeTimeDefinition _viewModelLifeTime   = new LifeTimeDefinition();
        
        /// <summary>
        /// ui element visibility status
        /// </summary>
        private readonly BoolRecycleReactiveProperty _visibility = new BoolRecycleReactiveProperty(false);

        /// <summary>
        /// view statuses reactions
        /// </summary>
        private readonly RecycleReactiveProperty<ViewStatus> _status = new RecycleReactiveProperty<ViewStatus>();

        private IViewLayoutProvider _viewLayout;

        private bool _isViewOwner;
        private bool _isModelAttached;
        
        protected bool IsCommandsAction => Application.isPlaying;
        
        #region public properties

        public IReadOnlyReactiveProperty<bool> IsInitialized => _isInitialized;

        public GameObject Owner => gameObject;
        
        public GameObject GameObject => gameObject;

        /// <summary>
        /// View LifeTime
        /// </summary>
        public virtual ILifeTime LifeTime => _viewModelLifeTime;

        public ILifeTime ModelLifeTime => _viewModelLifeTime;
        
        public ILifeTime ViewLifeTime  => _lifeTimeDefinition;

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

        /// <summary>
        /// Is View Active
        /// </summary>
        public IReadOnlyReactiveProperty<bool> IsVisible => _visibility;

        public string SourceName => sourceName;
        
        public bool IsTerminated => _lifeTimeDefinition.IsTerminated;

        public bool IsModelAttached => _isModelAttached;

        public IViewModel ViewModel { get; private set; }

        #endregion public properties

        #region public methods

        /// <summary>
        /// complete view lifetime immediately
        /// </summary>
        public void Destroy()
        {
            _lifeTimeDefinition.Terminate();
            _viewModelLifeTime.Terminate();
        }

        public void BindLayout(IViewLayoutProvider layoutProvider)
        {
            _viewLayout = layoutProvider;
        }
        
        public void NotifyModelChanged() => _isModelChanged.Value = true;

        public async UniTask<IView> BindNested(ILayoutView view, IViewModel model)
        {
            if (view == null) return this;
            
            view.BindLayout(_viewLayout);
            await view.Initialize(model);
            
            return this;
        }

        public async UniTask<IView> Initialize(IViewModel model, IViewLayoutProvider layoutProvider)
        {
            BindLayout(layoutProvider);
            await Initialize(model);
            BindLayout(layoutProvider);

            return this;
        }

        public void SetSourceName(string value)
        {
            sourceName = value;
        }

        public async UniTask<IView> Initialize(IViewModel model, bool ownViewModel = false)
        {
            // save current state
            _isViewOwner = ownViewModel;
            //restart view lifetime
            _viewModelLifeTime.Release();
            _progressLifeTime.Release();
            
            _isModelAttached = true;
            
            //calls one per lifetime
            if (!_isInitialized.Value) {
                InitialSetup();
            }
            
            InitializeHandlers(model);
            BindLifeTimeActions(model);

            //custom initialization
            await OnInitialize(model);

            _isInitialized.Value = true;

            return this;
        }

        /// <summary>
        /// show active view
        /// </summary>
#if ODIN_INSPECTOR
        [Button]
        [ShowIf(nameof(IsCommandsAction))]
#endif
        public IView Show()
        {
            if(!SetInternalStatus(ViewStatus.Shown))
                return this;
            
            StartProgressAction(_progressLifeTime, OnShow);
            return this;
        }

        /// <summary>
        /// hide view without release it
        /// </summary>
#if ODIN_INSPECTOR
        [Button]
        [ShowIf(nameof(IsCommandsAction))]
#endif
        public void Hide()
        {
            if(!SetInternalStatus(ViewStatus.Hidden))
                return;
            
            if(!SetStatus(ViewStatus.Hiding))
                return;

            StartProgressAction(_progressLifeTime, OnHide);
        }

        /// <summary>
        /// end of view lifetime
        /// </summary>
#if ODIN_INSPECTOR
        [Button]
        [ShowIf(nameof(IsCommandsAction))]
#endif
        public void Close()
        {
            if(!SetInternalStatus(ViewStatus.Closed))
                return;

            if (_status.Value == ViewStatus.Hidden)
            {
                Destroy();
                return;
            }
            
            StartProgressAction(_progressLifeTime, OnClose, Destroy);
        }
        
        public IObservable<IView> SelectStatus(ViewStatus status)
        {
            return _status.
                Where(x => x == status).
                Select(x => this);
        }

        public bool IsActive()
        {
            return isActiveAndEnabled && 
                   _status.Value is ViewStatus.Showing or ViewStatus.Shown;
        }

        #endregion public methods

        #region private methods

        protected virtual UniTask OnModelChanged() => UniTask.CompletedTask;
        
        /// <summary>
        /// custom initialization methods
        /// </summary>
        protected virtual UniTask OnInitialize(IViewModel model)
        {
            return UniTask.CompletedTask;
        }
        
        /// <summary>
        /// view closing process
        /// windows auto terminated on close complete
        /// </summary>
        /// <returns></returns>
        private IEnumerator OnClose()
        {
            yield return OnCloseProgress(_progressLifeTime);
        }
        
        /// <summary>
        /// hide process
        /// </summary>
        private IEnumerator OnHide()
        {
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

            if (!SetStatus(ViewStatus.Showing))
                yield break;

            yield return OnShowProgress(_progressLifeTime);

            SetStatus(ViewStatus.Shown);
        }

        protected bool SetStatus(ViewStatus status)
        {
            if (_lifeTimeDefinition.IsTerminated || _internalViewStatus == ViewStatus.Closed)
            {
                _status.Value     = ViewStatus.Closed;
                _visibility.Value = false;
                return false;
            }

            switch (status)
            {
                case ViewStatus.Hidden:
                case ViewStatus.Closed:
                    _visibility.Value = false;
                    break;
                case ViewStatus.Showing:
                case ViewStatus.Shown:
                    _visibility.Value = true;
                    break;
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

        private bool SetInternalStatus(ViewStatus internalStatus)
        {
            var viewName = this != null ? name : NullViewName;

            if (this == null)
                return false;
            
            if (_internalViewStatus == ViewStatus.Closed)
                return false;
            
            if(_internalViewStatus == internalStatus)
            {
                GameLog.LogWarning($"You try to {internalStatus} {viewName} but it has {internalStatus} status yet");
                return false;
            }

            _internalViewStatus = internalStatus;

            return true;
        }

        private async UniTask StartProgressAction(LifeTimeDefinition lifeTime,Func<UniTask> action,Action finallyAction = null,RoutineType routineType = RoutineType.Update)
        {
            if (lifeTime.IsTerminated) 
                return;
            
            lifeTime.Release();

            try
            {
                if (action == null) return;
                //run animation immediately
                var actionTask = action.Invoke();
                await actionTask.AttachExternalCancellation(lifeTime.CancellationToken);
            }
            finally
            {
                finallyAction?.Invoke();
            }
            
        }

        private void InitializeHandlers(IViewModel model)
        {
            ViewModel = model;

            _isVisible = _visibility.Value;
            
            _visibility.
                Subscribe(x => _isVisible = x).
                AddTo(_lifeTimeDefinition);
        }
        
        private void BindLifeTimeActions(IViewModel model)
        {
            _isModelChanged.Value = false;
            //bind model lifetime to local
            var modelLifeTime = model.LifeTime;
            if (model.IsDisposeWithModel)
            {
                modelLifeTime.ComposeCleanUp(_viewModelLifeTime, () =>
                {
                    if (Equals(ViewModel, model))
                        Destroy();
                });
            }

            _viewModelLifeTime.AddCleanUpAction(() =>
            {
                if (_isViewOwner) 
                    ViewModel.Cancel();
                ViewModel = null;
            });
            
            _viewModelLifeTime.AddCleanUpAction(_progressLifeTime.Release);
        
            ModelLifeTime.AddCleanUpAction(() => _isModelAttached = false);
            
            if(!disableModelUpdate) OnEndOfFrameCheck().Forget();
            
#if UNITY_EDITOR
            _status.Subscribe(x => _editorViewStatus = x).AddTo(ViewLifeTime);
#endif
        }

        private async UniTask OnEndOfFrameCheck()
        {
            var token = _viewModelLifeTime.CancellationToken;

            while (!token.IsCancellationRequested)
            {
                await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate,token);
                
                if (_isModelChanged.Value)
                {
                    await OnModelChanged();
                }
                
                _isModelChanged.Value = false;
            }
        }
        
        private void InitialSetup()
        {
            _lifeTimeDefinition.Release();
            _isInitialized.Value = true;
            _status.Value  = ViewStatus.None;
            _internalViewStatus = ViewStatus.None;
            _viewModelLifeTime.AddTo(ViewLifeTime);
            _progressLifeTime.AddTo(ViewLifeTime);
            
            ViewLifeTime.AddCleanUpAction(OnViewDestroy);
        }

        private void OnViewDestroy()
        {
            _isInitialized.Value = false;

            SetInternalStatus(ViewStatus.Closed);
            SetStatus(ViewStatus.Closed);
            
            ViewModel            = null;
            _viewLayout          = null;
            
            _status.Release();
            _visibility.Release();
        }

        protected void OnDisable() => _progressLifeTime.Release();

        protected void OnDestroy() => Destroy();

        protected void Awake() => OnAwake();

        protected virtual void OnAwake() { }

        protected void OnValidate() => OnViewValidate();

        protected virtual void OnViewValidate() {}

        #endregion
    }
}