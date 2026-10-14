namespace UniGame.UiSystem.Runtime.UiToolkit
{
    using System;
    using System.Collections.Generic;
    using UniGame.ViewSystem.Runtime;
    using UnityEngine;
    using UnityEngine.EventSystems;

    /// <summary>Maps the ordered ViewSystem stack to Canvas and UIDocument ordering.</summary>
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
                adapter?.Release();
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
            var top = LastInteractive(views);
            var eventSystem = EventSystem.current;
            if (eventSystem != null && eventSystem.currentSelectedGameObject != null &&
                !BelongsTo(eventSystem.currentSelectedGameObject, top?.GameObject))
            {
                _previousSelectedObject = eventSystem.currentSelectedGameObject;
                eventSystem.SetSelectedGameObject(null);
            }

            for (var index = 0; index < views.Count; index++)
            {
                var view = views[index];
                if (view?.GameObject == null) continue;
                var adapter = view.GameObject.GetComponent<UiToolkitMixedOrderAdapter>() ??
                              view.GameObject.AddComponent<UiToolkitMixedOrderAdapter>();
                _adapters[view] = adapter;
                adapter.Apply(index, ReferenceEquals(view, top));
            }

            RemoveAdaptersOutsideStack(views);

            if (top?.GameObject == null) return;
            var toolkit = top.GameObject.GetComponent<UiToolkitViewPresentation>();
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

        private void RemoveAdaptersOutsideStack(IReadOnlyList<IView> views)
        {
            var removed = new List<IView>();
            foreach (var pair in _adapters)
            {
                if (!Contains(views, pair.Key)) removed.Add(pair.Key);
            }

            foreach (var view in removed)
            {
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

        private static IView LastInteractive(IReadOnlyList<IView> views)
        {
            for (var index = views.Count - 1; index >= 0; index--)
            {
                var view = views[index];
                if (view != null && view.GameObject != null && view.Status.CurrentValue is ViewStatus.Showing or ViewStatus.Shown) return view;
            }
            return null;
        }

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
