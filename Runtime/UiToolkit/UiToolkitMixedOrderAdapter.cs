namespace UniGame.UiSystem.Runtime.UiToolkit
{
    using UnityEngine;
    using UnityEngine.UIElements;
#if UNITY_6000_6_OR_NEWER
    using UiToolkitHost = UnityEngine.UIElements.PanelRenderer;
#else
    using UiToolkitHost = UnityEngine.UIElements.UIDocument;
#endif

    /// <summary>
    /// Applies one ViewSystem stack position to a UI Toolkit host (PanelRenderer
    /// on Unity 6.6+, UIDocument before). Each host receives a runtime PanelSettings instance,
    /// while the configured source asset is restored when the adapter is released.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class UiToolkitMixedOrderAdapter : MonoBehaviour
    {
        [SerializeField] private int orderStep = 10;

        private UiToolkitHost _panelRenderer;
        private PanelSettings _sourcePanel;
        private PanelSettings _runtimePanel;
        private Canvas _canvas;
        private CanvasGroup _canvasGroup;
        private bool _addedCanvasGroup;
        private bool _sourceCanvasGroupInteractable;
        private bool _sourceCanvasGroupBlocksRaycasts;
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
            if (_panelRenderer != null && _runtimePanel != null && _panelRenderer.panelSettings == _runtimePanel)
            {
                GetComponent<UiToolkitViewPresentation>()?.PrepareForPanelReload();
                _panelRenderer.panelSettings = _sourcePanel;
            }
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
            GetComponent<UiToolkitViewPresentation>()?.SetStackInputAllowed(true);
            CurrentStackIndex = -1;
        }

        private void ApplyToolkitOrder(int sortingOrder, bool acceptsInput)
        {
            _panelRenderer ??= GetComponent<UiToolkitHost>();
            if (_panelRenderer == null) return;
            if (_runtimePanel == null)
            {
                _sourcePanel = _panelRenderer.panelSettings;
                if (_sourcePanel == null)
                    throw new System.InvalidOperationException($"Mixed UI adapter on '{name}' requires PanelSettings.");
                _runtimePanel = Instantiate(_sourcePanel);
                _runtimePanel.name = $"{_sourcePanel.name} ({name} runtime)";
                GetComponent<UiToolkitViewPresentation>()?.PrepareForPanelReload();
                _panelRenderer.panelSettings = _runtimePanel;
            }
            _runtimePanel.sortingOrder = sortingOrder;
            GetComponent<UiToolkitViewPresentation>()?.SetStackInputAllowed(acceptsInput);
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
