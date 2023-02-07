﻿namespace UniGame.UiSystem.Runtime
{
    using System;
    using System.Collections;
    using Cysharp.Threading.Tasks;
    using JetBrains.Annotations;
    using Sirenix.OdinInspector;
    using UniCore.Runtime.Attributes;
    using UniCore.Runtime.ProfilerTools;
    using UniGame.Core.Runtime;
    using UniGame.ViewSystem.Runtime;
    using UniModules.UniCore.Runtime.DataFlow;
    using UniModules.UniGame.Core.Runtime.Rx;
    using UniModules.UniGame.UISystem.Runtime;
    using UniModules.UniRoutine.Runtime;
    using UniModules.UniRoutine.Runtime.Extension;
    using UniRx;
    using UnityEngine;
    
    public abstract class View : MonoBehaviour ,IView
    {
        private const string NullViewName = "Null";
        
        #region inspector

        [ReadOnlyValue]
        [SerializeField]
        [UsedImplicitly]
        private bool _isVisible;

        [ReadOnlyValue]
        [SerializeField]
        private BoolRecycleReactiveProperty _isInitialized = new BoolRecycleReactiveProperty();

        [ReadOnlyValue]
        [SerializeField]
        private ViewStatus _editorViewStatus = ViewStatus.None;

        [ReadOnlyValue]
        [SerializeField]
        private ViewStatus _internalViewStatus = ViewStatus.None;
        
        [HideInInspector]
        [SerializeField]
        public string skinTag = string.Empty;
        
        #endregion

        private IViewModel _viewModel;
        private RectTransform _rectTransform;
        private Transform _transform;
        
        private readonly LifeTimeDefinition _lifeTimeDefinition = new LifeTimeDefinition();
        private readonly LifeTimeDefinition _progressLifeTime   = new LifeTimeDefinition();

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
        
        protected bool IsCommandsAction => Application.isPlaying;
        
        #region public properties

        public IReadOnlyReactiveProperty<bool> IsInitialized => _isInitialized;
        
        public GameObject Owner => gameObject;

        /// <summary>
        /// View LifeTime
        /// </summary>
        public virtual ILifeTime LifeTime => _lifeTimeDefinition;

        public ILifeTime ViewLifeTime  => _lifeTimeDefinition;

        /// <summary>
        /// view transform
        /// </summary>
        public RectTransform RectTransform => _rectTransform != null ? 
            _rectTransform:
            _rectTransform = transform as RectTransform;

        public IViewModel ViewModel { get; }
        
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

        public bool IsTerminated => _lifeTimeDefinition.IsTerminated;


        #endregion public properties

        #region public methods

        public virtual async UniTask<IView> Initialize(IViewModel vm, bool ownViewModel = false)
        {
            //restart view lifetime
            _progressLifeTime.Release();

            //calls one per lifetime
            if (!_isInitialized.Value)
                InitialSetup();
            
            _viewModel = vm;
            _isVisible = _visibility.Value;
            _visibility.
                Subscribe(x => _isVisible = x).
                AddTo(_lifeTimeDefinition);
            
#if UNITY_EDITOR
            _status.Subscribe(x => _editorViewStatus = x).AddTo(ViewLifeTime);
#endif

            //custom initialization
            await OnInitialize();
            
            _isInitialized.Value = true;

            return this;
        }
        
        /// <summary>
        /// complete view lifetime immediately
        /// </summary>
        public void Destroy()
        {
            _lifeTimeDefinition.Terminate();
        }

        public void BindLayout(IViewLayoutProvider layoutProvider)
        {
            _viewLayout = layoutProvider;
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
        
        /// <summary>
        /// custom initialization methods
        /// </summary>
        protected virtual UniTask OnInitialize()
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

#if UNITY_EDITOR || UNITY_DEBUG
            if (!this)
            {
                GameLog.LogWarning($"You try to {internalStatus} {viewName} but it has destroy status yet");
                return false;
            }
#endif
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

        private void StartProgressAction(LifeTimeDefinition lifeTime,Func<IEnumerator> action,Action finallyAction = null,RoutineType routineType = RoutineType.Update)
        {
#if UNITY_EDITOR
            if (action == null)
            {
                Debug.LogError($"VIEW {name} {GetType().Name} Progress action is NULL");
                return;
            }
#endif
            
            if (lifeTime.IsTerminated) 
                return;
            lifeTime.Release();
            
            //run animation immediately
            action().Execute(routineType,true)
                .WithFinally(finallyAction)
                .AddTo(lifeTime);
        }

        private void InitialSetup()
        {
            _lifeTimeDefinition.Release();
            _isInitialized.Value = true;
            _status.Value  = ViewStatus.None;
            _internalViewStatus = ViewStatus.None;
            _progressLifeTime.AddTo(ViewLifeTime);
            
            ViewLifeTime.AddCleanUpAction(OnViewDestroy);
        }

        private void OnViewDestroy()
        {
            _isInitialized.Value = false;

            SetInternalStatus(ViewStatus.Closed);
            SetStatus(ViewStatus.Closed);
            
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