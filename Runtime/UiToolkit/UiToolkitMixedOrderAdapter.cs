namespace UniGame.UiSystem.Runtime.UiToolkit
{
#if !UNITY_6000_5_OR_NEWER
    using System.Collections.Generic;
#endif
    using UnityEngine;
    using UnityEngine.UIElements;
    using UniModules.UniGame.UiSystem.Runtime;
#if UNITY_6000_5_OR_NEWER
    using PanelHost = UnityEngine.UIElements.PanelRenderer;
#else
    using PanelHost = UnityEngine.UIElements.UIDocument;
#endif

    /// <summary>
    /// Applies one ViewSystem stack position to a UI Toolkit host. Each host
    /// receives a runtime PanelSettings instance,
    /// while the configured source asset is restored when the adapter is released.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class UiToolkitMixedOrderAdapter : MonoBehaviour
    {
        internal const int LayerCapacity = 100;

        [SerializeField] private int orderStep = 10;

        private PanelHost _panelHost;
        private PanelSettings _sourcePanel;
        private PanelSettings _runtimePanel;
        private Canvas _canvas;
        private CanvasGroup _canvasGroup;
        private bool _addedCanvasGroup;
        private bool _sourceCanvasGroupInteractable;
        private bool _sourceCanvasGroupBlocksRaycasts;
        private int _sourceCanvasOrder;
        private bool _sourceOverrideSorting;
#if UNITY_6000_5_OR_NEWER
        private int _sourceRendererOrder;
#else
        private float _sourceRendererOrder;
#endif
        private bool _sourceRendererSortingCaptured;
        private int? _requestedOrder;
        private bool _hasExternalPlacement;
        private bool _acceptsInput;
        private ViewType _layerType;
#if !UNITY_6000_5_OR_NEWER
        private readonly Dictionary<UIDocument, PanelSettings> _nestedPanelSources = new();
#endif

        public int CurrentStackIndex { get; private set; } = -1;

        public event System.Action RequestedOrderChanged;

        /// <summary>Overrides the stack order with an explicit request order.</summary>
        public void SetRequestedOrder(int order, ViewType layerType = ViewType.None)
        {
            if (_requestedOrder == order) return;
            var previousOrder = _requestedOrder;
            _requestedOrder = order;
            try
            {
                ApplyCurrent(CurrentStackIndex < 0 ? 0 : CurrentStackIndex,
                    CurrentStackIndex < 0 || _acceptsInput,
                    CurrentStackIndex < 0 ? layerType : _layerType);
            }
            catch
            {
                _requestedOrder = previousOrder;
                throw;
            }
            RequestedOrderChanged?.Invoke();
        }

        /// <summary>Removes the request override while preserving an active stack placement.</summary>
        public void ClearRequestedOrder()
        {
            if (!_requestedOrder.HasValue) return;
            _requestedOrder = null;
            if (_hasExternalPlacement)
                ApplyCurrent(CurrentStackIndex, _acceptsInput, _layerType);
            else
                Release();
            RequestedOrderChanged?.Invoke();
        }

        internal int CalculateSortingOrder(int stackIndex, ViewType layerType)
        {
            var layered = TryGetLayerBase(layerType, out var baseOrder);
            if (layered && (stackIndex < 0 || stackIndex >= LayerCapacity))
                throw new System.ArgumentOutOfRangeException(nameof(stackIndex), stackIndex,
                    $"{layerType} supports stack indices from 0 to {LayerCapacity - 1}.");
            if (layered && _requestedOrder is < 0 or >= LayerCapacity)
                throw new System.ArgumentOutOfRangeException(nameof(_requestedOrder), _requestedOrder,
                    $"The requested {layerType} order must be between 0 and {LayerCapacity - 1}.");

            var sortingOrder = layered
                ? (long)baseOrder + (_requestedOrder ?? stackIndex)
                : _requestedOrder ?? (long)stackIndex * Mathf.Max(1, orderStep);
            if (sortingOrder < short.MinValue || sortingOrder > short.MaxValue)
                throw new System.ArgumentOutOfRangeException(
                    _requestedOrder.HasValue ? nameof(_requestedOrder) : nameof(stackIndex), sortingOrder,
                    $"The sorting order must be between {short.MinValue} and {short.MaxValue}.");
            return (int)sortingOrder;
        }

        public void Apply(int stackIndex, bool acceptsInput)
        {
            Apply(stackIndex, acceptsInput, ViewType.None);
        }

        public void Apply(int stackIndex, bool acceptsInput, ViewType layerType)
        {
            ApplyCurrent(stackIndex, acceptsInput, layerType);
            _hasExternalPlacement = true;
        }

        private void ApplyCurrent(int stackIndex, bool acceptsInput, ViewType layerType)
        {
            var layered = TryGetLayerBase(layerType, out _);
            var sortingOrder = CalculateSortingOrder(stackIndex, layerType);

            CurrentStackIndex = stackIndex;
            _acceptsInput = acceptsInput;
            _layerType = layerType;
            ApplyToolkitOrder(sortingOrder, acceptsInput, layered);
            ApplyCanvasOrder(sortingOrder, acceptsInput);
        }

        internal static bool HasLayerRange(ViewType layerType) => TryGetLayerBase(layerType, out _);

        internal static bool TryGetLayerBase(ViewType layerType, out int baseOrder)
        {
            baseOrder = layerType switch
            {
                ViewType.Screen => 100,
                ViewType.Window => 200,
                ViewType.Overlay => 300,
                _ => 0
            };
            return layerType is ViewType.Screen or ViewType.Window or ViewType.Overlay;
        }

        public void Release()
        {
            if (_panelHost != null && _runtimePanel != null && _panelHost.panelSettings == _runtimePanel)
            {
                GetComponent<IUiToolkitView>()?.PrepareForPanelReload();
#if !UNITY_6000_5_OR_NEWER
                SetNestedPanels(_sourcePanel, false);
#endif
                _panelHost.panelSettings = _sourcePanel;
#if !UNITY_6000_5_OR_NEWER
                GetComponent<IUiToolkitView>()?.RefreshHost();
#endif
            }
#if !UNITY_6000_5_OR_NEWER
            RestoreNestedPanels();
#endif
            if (_runtimePanel != null)
            {
                if (Application.isPlaying) Destroy(_runtimePanel);
                else DestroyImmediate(_runtimePanel);
            }
            RestoreRendererSorting();
            _runtimePanel = null;
            _sourcePanel = null;
            _panelHost = null;

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
            _requestedOrder = null;
            _hasExternalPlacement = false;
            _acceptsInput = false;
            _layerType = ViewType.None;
            GetComponent<IUiToolkitView>()?.SetStackInputAllowed(true);
            CurrentStackIndex = -1;
        }

        private void ApplyToolkitOrder(int sortingOrder, bool acceptsInput, bool layered)
        {
            _panelHost ??= GetComponent<PanelHost>();
            if (_panelHost == null) return;
#if !UNITY_6000_5_OR_NEWER
            if (_panelHost.parentUI != null)
            {
                GetComponent<IUiToolkitView>()?.SetStackInputAllowed(acceptsInput);
                return;
            }
#endif
            if (_runtimePanel == null)
            {
                _sourcePanel = _panelHost.panelSettings;
                if (_sourcePanel == null)
                    throw new System.InvalidOperationException($"Mixed UI adapter on '{name}' requires PanelSettings.");
                _runtimePanel = Instantiate(_sourcePanel);
                _runtimePanel.name = $"{_sourcePanel.name} ({name} runtime)";
                GetComponent<IUiToolkitView>()?.PrepareForPanelReload();
#if !UNITY_6000_5_OR_NEWER
                SetNestedPanels(_runtimePanel, true);
#endif
                _panelHost.panelSettings = _runtimePanel;
#if !UNITY_6000_5_OR_NEWER
                GetComponent<IUiToolkitView>()?.RefreshHost();
#endif
            }
            _runtimePanel.sortingOrder = sortingOrder;
            if (layered || _requestedOrder.HasValue)
            {
                if (!_sourceRendererSortingCaptured)
                {
                    _sourceRendererOrder = _panelHost.sortingOrder;
                    _sourceRendererSortingCaptured = true;
                }
                _panelHost.sortingOrder = sortingOrder;
            }
            else
            {
                RestoreRendererSorting();
            }
            GetComponent<IUiToolkitView>()?.SetStackInputAllowed(acceptsInput);
        }

#if !UNITY_6000_5_OR_NEWER
        // Nested UIDocuments must share the parent's panel during a PanelSettings switch.
        private void SetNestedPanels(PanelSettings panel, bool rememberSources)
        {
            foreach (var nested in GetComponentsInChildren<UIDocument>(true))
            {
                if (nested == _panelHost)
                    continue;
                if (rememberSources && !_nestedPanelSources.ContainsKey(nested))
                    _nestedPanelSources.Add(nested, nested.panelSettings);
                if (nested.panelSettings != panel)
                    nested.panelSettings = panel;
            }
        }

        private void RestoreNestedPanels()
        {
            foreach (var source in _nestedPanelSources)
            {
                if (source.Key != null && source.Key.panelSettings != source.Value)
                    source.Key.panelSettings = source.Value;
            }
            _nestedPanelSources.Clear();
        }
#endif

        private void RestoreRendererSorting()
        {
            if (_panelHost != null && _sourceRendererSortingCaptured)
                _panelHost.sortingOrder = _sourceRendererOrder;
            _sourceRendererSortingCaptured = false;
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
