namespace UniGame.UiSystem.Runtime.UiToolkit
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using UniGame.ViewSystem.Runtime;
    using UniGame.ViewSystem.Runtime.Views.Abstract;
    using UnityEngine;
    using UnityEngine.UIElements;
#if UNITY_6000_5_OR_NEWER
    using PanelHost = UnityEngine.UIElements.PanelRenderer;
#else
    using PanelHost = UnityEngine.UIElements.UIDocument;
#endif

    /// <summary>Exposes a non-generic UI Toolkit host to ViewSystem adapters and bindings.</summary>
    public interface IUiToolkitView
    {
        bool AcceptInput { get; set; }
#if UNITY_6000_5_OR_NEWER
        PanelRenderer PanelRenderer { get; }
#else
        UIDocument UiDocument { get; }
        void RefreshHost();
#endif
        VisualElement Root { get; }
        void AddUnbind(Action unbind);
        void SetVisible(bool visible);
        void SetStackInputAllowed(bool allowed);
        void PrepareForPanelReload();
        void FocusFirstFocusable();
    }

    /// <summary>Hosts a UI Toolkit tree and binds it to a ViewSystem model.</summary>
    public abstract class UiToolkitView<TViewModel> : View<TViewModel>, IUiToolkitView
        where TViewModel : class, IViewModel
    {
        public const string ContentClassName = "view-system-content";
        public const string HiddenClassName = "view-system-hidden";

        public bool acceptInput = true;

        private readonly UiToolkitViewAnimation _toolkitAnimation = new();
        private readonly List<Action> _unbindActions = new();
        private readonly Dictionary<VisualElement, PickingMode> _sourcePickingModes = new();
        private PanelHost _panelHost;
        private VisualElement _root;
        private VisualElement _content;
        private VisualElement _boundRoot;
        private VisualElement _pickingRoot;
        private bool _suspended;
        private bool _requestedVisible;
        private bool _stackAllowsInput = true;
        private bool _initializing;
#if UNITY_6000_5_OR_NEWER
        private int _lastPanelVersion = -1;
#endif

        public bool AcceptInput
        {
            get => acceptInput;
            set
            {
                acceptInput = value;
                ApplyVisibility();
            }
        }
        private PanelHost Host => _panelHost != null ? _panelHost : _panelHost = GetComponent<PanelHost>();
#if UNITY_6000_5_OR_NEWER
        public PanelRenderer PanelRenderer => Host;
#else
        public UIDocument UiDocument => Host;
#endif
        public VisualElement Root => _root;
        public bool IsToolkitReady => !_suspended && _root != null && _content != null;
        public bool HasActiveHost => Host != null && Host.enabled && Host.gameObject.activeInHierarchy;
#if UNITY_6000_5_OR_NEWER
        private bool IsNestedHost => false;
#else
        private bool IsNestedHost => Host != null && Host.parentUI != null;
#endif

        protected override IViewAnimation SelectAnimation() => _toolkitAnimation;

        protected override async UniTask OnInitialize(TViewModel model)
        {
            LifeTime.AddCleanUpAction(ClearBindings);
            LifeTime.AddCleanUpAction(DetachHost);
            await base.OnInitialize(model);

            _initializing = true;
            try
            {
                ClearBindings();
                _requestedVisible = false;
                if (!gameObject.activeInHierarchy || !HasActiveHost)
                    return;

                AttachHost();
                RefreshTree();
                if (!IsToolkitReady)
                    await UniTask.WaitUntil(() => IsToolkitReady).AttachExternalCancellation(LifeTime.Token);
                await BindCurrentTree(model);
            }
            finally
            {
                _initializing = false;
            }
        }

        /// <summary>Bind the current model to the current UI Toolkit tree.</summary>
        protected virtual UniTask OnInitializeToolkit(TViewModel model, VisualElement root) => UniTask.CompletedTask;

        public T Q<T>(string elementName) where T : VisualElement
        {
            var element = Root?.Q<T>(elementName);
            if (element == null)
                throw Error($"required element '{elementName}' ({typeof(T).Name}) was not found");
            return element;
        }

        public bool TryQ<T>(string elementName, out T element) where T : VisualElement
        {
            element = Root?.Q<T>(elementName);
            return element != null;
        }

        public void AddUnbind(Action unbind)
        {
            if (unbind != null) _unbindActions.Add(unbind);
        }

        public void ClearBindings()
        {
            for (var index = _unbindActions.Count - 1; index >= 0; index--)
            {
                try { _unbindActions[index]?.Invoke(); }
                catch (Exception exception) { Debug.LogException(exception, this); }
            }
            _unbindActions.Clear();
            _boundRoot = null;
        }

        public void PrepareForPanelReload()
        {
            ClearBindings();
            RestorePickingModes();
            _root = null;
            _content = null;
#if UNITY_6000_5_OR_NEWER
            _lastPanelVersion = -1;
#endif
        }

#if !UNITY_6000_5_OR_NEWER
        public void RefreshHost()
        {
            if (!HasActiveHost)
            {
                if (_root != null)
                    PrepareForPanelReload();
                return;
            }

            var root = Host.rootVisualElement;
            if (root == null || root.childCount == 0 || root.panel == null)
            {
                if (_root != null)
                    PrepareForPanelReload();
                return;
            }

            if (ReferenceEquals(_root, root) && _content != null && _content.panel != null)
                return;

            PrepareForPanelReload();
            SetRoot(root);
        }

        private void LateUpdate() => RefreshHost();
#endif

        public void SetVisible(bool visible)
        {
            _requestedVisible = visible;
            ApplyVisibility();
        }

        public void SetStackInputAllowed(bool allowed)
        {
            _stackAllowsInput = allowed;
            ApplyVisibility();
        }

        public void FocusFirstFocusable()
        {
            if (acceptInput)
                Root?.Query<VisualElement>().Where(element => element.focusable && element.enabledInHierarchy).First()?.Focus();
        }

        protected override void OnViewEnable()
        {
            base.OnViewEnable();
            _suspended = false;
            AttachHost();
            if (Model == null)
                return;
            RefreshTree();
            SetVisible(Status.CurrentValue is ViewStatus.Showing or ViewStatus.Shown);
            if (IsToolkitReady && _boundRoot != Root)
                BindCurrentTree(Model).Forget();
        }

        protected override void OnViewDisable()
        {
            _suspended = true;
            ApplyVisibility();
            Root?.Blur();
            Root?.ReleasePointer(0);
            DetachHost();
            ClearBindings();
#if UNITY_6000_5_OR_NEWER
            _lastPanelVersion = -1;
#endif
            _stackAllowsInput = true;
            base.OnViewDisable();
        }

        protected override void OnViewStatus(ViewStatus status)
        {
            base.OnViewStatus(status);
            SetVisible(status is ViewStatus.Showing or ViewStatus.Shown);
            if (status == ViewStatus.Closed)
                ClearBindings();
        }

        private async UniTask BindCurrentTree(TViewModel model)
        {
            var root = Root;
            if (model == null || root == null || !isActiveAndEnabled || ReferenceEquals(_boundRoot, root))
                return;
            ClearBindings();
            _boundRoot = root;
            await OnInitializeToolkit(model, root);
        }

        private void RefreshTree()
        {
            if (Host == null)
                throw Error("UI Toolkit host component is missing");
            if (Host.panelSettings == null && !IsNestedHost)
                throw Error("UI Toolkit host PanelSettings is missing");
            if (Host.visualTreeAsset == null)
                throw Error("UI Toolkit host VisualTreeAsset is missing");
#if !UNITY_6000_5_OR_NEWER
            RefreshHost();
#endif
            ApplyVisibility();
        }

        private void AttachHost()
        {
            var host = Host;
            if (host == null || !host.enabled || !host.gameObject.activeInHierarchy)
                return;
#if UNITY_6000_5_OR_NEWER
            host.UnregisterUIReloadCallback(OnPanelRendererReload);
            host.RegisterUIReloadCallback(OnPanelRendererReload);
#else
            RefreshHost();
#endif
        }

        private void DetachHost()
        {
#if UNITY_6000_5_OR_NEWER
            if (Host != null)
                Host.UnregisterUIReloadCallback(OnPanelRendererReload);
#endif
        }

#if UNITY_6000_5_OR_NEWER
        private void OnPanelRendererReload(PanelRenderer renderer, VisualElement root, int version)
        {
            if (version == _lastPanelVersion && ReferenceEquals(root, _root))
                return;
            _lastPanelVersion = version;
            SetRoot(root);
        }
#endif

        private void SetRoot(VisualElement root)
        {
            if (root == null)
                return;
            var content = root.Q<VisualElement>(className: ContentClassName);
            if (content == null || ReferenceEquals(content, root))
                throw Error($"UXML content element with class '{ContentClassName}' is missing");

            if (!ReferenceEquals(_pickingRoot, root) ||
                (ReferenceEquals(_root, root) && !ReferenceEquals(_content, content)))
            {
                RestorePickingModes();
                _pickingRoot = root;
            }
            var changed = !ReferenceEquals(_root, root) || !ReferenceEquals(_content, content);
            if (changed)
            {
                ClearBindings();
                _root = root;
                _content = content;
            }
            _suspended = !isActiveAndEnabled;
            ApplyVisibility();
            if (changed && !_initializing && Model != null && isActiveAndEnabled)
                BindCurrentTree(Model).Forget();
        }

        private void ApplyVisibility()
        {
            var active = _requestedVisible && !_suspended;
            if (_content != null)
                _content.EnableInClassList(HiddenClassName, !active);
            if (_root != null)
                ApplyPickingMode(_root, active && acceptInput && _stackAllowsInput);
        }

        private void ApplyPickingMode(VisualElement element, bool acceptsInput)
        {
            if (!_sourcePickingModes.TryGetValue(element, out var sourceMode))
            {
                sourceMode = element.pickingMode;
                _sourcePickingModes.Add(element, sourceMode);
            }
            element.pickingMode = acceptsInput ? sourceMode : PickingMode.Ignore;
            for (var index = 0; index < element.hierarchy.childCount; index++)
                ApplyPickingMode(element.hierarchy.ElementAt(index), acceptsInput);
        }

        private void RestorePickingModes()
        {
            foreach (var source in _sourcePickingModes)
            {
                if (source.Key != null)
                    source.Key.pickingMode = source.Value;
            }
            _sourcePickingModes.Clear();
            _pickingRoot = null;
        }

        private InvalidOperationException Error(string reason) =>
            new($"UITK view '{ViewId}' on '{name}': {reason}.");
    }
}
