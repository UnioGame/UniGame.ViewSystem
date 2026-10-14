namespace UniGame.UiSystem.Runtime.UiToolkit
{
    using UnityEngine;
    using UnityEngine.UIElements;

    /// <summary>
    /// Applies one ViewSystem stack position to either presentation type. Each
    /// UIDocument and PanelRenderer receive a runtime PanelSettings instance,
    /// while the configured source asset is restored when the adapter is released.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class UiToolkitMixedOrderAdapter : MonoBehaviour
    {
        [SerializeField] private int orderStep = 10;

        private UIDocument _document;
        private PanelRenderer _panelRenderer;
        private PanelSettings _sourcePanel;
        private PanelSettings _runtimePanel;
        private Canvas _canvas;
        private CanvasGroup _canvasGroup;
        private bool _addedCanvasGroup;
        private bool _sourceCanvasGroupInteractable;
        private bool _sourceCanvasGroupBlocksRaycasts;
        private int _sourceCanvasOrder;
        private bool _sourceOverrideSorting;
        private PickingMode _sourcePickingMode;
        private VisualElement _orderedRoot;
        private bool _hasSourcePickingMode;

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
            if (_panelRenderer != null && _runtimePanel != null && _panelRenderer.panelSettings == _runtimePanel)
                _panelRenderer.panelSettings = _sourcePanel;
            if (_runtimePanel != null)
            {
                if (Application.isPlaying) Destroy(_runtimePanel);
                else DestroyImmediate(_runtimePanel);
            }
            _runtimePanel = null;
            _sourcePanel = null;
            _panelRenderer = null;

            if (_canvas != null)
            {
                _canvas.overrideSorting = _sourceOverrideSorting;
                _canvas.sortingOrder = _sourceCanvasOrder;
            }
            if (_canvasGroup != null)
            {
                _canvasGroup.interactable = _sourceCanvasGroupInteractable;
                _canvasGroup.blocksRaycasts = _sourceCanvasGroupBlocksRaycasts;
                if (_addedCanvasGroup)
                {
                    if (Application.isPlaying) Destroy(_canvasGroup);
                    else DestroyImmediate(_canvasGroup);
                }
            }
            _canvas = null;
            _canvasGroup = null;
            _addedCanvasGroup = false;
            if (_orderedRoot != null && _hasSourcePickingMode)
                _orderedRoot.pickingMode = _sourcePickingMode;
            _orderedRoot = null;
            _hasSourcePickingMode = false;
            CurrentStackIndex = -1;
        }

        private void ApplyToolkitOrder(int sortingOrder, bool acceptsInput)
        {
            _document ??= GetComponent<UIDocument>();
            _panelRenderer ??= GetComponent<PanelRenderer>();
            if (_document == null && _panelRenderer == null) return;
            if (_runtimePanel == null)
            {
                _sourcePanel = _document != null
                    ? _document.panelSettings
                    : _panelRenderer.panelSettings;
                if (_sourcePanel == null)
                    throw new System.InvalidOperationException($"Mixed UI adapter on '{name}' requires PanelSettings.");
                _runtimePanel = Instantiate(_sourcePanel);
                _runtimePanel.name = $"{_sourcePanel.name} ({name} runtime)";
                if (_document != null)
                    _document.panelSettings = _runtimePanel;
                else
                    _panelRenderer.panelSettings = _runtimePanel;
            }
            _runtimePanel.sortingOrder = sortingOrder;
            var root = _document != null
                ? _document.rootVisualElement
                : GetComponent<UiToolkitViewPresentation>()?.Root;
            if (root != null)
            {
                if (!ReferenceEquals(_orderedRoot, root))
                {
                    _orderedRoot = root;
                    _sourcePickingMode = root.pickingMode;
                    _hasSourcePickingMode = true;
                }
                root.pickingMode = acceptsInput ? PickingMode.Position : PickingMode.Ignore;
            }
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
            if (_canvasGroup == null)
            {
                _canvasGroup = _canvas.GetComponent<CanvasGroup>();
                if (_canvasGroup == null)
                {
                    _canvasGroup = _canvas.gameObject.AddComponent<CanvasGroup>();
                    _addedCanvasGroup = true;
                }
                else
                {
                    _sourceCanvasGroupInteractable = _canvasGroup.interactable;
                    _sourceCanvasGroupBlocksRaycasts = _canvasGroup.blocksRaycasts;
                }
            }
            _canvasGroup.interactable = acceptsInput;
            _canvasGroup.blocksRaycasts = acceptsInput;
        }

        private void OnDestroy() => Release();
    }
}
