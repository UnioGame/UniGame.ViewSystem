namespace UniGame.UiSystem.Runtime.UiToolkit
{
    using System;
    using R3;
    using UniGame.Core.Runtime;
    using UniGame.Runtime.Common;
    using UniGame.Runtime.Rx;
    using UniGame.ViewSystem.Runtime;
    using UnityEngine;
    using UnityEngine.UIElements;

    /// <summary>Lifecycle-scoped reactive bindings shared by UI Toolkit views.</summary>
    public static class UiToolkitReactiveBindingExtensions
    {
        public static TView BindToolkitValue<TView>(this TView view, ReactiveValue<string> source, Label target)
            where TView : ILifeTimeContext => BindValue(view, source, target, static (v, e) => e.text = v ?? string.Empty);

        public static TView BindToolkitValue<TView>(this TView view, ReactiveValue<Color> source, VisualElement target)
            where TView : ILifeTimeContext => BindValue(view, source, target, static (v, e) => e.style.color = v);

        public static TView BindToolkitDisplay<TView>(this TView view, ReactiveValue<bool> source, VisualElement target)
            where TView : ILifeTimeContext => BindValue(view, source, target,
                static (v, e) => e.style.display = v ? DisplayStyle.Flex : DisplayStyle.None);

        public static TView BindToolkitReactive<TView, TValue>(
            this TView view,
            ReactiveValue<TValue> source,
            Action<TValue> callback)
            where TView : ILifeTimeContext
        {
            if (view == null || source == null || callback == null)
                return view;

            var subscription = source.Where(source, static (_, value) => value.HasValue)
                .Subscribe(callback, static (value, action) => action(value));
            RegisterBinding(view, subscription);
            return view;
        }

        private static TView BindValue<TView, TValue, TElement>(
            TView view,
            ReactiveValue<TValue> source,
            TElement target,
            Action<TValue, TElement> callback)
            where TView : ILifeTimeContext
            where TElement : VisualElement
        {
            if (view == null || source == null || target == null)
                return view;

            var subscription = source.Where(source, static (_, value) => value.HasValue)
                .Subscribe(target, callback);
            RegisterBinding(view, subscription);
            return view;
        }

        private static void RegisterBinding<TView>(TView view, IDisposable subscription)
            where TView : ILifeTimeContext
        {
            if (view is Component component)
            {
                var presentation = component.GetComponent<UiToolkitViewPresentation>();
                if (presentation != null)
                {
                    presentation.AddUnbind(subscription.Dispose);
                    return;
                }
            }

            subscription.AddTo(view.LifeTime);
        }
    }
}
