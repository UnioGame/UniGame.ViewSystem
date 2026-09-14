namespace UniGame.UiSystem.Runtime.UiToolkit
{
    using UnityEngine;
    using UnityEngine.UIElements;

    /// <summary>
    /// Applies one ViewSystem stack position to either presentation type. Each
    /// UIDocument receives a runtime PanelSettings instance, while the configured
    /// source asset is restored when the adapter is released.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class UiToolkitMixedOrderAdapter : MonoBehaviour
    {
        [SerializeField] private int orderStep = 10;

        private UIDocument _document;
        private PanelSettings _sourcePanel;
        private PanelSettings _runtimePanel;
        private Canvas _canvas;
        private int _sourceCanvasOrder;
        private bool _sourceOverrideSorting;

        public int CurrentStackIndex { get; private set; } = -1;

        public void Apply(int stackIndex, bool acceptsInput)
        {
            CurrentStackIndex = stackIndex;
            var sortingOrder = stackIndex * Mathf.Max(1, orderStep);
            ApplyToolkitOrder(sortingOrder, acceptsInput);
            ApplyCanvasOrder(sortingOrder, acceptsInput);
        }

        public void Release()
        {
            if (_document != null && _runtimePanel != null && _document.panelSettings == _runtimePanel)
                _document.panelSettings = _sourcePanel;
            if (_runtimePanel != null)
                Destroy(_runtimePanel);
            _runtimePanel = null;

            if (_canvas != null)
            {
                _canvas.overrideSorting = _sourceOverrideSorting;
                _canvas.sortingOrder = _sourceCanvasOrder;
                var group = _canvas.GetComponent<CanvasGroup>();
                if (group != null) { group.interactable = true; group.blocksRaycasts = true; }
            }
            CurrentStackIndex = -1;
        }

        private void ApplyToolkitOrder(int sortingOrder, bool acceptsInput)
        {
            _document ??= GetComponent<UIDocument>();
            if (_document == null) return;
            if (_runtimePanel == null)
            {
                _sourcePanel = _document.panelSettings;
                if (_sourcePanel == null)
                    throw new System.InvalidOperationException($"Mixed UI adapter on '{name}' requires PanelSettings.");
                _runtimePanel = Instantiate(_sourcePanel);
                _runtimePanel.name = $"{_sourcePanel.name} ({name} runtime)";
                _document.panelSettings = _runtimePanel;
            }
            _runtimePanel.sortingOrder = sortingOrder;
            var root = _document.rootVisualElement;
            if (root != null)
                root.pickingMode = acceptsInput ? PickingMode.Position : PickingMode.Ignore;
        }

        private void ApplyCanvasOrder(int sortingOrder, bool acceptsInput)
        {
            if (_canvas == null)
            {
                _canvas = GetComponent<Canvas>();
                if (_canvas == null) return;
                _sourceCanvasOrder = _canvas.sortingOrder;
                _sourceOverrideSorting = _canvas.overrideSorting;
            }
            _canvas.overrideSorting = true;
            _canvas.sortingOrder = sortingOrder;
            var group = _canvas.GetComponent<CanvasGroup>() ?? _canvas.gameObject.AddComponent<CanvasGroup>();
            group.interactable = acceptsInput;
            group.blocksRaycasts = acceptsInput;
        }

        private void OnDisable() => Release();
        private void OnDestroy() => Release();
    }
}
