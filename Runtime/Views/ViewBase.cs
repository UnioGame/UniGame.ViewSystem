using UniGame.Runtime.Extension;

namespace UniGame.UiSystem.Runtime
{
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using JetBrains.Annotations;
    using UniCore.Runtime.ProfilerTools;
    using UniCore.Runtime.Attributes;
    using UniGame.Runtime.DataFlow;
    using UniGame.Runtime.Rx;
    using Core.Runtime;
    using R3;
    using ViewSystem.Runtime.Animations;
    using ViewSystem.Runtime.Views.Abstract;
    using ViewSystem.Runtime;
     
    using UnityEngine;
    using ViewSystem.Runtime.Binding;

#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif
    
    public abstract class ViewBase : 
        MonoBehaviour,
        ILayoutView
    {
        public const string NullViewName = "Null";
        public const string RuntimeInfo = "runtime info";
        
        private static IViewBinderProcessor ViewBinderProcessor = new ViewBinderProcessor();
        
        #region inspector

#if ODIN_INSPECTOR
        [FoldoutGroup("animation")]
        [InlineProperty]
        [HideLabel]
#endif
        [SerializeReference]
        public IViewAnimation viewAnimation = new SimpleFadeViewAnimation();
        
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
        private BoolReactiveValue _isInitialized = new();

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

#if ODIN_INSPECTOR
        [FoldoutGroup("settings")]
#endif
        public string sourceName;
#if ODIN_INSPECTOR
        [FoldoutGroup("settings")]
#endif
        public string viewId;
#if ODIN_INSPECTOR
        [FoldoutGroup("settings")]
#endif
        public int viewIdHash;
        
        /// <summary>
        /// if value enabled, then model will not be updated when changed
        /// OnModelChanged not be called
        /// </summary>
        public bool enableModelUpdate = false;
        
        #endregion

        #region private fields

        private IViewAnimation _monoAnimation;
        private RectTransform _rectTransform;
        private Transform _transform;
        private ReactiveValue<bool> _isModelChanged = new();

        private readonly LifeTime _lifeTimeDefinition = new();
        private readonly LifeTime _progressLifeTime   = new();
        private readonly LifeTime _viewModelLifeTime   = new();
        private readonly Subject<IViewModel> _viewModelChanged = new();
        
        /// <summary>
        /// ui element visibility status
        /// </summary>
        private readonly BoolReactiveValue _visibility = new(false);

        /// <summary>
        /// view statuses reactions
        /// </summary>
        private readonly ReactiveValue<ViewStatus> _status = new(ViewStatus.None);

        private IViewsLayout _viewLayout;

        private bool _isViewOwner;
        private bool _isModelAttached;
        
        protected bool IsCommandsAction => Application.isPlaying;
        
        #endregion
        
        #region public properties

        public string SourceName => sourceName;
        
        public string ViewId => viewId;

        public int ViewIdHash => viewIdHash;
        
        public ReadOnlyReactiveProperty<bool> IsInitialized => _isInitialized;

        public GameObject Owner => gameObject;
        
        public GameObject GameObject => gameObject;

        public virtual Type ModelType => typeof(IViewModel);

        /// <summary>
        /// View LifeTime
        /// </summary>
        public virtual ILifeTime LifeTime => _viewModelLifeTime;

        public ILifeTime ModelLifeTime => _viewModelLifeTime;
        
        public ILifeTime ViewLifeTime  => _lifeTimeDefinition;

        public IViewAnimation Animation { get; set; }
        
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
        public virtual IViewsLayout Layout => _viewLayout ?? GameViewSystem.ViewSystem;

        public ReadOnlyReactiveProperty<ViewStatus> Status => _status;

        /// <summary>
        /// Is View Active
        /// </summary>
        public ReadOnlyReactiveProperty<bool> IsVisible => _visibility;

        public bool IsTerminated => _lifeTimeDefinition.IsTerminated;

        public bool IsModelAttached => _isModelAttached;

        public IViewModel ViewModel { get; private set; }
        
        public Observable<IViewModel> OnViewModelChanged => _viewModelChanged;

        #endregion public properties

        #region public methods

        /// <summary>
        /// complete view lifetime immediately
        /// </summary>
        public void Destroy()
        {
            SetInternalStatus(ViewStatus.Closed);
            SetStatus(ViewStatus.Closed);
            _viewModelLifeTime.Terminate();
            _lifeTimeDefinition.Terminate();
        }

        public void BindLayout(IViewsLayout layoutProvider)
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

        public async UniTask<IView> Initialize(IViewModel model, IViewsLayout layoutProvider)
        {
            BindLayout(layoutProvider);
            await Initialize(model);
            BindLayout(layoutProvider);

            return this;
        }

        public void SetSourceName(string id,string source)
        {
            viewId = id;
            viewIdHash = viewId.GetHashCode();
            sourceName = source;
        }

        public async UniTask<IView> Initialize(IViewModel model, bool ownViewModel = false)
        {
            if (gameObject == null) return this;
            
            // save current state
            _isViewOwner = ownViewModel;
            //restart view lifetime
            _viewModelLifeTime.Restart();
            _progressLifeTime.Restart();
            
            _isModelAttached = true;
            
            //calls one per lifetime
            if (!_isInitialized.Value) {
                InitialSetup();
            }
            
            InitializeHandlers(model);
            BindLifeTimeActions(model);

            Animation = SelectAnimation();
            
            PlayAnimation(_progressLifeTime)
                .Forget();

            //custom initialization
            await OnInitialize(model);

            ViewBinderProcessor.Bind(this, model);
            
            _isInitialized.Value = true;
            _viewModelChanged.OnNext(model);

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
            ShowAsync().Forget();
            return this;
        }
        
        public async UniTask<IView> ShowAsync()
        {
            if(!SetInternalStatus(ViewStatus.Shown))
                return this;
            
            await StartProgressAction(_progressLifeTime, OnShow)
                .AttachExternalCancellation(_progressLifeTime.Token);
            
            return this;
        }

        /// <summary>
        /// hide view without release it
        /// </summary>
#if ODIN_INSPECTOR
        [Button]
        [ShowIf(nameof(IsCommandsAction))]
#endif
        private UniTask _hideTask;
        public void Hide()
        {
            if(!SetInternalStatus(ViewStatus.Hidden))
                return;
            
            if(!SetStatus(ViewStatus.Hiding))
                return;

            if(_hideTask.Status == UniTaskStatus.Pending) return;
            
            _hideTask = StartProgressAction(_progressLifeTime, OnHide)
                .AttachExternalCancellation(_progressLifeTime.Token);
            
            _hideTask.Forget();

        }

        /// <summary>
        /// end of view lifetime
        /// </summary>
#if ODIN_INSPECTOR
        [Button]
        [ShowIf(nameof(IsCommandsAction))]
#endif
        private UniTask _closeTask;
        
        public void Close()
        {
            CloseAsync().Forget();
        }
        
        public async UniTask CloseAsync()
        {
            //if(!SetInternalStatus(ViewStatus.Closed)) return;
            if (_status.Value == ViewStatus.Hidden)
            {
                Destroy();
                return;
            }

            if (_closeTask.Status == UniTaskStatus.Pending)
            {
                await UniTask.WaitWhile(_closeTask,x => x.Status == UniTaskStatus.Pending);
                return;
            }
            
            _closeTask =  StartProgressAction(_progressLifeTime, OnClose, Destroy)
                .Preserve()
                .AttachExternalCancellation(_progressLifeTime.Token);
            
            await _closeTask;
        }
        
        public Observable<IView> SelectStatus(ViewStatus status)
        {
            return _status.
                Where(x => x == status).
                Select(x => this as IView);
        }

        public bool IsActive()
        {
            return isActiveAndEnabled && 
                   _status.Value is ViewStatus.Showing or ViewStatus.Shown;
        }

        #endregion public methods

        #region private methods

        protected virtual IViewAnimation SelectAnimation()
        {   
            _monoAnimation = gameObject.GetComponent<IViewAnimation>();
            return _monoAnimation ?? viewAnimation;
        }

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
        private async UniTask OnClose()
        {
            SetStatus(ViewStatus.Hiding);
            
            await OnCloseProgress(_progressLifeTime);
        }
        
        /// <summary>
        /// hide process
        /// </summary>
        private async UniTask OnHide()
        {
            SetStatus(ViewStatus.Hiding);
            
            //wait until user defined closing operation complete
            await OnHidingProgress(_progressLifeTime);

            SetStatus(ViewStatus.Hidden);
        }
        
        /// <summary>
        /// show process
        /// </summary>
        private async UniTask OnShow()
        {
            if (!SetStatus(ViewStatus.Showing))
                return;

            await OnShowProgress(_progressLifeTime);

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
                    _visibility.Value = true;
                    break;
                case ViewStatus.Shown:
                    _visibility.Value = true;
                    break;
            }

            _status.Value = status;

            return true;
        }

