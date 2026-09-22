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
        private GameObject _previousSelectedObject;

        public UiToolkitMixedStackPresenter(ViewLayout layout)
        {
            _layout = layout ?? throw new ArgumentNullException(nameof(layout));
            _layout.OrderChanged += Apply;
            Apply(_layout.OrderedViews);
        }

        public void Dispose()
        {
            _layout.OrderChanged -= Apply;
            foreach (var view in _layout.OrderedViews)
                view?.GameObject?.GetComponent<UiToolkitMixedOrderAdapter>()?.Release();
        }

        private void Apply(IReadOnlyList<IView> views)
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
                adapter.Apply(index, ReferenceEquals(view, top));
            }

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
            candidate != null && owner != null && candidate.transform.IsChildOf(owner.transform);

        private static GameObject FirstSelectable(GameObject owner)
        {
            foreach (var selectable in owner.GetComponentsInChildren<UnityEngine.UI.Selectable>(true))
                if (selectable.IsActive() && selectable.IsInteractable()) return selectable.gameObject;
            return null;
        }
    }
}
