namespace UniGame.UiSystem.Runtime.UiToolkit
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UIElements;

    /// <summary>
    /// Owns the runtime UIDocument tree and tree-scoped bindings. It never mutates
    /// the shared PanelSettings asset.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(UIDocument))]
    public sealed class UiToolkitViewPresentation : MonoBehaviour
    {
        [SerializeField] private UIDocument _document;

        private readonly List<Action> _unbindActions = new();
        private VisualElement _root;
        private bool _initialized;
        private bool _suspended;
        private bool _requestedVisible;
        private string _viewId;

        public UIDocument Document => _document != null ? _document : _document = GetComponent<UIDocument>();
        public VisualElement Root => _root;
        public bool IsReady => _initialized && _root != null;

        public void Initialize(string viewId = null)
        {
            _viewId = string.IsNullOrWhiteSpace(viewId) ? name : viewId;
            ClearBindings();
            RefreshTree(_viewId);
            _requestedVisible = false;
            ApplyVisibility();
        }

        public void RefreshTree(string viewId = null)
        {
            if (!string.IsNullOrWhiteSpace(viewId)) _viewId = viewId;
            if (Document == null)
                throw Error("UIDocument component is missing");
            if (Document.visualTreeAsset == null)
                throw Error("UIDocument.visualTreeAsset is missing");
            if (Document.panelSettings == null)
                throw Error("UIDocument.panelSettings is missing");

            var current = Document.rootVisualElement;
            if (current == null)
                throw Error("UIDocument has no runtime root");

            if (_root != null && !ReferenceEquals(_root, current))
                ClearBindings();
            _root = current;
            _initialized = true;
            _suspended = false;
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
            Root?.Query<VisualElement>().Where(element => element.focusable && element.enabledInHierarchy).First()?.Focus();
        }

        private void ApplyVisibility()
        {
            if (_root == null) return;
            var active = _requestedVisible && !_suspended;
            _root.style.display = active ? DisplayStyle.Flex : DisplayStyle.None;
            _root.pickingMode = active ? PickingMode.Position : PickingMode.Ignore;
        }

        private InvalidOperationException Error(string reason) =>
            new($"UITK view '{(string.IsNullOrWhiteSpace(_viewId) ? name : _viewId)}': {reason}.");

        private void OnEnable()
        {
            if (_initialized) RefreshTree(_viewId);
        }

        private void OnDisable()
        {
            ClearBindings();
            _root = null;
            _initialized = false;
            _suspended = true;
        }
    }
}