        public async UniTask PlayAnimation(ILifeTime animationLifeTime)
        {
            if (Animation == null) return;
            await Animation.PlayAnimation(this, _status.Value, animationLifeTime);
        }

        /// <summary>
        /// close continuation
        /// use hiding progress by default
        /// </summary>
        protected virtual async UniTask OnCloseProgress(ILifeTime progressLifeTime)
        {
            await PlayAnimation(progressLifeTime);
        }

        /// <summary>
        /// showing continuation
        /// </summary>
        protected virtual async UniTask OnShowProgress(ILifeTime progressLifeTime)
        {
            await PlayAnimation(progressLifeTime);
        }
        
        /// <summary>
        /// hiding continuation
        /// </summary>
        protected virtual async UniTask OnHidingProgress(ILifeTime progressLifeTime)
        {
            await PlayAnimation(progressLifeTime);
        }

        private bool SetInternalStatus(ViewStatus internalStatus)
        {
            if (this == null) return false;
            if (_internalViewStatus == ViewStatus.Closed) return false;
            if(_internalViewStatus == internalStatus) return false;

            _internalViewStatus = internalStatus;

            return true;
        }

        private async UniTask StartProgressAction(
            LifeTime lifeTime,
            Func<UniTask> action,
            Action finallyAction = null)
        {
            if (lifeTime.IsTerminated) 
                return;
            
            lifeTime.Release();

            try
            {
                if (action == null) return;
                //run animation
                var actionTask = action.Invoke();
                await actionTask.AttachExternalCancellation(lifeTime.Token);
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

            _viewModelLifeTime.AddCleanUpAction(() =>
            {
                if (_isViewOwner) 
                    ViewModel.Cancel();
                ViewModel = null;
            });
            
            _viewModelLifeTime.AddCleanUpAction(_progressLifeTime.Release);
        
            ModelLifeTime.AddCleanUpAction(() => _isModelAttached = false);

            if (enableModelUpdate)
            {
                OnEndOfFrameCheck(_viewModelLifeTime.Token)
                    .Forget();
            }
            
#if UNITY_EDITOR
            _status.Subscribe(x => _editorViewStatus = x)
                .AddTo(ViewLifeTime);
#endif
        }

        private async UniTask OnEndOfFrameCheck(CancellationToken token)
        {
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
            _lifeTimeDefinition.Restart();
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
            
            _visibility.Dispose();
        }

        protected void OnDisable() => _progressLifeTime.Release();

        protected void OnDestroy()
        {
            Destroy();
            _visibility.Dispose();
        }

        protected void Awake()
        {
            _status.Subscribe(OnViewStatus);
            OnAwake();
        }

        protected void OnEnable()
        {
            var animation = SelectAnimation();
            animation?.PlayAnimation(this, _status.Value, _progressLifeTime);
        }

        protected void OnValidate() => OnViewValidate();

        protected virtual void OnViewEnable(){ }
        
        protected virtual void OnAwake() { }
        
        protected virtual void OnViewValidate() {}

        protected virtual void OnViewStatus(ViewStatus status) { }

        #endregion
    }
}