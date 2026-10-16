namespace UniGame.UiSystem.Runtime.UiToolkit
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Serialization;
    using UnityEngine.UIElements;
#if UNITY_6000_6_OR_NEWER
    using UiToolkitHost = UnityEngine.UIElements.PanelRenderer;
#else
    using UiToolkitHost = UnityEngine.UIElements.UIDocument;
#endif

    /// <summary>
    /// Owns a runtime UI Toolkit tree hosted by a PanelRenderer (Unity 6.6+)
    /// or a UIDocument (earlier Unity 6 versions).
    /// </summary>
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(100)]
    public sealed class UiToolkitViewPresentation : MonoBehaviour
    {
        public const string ContentClassName = "view-system-content";
        public const string HiddenClassName = "view-system-hidden";

        [FormerlySerializedAs("panelRenderer")]
        public UiToolkitHost host;
        public bool acceptInput = true;

        private readonly List<Action> _unbindActions = new();
        private readonly Dictionary<VisualElement, PickingMode> _sourcePickingModes = new();
        private VisualElement _root;
        private VisualElement _content;
        private bool _initialized;
        private bool _suspended;
        private bool _requestedVisible;
        private bool _stackAllowsInput = true;
        private bool _externalAllowsInput = true;
        private int _lastPanelVersion = -1;
        private string _viewId;

        public UiToolkitHost Host => host != null ? host : host = GetComponent<UiToolkitHost>();
#if UNITY_6000_6_OR_NEWER
        public PanelRenderer PanelRenderer => Host;
#else
        public UIDocument Document => Host;
#endif
        public VisualElement Root => _root;
        public bool IsReady => _initialized && _root != null && _content != null;
        public bool HasActiveHost => Host != null && Host.enabled && Host.gameObject.activeInHierarchy;

        public event Action<VisualElement> RootChanged;

        public void Initialize(string viewId = null)
        {
            _viewId = string.IsNullOrWhiteSpace(viewId) ? name : viewId;
            ClearBindings();
            _requestedVisible = false;
            AttachHost();
            RefreshTree(_viewId);
            ApplyVisibility();
        }

        public void RefreshTree(string viewId = null)
        {
            if (!string.IsNullOrWhiteSpace(viewId)) _viewId = viewId;
            if (Host == null)
                throw Error($"{nameof(UiToolkitHost)} component is missing");
            if (Host.panelSettings == null)
                throw Error($"{nameof(UiToolkitHost)}.panelSettings is missing");
            if (Host.visualTreeAsset == null)
                throw Error($"{nameof(UiToolkitHost)}.visualTreeAsset is missing");
#if !UNITY_6000_6_OR_NEWER
            SyncDocumentRoot();
#endif
            if (_root != null) ApplyVisibility();
        }

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
        }

        /// <summary>Discards elements before the host replaces its panel resources.</summary>
        public void PrepareForPanelReload()
        {
            ClearBindings();
            _root = null;
            _content = null;
            _sourcePickingModes.Clear();
            _initialized = false;
            _lastPanelVersion = -1;
        }

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

        /// <summary>
        /// Input gate owned by the game (e.g. modality across layouts). Combined with the stack gate:
        /// the tree is pickable only when both allow input.
        /// </summary>
        public void SetExternalInputAllowed(bool allowed)
        {
            if (_externalAllowsInput == allowed) return;
            _externalAllowsInput = allowed;
            ApplyVisibility();
        }

        public void Suspend()
        {
            _suspended = true;
            ApplyVisibility();
            ClearBindings();
            Root?.Blur();
            Root?.ReleasePointer(0);
        }

        public void Resume()
        {
            _suspended = false;
            RefreshTree(_viewId);
        }

        public void FocusFirstFocusable()
        {
            if (acceptInput)
                Root?.Query<VisualElement>().Where(element => element.focusable && element.enabledInHierarchy).First()?.Focus();
        }

#if UNITY_6000_6_OR_NEWER
        private void AttachHost()
        {
            var renderer = Host;
            if (renderer == null || !renderer.enabled || !renderer.gameObject.activeInHierarchy)
                return;

            renderer.UnregisterUIReloadCallback(OnPanelRendererReload);
            renderer.RegisterUIReloadCallback(OnPanelRendererReload);
        }

        private void DetachHost()
        {
            var renderer = Host;
            if (renderer != null)
                renderer.UnregisterUIReloadCallback(OnPanelRendererReload);
        }

        private void OnPanelRendererReload(PanelRenderer renderer, VisualElement root, int version)
        {
            if (version == _lastPanelVersion)
                return;

            _lastPanelVersion = version;
            SetRoot(root);
        }
#else
        // UIDocument has no reload callback: it rebuilds its tree on enable and
        // when panelSettings or visualTreeAsset change, so the root is polled.
        private void AttachHost() => SyncDocumentRoot();

        private void DetachHost() { }

        private void SyncDocumentRoot()
        {
            var document = Host;
            if (document == null || !document.isActiveAndEnabled)
                return;

            var root = document.rootVisualElement;
            if (root == null || root.childCount == 0)
                return;
            if (_initialized && ReferenceEquals(root, _root) && _content != null && _content.panel != null)
                return;

            SetRoot(root);
        }

        private void LateUpdate() => SyncDocumentRoot();
#endif

        private void SetRoot(VisualElement root)
        {
            if (root == null)
                return;

            var content = root.Q<VisualElement>(className: ContentClassName);
            if (content == null || ReferenceEquals(content, root))
                throw Error($"UXML content element with class '{ContentClassName}' is missing");

            var changed = !ReferenceEquals(_root, root) || !ReferenceEquals(_content, content);
            if (changed)
            {
                ClearBindings();
                _sourcePickingModes.Clear();
                _root = root;
                _content = content;
            }

            _initialized = true;
            _suspended = false;
            ApplyVisibility();
            if (changed)
                RootChanged?.Invoke(_root);
        }

        private void ApplyVisibility()
        {
            var active = _requestedVisible && !_suspended;
            if (_content != null)
                _content.EnableInClassList(HiddenClassName, !active);
            if (_root != null)
                ApplyPickingMode(_root, active && acceptInput && _stackAllowsInput && _externalAllowsInput);
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

        private InvalidOperationException Error(string reason) =>
            new($"UITK view '{(string.IsNullOrWhiteSpace(_viewId) ? name : _viewId)}': {reason}.");

        private void OnEnable()
        {
            AttachHost();
            if (_initialized)
                RefreshTree(_viewId);
        }

        private void OnDisable()
        {
            DetachHost();
            ClearBindings();
            _root = null;
            _content = null;
            _sourcePickingModes.Clear();
            _initialized = false;
            _suspended = true;
            _stackAllowsInput = true;
            _lastPanelVersion = -1;
        }
    }
}
