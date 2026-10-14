namespace UniGame.UiSystem.Runtime.UiToolkit
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UIElements;

    /// <summary>Owns a runtime UI Toolkit tree hosted by a UIDocument or PanelRenderer.</summary>
    [DisallowMultipleComponent]
    public sealed class UiToolkitViewPresentation : MonoBehaviour
    {
        public UIDocument document;
        public PanelRenderer panelRenderer;
        public bool acceptInput = true;

        private readonly List<Action> _unbindActions = new();
        private VisualElement _root;
        private bool _initialized;
        private bool _suspended;
        private bool _requestedVisible;
        private int _lastPanelVersion = -1;
        private string _viewId;

        public UIDocument Document => document != null ? document : document = GetComponent<UIDocument>();
        public PanelRenderer PanelRenderer => panelRenderer != null ? panelRenderer : panelRenderer = GetComponent<PanelRenderer>();
        public VisualElement Root => _root;
        public bool IsReady => _initialized && _root != null;
        public bool HasActiveHost => Document != null && Document.isActiveAndEnabled ||
                                     PanelRenderer != null && PanelRenderer.enabled && PanelRenderer.gameObject.activeInHierarchy;

        public event Action<VisualElement> RootChanged;

        public void Initialize(string viewId = null)
        {
            _viewId = string.IsNullOrWhiteSpace(viewId) ? name : viewId;
            ClearBindings();
            AttachHost();
            RefreshTree(_viewId);
            _requestedVisible = false;
            ApplyVisibility();
        }

        public void RefreshTree(string viewId = null)
        {
            if (!string.IsNullOrWhiteSpace(viewId)) _viewId = viewId;
            var hostCount = (Document != null ? 1 : 0) + (PanelRenderer != null ? 1 : 0);
            if (hostCount == 0)
                throw Error("UIDocument or PanelRenderer component is missing");
            if (hostCount > 1)
                throw Error("a view prefab must use either UIDocument or PanelRenderer, not both");

            if (Document != null)
            {
                if (Document.visualTreeAsset == null)
                    throw Error("UIDocument.visualTreeAsset is missing");
                if (Document.panelSettings == null)
                    throw Error("UIDocument.panelSettings is missing");
                SetRoot(Document.rootVisualElement);
                if (_root == null)
                    throw Error("UIDocument has no runtime root");
                return;
            }

            if (PanelRenderer.panelSettings == null)
                throw Error("PanelRenderer.panelSettings is missing");
            if (PanelRenderer.visualTreeAsset == null)
                throw Error("PanelRenderer.visualTreeAsset is missing");
            if (_root == null)
                return;
            ApplyVisibility();
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

        public void SetVisible(bool visible)
        {
            _requestedVisible = visible;
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

            var changed = !ReferenceEquals(_root, root);
            if (changed)
            {
                ClearBindings();
                _root = root;
            }

            _initialized = true;
            _suspended = false;
            ApplyVisibility();
            if (changed)
                RootChanged?.Invoke(_root);
        }

        private void ApplyVisibility()
        {
            if (_root == null) return;
            var active = _requestedVisible && !_suspended;
            _root.style.display = active ? DisplayStyle.Flex : DisplayStyle.None;
            _root.pickingMode = active && acceptInput ? PickingMode.Position : PickingMode.Ignore;
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
            _initialized = false;
            _suspended = true;
            _lastPanelVersion = -1;
        }
    }
}
