namespace UniGame.UiSystem.Runtime.UiToolkit
{
    using System;
    using System.Collections.Generic;
    using UniGame.ViewSystem.Runtime;
    using UnityEngine;
    using UnityEngine.EventSystems;

    /// <summary>Allows visible windows in the same group to receive pointer input together.</summary>
    public interface IUiToolkitSharedInputView
    {
        string SharedInputGroup { get; }
    }

    /// <summary>Maps the ordered ViewSystem stack to Canvas and UI Toolkit ordering.</summary>
    public sealed class UiToolkitMixedStackPresenter : IDisposable
    {
        private readonly ViewLayout _layout;
        private readonly Dictionary<IView, UiToolkitMixedOrderAdapter> _adapters = new();
        private GameObject _previousSelectedObject;
        private bool _isApplying;
        private bool _applyPending;

        public UiToolkitMixedStackPresenter(ViewLayout layout)
        {
            _layout = layout ?? throw new ArgumentNullException(nameof(layout));
            _layout.OrderChanged += Apply;
            Apply(_layout.OrderedViews);
        }

        public void Dispose()
        {
            _layout.OrderChanged -= Apply;
            foreach (var adapter in _adapters.Values)
            {
                if (adapter != null) adapter.RequestedOrderChanged -= Refresh;
                adapter?.Release();
            }
            _adapters.Clear();
        }

        public void Refresh() => Apply(_layout.OrderedViews);

        private void Apply(IReadOnlyList<IView> views)
        {
            if (_isApplying)
            {
                _applyPending = true;
                return;
            }

            _isApplying = true;
            try
            {
                do
                {
                    _applyPending = false;
                    ApplyCurrentStack(_layout.OrderedViews);
                } while (_applyPending);
            }
            finally
            {
                _isApplying = false;
            }
        }

        private void ApplyCurrentStack(IReadOnlyList<IView> views)
        {
            foreach (var view in views)
            {
                if (view?.GameObject != null) EnsureAdapter(view);
            }

            int? layerBaseOrder = _layout.LayoutCanvas != null ? _layout.LayoutCanvas.sortingOrder : null;
            var top = LastInteractive(views, layerBaseOrder, out var highestOrder, out var topOrderCount);
            var hasOrderTie = topOrderCount > 1;
            var eventSystem = EventSystem.current;
            if (!hasOrderTie && eventSystem != null && eventSystem.currentSelectedGameObject != null &&
                !BelongsTo(eventSystem.currentSelectedGameObject, top?.GameObject))
            {
                _previousSelectedObject = eventSystem.currentSelectedGameObject;
                eventSystem.SetSelectedGameObject(null);
            }

            for (var index = 0; index < views.Count; index++)
            {
                var view = views[index];
                if (view?.GameObject == null) continue;
                var adapter = _adapters[view];
                adapter.Apply(index, AcceptsInput(view) &&
                    (IsInteractiveAtOrder(view, adapter, index, highestOrder, layerBaseOrder) || SharesInputGroup(view, top)),
                    _layout.LayerType, layerBaseOrder);
            }

            RemoveAdaptersOutsideStack(views);

            if (top?.GameObject == null || hasOrderTie) return;
            var toolkit = top as IUiToolkitView;
            if (toolkit != null)
            {
                toolkit.FocusFirstFocusable();
                return;
            }

            if (eventSystem == null) return;
            if (_previousSelectedObject != null && BelongsTo(_previousSelectedObject, top.GameObject) && _previousSelectedObject.activeInHierarchy)
                eventSystem.SetSelectedGameObject(_previousSelectedObject);
            else
                eventSystem.SetSelectedGameObject(FirstSelectable(top.GameObject));
        }

        private UiToolkitMixedOrderAdapter EnsureAdapter(IView view)
        {
            if (_adapters.TryGetValue(view, out var adapter) && adapter != null)
                return adapter;

            adapter = view.GameObject.GetComponent<UiToolkitMixedOrderAdapter>() ??
                      view.GameObject.AddComponent<UiToolkitMixedOrderAdapter>();
            adapter.RequestedOrderChanged += Refresh;
            _adapters[view] = adapter;
            return adapter;
        }

        private void RemoveAdaptersOutsideStack(IReadOnlyList<IView> views)
        {
            var removed = new List<IView>();
            foreach (var pair in _adapters)
            {
                if (!Contains(views, pair.Key)) removed.Add(pair.Key);
            }

            foreach (var view in removed)
            {
                if (_adapters[view] != null)
                    _adapters[view].RequestedOrderChanged -= Refresh;
                _adapters[view]?.Release();
                _adapters.Remove(view);
            }
        }

        private static bool Contains(IReadOnlyList<IView> views, IView target)
        {
            for (var index = 0; index < views.Count; index++)
            {
                if (ReferenceEquals(views[index], target)) return true;
            }

            return false;
        }

        private IView LastInteractive(IReadOnlyList<IView> views, int? layerBaseOrder, out int highestOrder, out int topOrderCount)
        {
            IView top = null;
            highestOrder = int.MinValue;
            topOrderCount = 0;
            for (var index = 0; index < views.Count; index++)
            {
                var view = views[index];
                if (view != null && view.GameObject != null && AcceptsInput(view) &&
                    view.Status.CurrentValue is ViewStatus.Showing or ViewStatus.Shown)
                {
                    var order = _adapters[view].CalculateSortingOrder(index, _layout.LayerType, layerBaseOrder);
                    if (order < highestOrder) continue;
                    topOrderCount = order == highestOrder ? topOrderCount + 1 : 1;
                    highestOrder = order;
                    top = view;
                }
            }
            return top;
        }

        private bool IsInteractiveAtOrder(IView view, UiToolkitMixedOrderAdapter adapter, int index, int order, int? layerBaseOrder) =>
            view.Status.CurrentValue is ViewStatus.Showing or ViewStatus.Shown &&
            adapter.CalculateSortingOrder(index, _layout.LayerType, layerBaseOrder) == order;

        private static bool AcceptsInput(IView view)
        {
            return view is not IUiToolkitView toolkit || toolkit.AcceptInput;
        }

        private static bool SharesInputGroup(IView view, IView top) =>
            view is IUiToolkitSharedInputView member &&
            top is IUiToolkitSharedInputView active &&
            !string.IsNullOrWhiteSpace(member.SharedInputGroup) &&
            string.Equals(member.SharedInputGroup, active.SharedInputGroup, StringComparison.Ordinal);

        private static bool BelongsTo(GameObject candidate, GameObject owner) =>
            candidate != null && owner != null &&
            (candidate == owner || candidate.transform.IsChildOf(owner.transform));

        private static GameObject FirstSelectable(GameObject owner)
        {
            foreach (var selectable in owner.GetComponentsInChildren<UnityEngine.UI.Selectable>(true))
                if (selectable.IsActive() && selectable.IsInteractable()) return selectable.gameObject;
            return null;
        }
    }
}
