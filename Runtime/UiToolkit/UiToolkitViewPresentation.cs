namespace UniGame.UiSystem.Runtime.UiToolkit
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UIElements;

    /// <summary>Owns a runtime UI Toolkit tree hosted by a PanelRenderer.</summary>
    [DisallowMultipleComponent]
    public sealed class UiToolkitViewPresentation : MonoBehaviour
    {
        public const string ContentClassName = "view-system-content";
        public const string HiddenClassName = "view-system-hidden";

        public PanelRenderer panelRenderer;
        public bool acceptInput = true;

        private readonly List<Action> _unbindActions = new();
        private readonly Dictionary<VisualElement, PickingMode> _sourcePickingModes = new();
        private VisualElement _root;
        private VisualElement _content;
        private bool _initialized;
        private bool _suspended;
        private bool _requestedVisible;
        private bool _stackAllowsInput = true;
        private int _lastPanelVersion = -1;
        private string _viewId;

        public PanelRenderer PanelRenderer => panelRenderer != null ? panelRenderer : panelRenderer = GetComponent<PanelRenderer>();
        public VisualElement Root => _root;
        public bool IsReady => _initialized && _root != null && _content != null;
        public bool HasActiveHost => PanelRenderer != null && PanelRenderer.enabled && PanelRenderer.gameObject.activeInHierarchy;

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
            if (PanelRenderer == null)
                throw Error("PanelRenderer component is missing");
            if (PanelRenderer.panelSettings == null)
                throw Error("PanelRenderer.panelSettings is missing");
            if (PanelRenderer.visualTreeAsset == null)
                throw Error("PanelRenderer.visualTreeAsset is missing");
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

        /// <summary>Discards elements before PanelRenderer replaces its panel resources.</summary>
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

        private void AttachHost()
        {
            var renderer = PanelRenderer;
            if (renderer == null || !renderer.enabled || !renderer.gameObject.activeInHierarchy)
                return;

            renderer.UnregisterUIReloadCallback(OnPanelRendererReload);
            renderer.RegisterUIReloadCallback(OnPanelRendererReload);
        }

        private void OnPanelRendererReload(PanelRenderer renderer, VisualElement root, int version)
        {
            if (version == _lastPanelVersion)
                return;

            _lastPanelVersion = version;
            SetRoot(root);
        }

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
            var renderer = PanelRenderer;
            if (renderer != null)
                renderer.UnregisterUIReloadCallback(OnPanelRendererReload);
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
