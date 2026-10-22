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

    /// <summary>Selects which visual state a boolean UI Toolkit binding updates.</summary>
    public enum UiToolkitBindingMode
    {
        Display,
        Enabled,
    }

    /// <summary>Bindings scoped to a UI Toolkit presentation tree or view lifetime.</summary>
    public static class UiToolkitReactiveBindingExtensions
    {
        public static TView Bind<TView>(this TView view, ReactiveValue<string> source, Label target)
            where TView : ILifeTimeContext => BindValue(view, source, target,
                static (value, element) => element.text = value ?? string.Empty);

        public static TView Bind<TView>(this TView view, ReactiveValue<string> source, Button target)
            where TView : ILifeTimeContext => BindValue(view, source, target,
                static (value, element) => element.text = value ?? string.Empty);

        public static TView Bind<TView>(this TView view, ReactiveValue<float> source, ProgressBar target)
            where TView : ILifeTimeContext => BindValue(view, source, target,
                static (value, element) => element.value = value);

        public static TView Bind<TView>(this TView view, ReactiveValue<Color> source, VisualElement target)
            where TView : ILifeTimeContext => BindValue(view, source, target,
                static (value, element) => element.style.color = value);

        public static TView Bind<TView>(this TView view, ReactiveValue<bool> source, Toggle target)
            where TView : ILifeTimeContext => BindValue(view, source, target,
                static (value, element) => element.SetValueWithoutNotify(value));

        /// <summary>Binds visibility by default; use the mode overload for enabled state.</summary>
        public static TView Bind<TView>(this TView view, ReactiveValue<bool> source, VisualElement target)
            where TView : ILifeTimeContext => Bind(view, source, target, UiToolkitBindingMode.Display);

        public static TView Bind<TView>(
            this TView view,
            ReactiveValue<bool> source,
            VisualElement target,
            UiToolkitBindingMode mode)
            where TView : ILifeTimeContext
        {
            return mode == UiToolkitBindingMode.Enabled
                ? BindValue(view, source, target, static (value, element) => element.SetEnabled(value))
                : BindValue(view, source, target, static (value, element) =>
                    element.style.display = value ? DisplayStyle.Flex : DisplayStyle.None);
        }

        /// <summary>Binds membership in a USS class.</summary>
        public static TView Bind<TView>(
            this TView view,
            ReactiveValue<bool> source,
            VisualElement target,
            string className)
            where TView : ILifeTimeContext
        {
            if (view == null || source == null || target == null || string.IsNullOrWhiteSpace(className))
                return view;

            var subscription = source.Where(source, static (_, value) => value.HasValue)
                .Subscribe((target, className), static (value, state) =>
                    state.target.EnableInClassList(state.className, value));
            RegisterBinding(view, subscription);
            return view;
        }

        /// <summary>Renders each published value through a view method.</summary>
        public static TView Bind<TView, TValue>(
            this TView view,
            ReactiveValue<TValue> source,
            Action<TValue> render)
            where TView : ILifeTimeContext
        {
            if (view == null || source == null || render == null)
                return view;

            var subscription = source.Where(source, static (_, value) => value.HasValue)
                .Subscribe(render, static (value, action) => action(value));
            RegisterBinding(view, subscription);
            return view;
        }

        /// <summary>Binds a button click to a one-update signal.</summary>
        public static TView Bind<TView>(this TView view, Button button, ISignalValueProperty<bool> signal)
            where TView : ILifeTimeContext => Bind(view, button, signal, true);

        public static TView Bind<TView, TValue>(
            this TView view,
            Button button,
            ISignalValueProperty<TValue> signal,
            TValue value)
            where TView : ILifeTimeContext
        {
            if (view == null || button == null || signal == null)
                return view;

            RegisterBinding(view, new SignalButtonBinding<TValue>(button, signal, value));
            return view;
        }

        /// <summary>Binds text field input to a reactive value.</summary>
        public static TView Bind<TView>(this TView view, TextField field, ReactiveValue<string> value)
            where TView : ILifeTimeContext
        {
            if (view == null || field == null || value == null)
                return view;

            field.SetValueWithoutNotify(value.Value ?? string.Empty);
            RegisterBinding(view, new TextFieldBinding(field, value));
            return view;
        }

        private static TView BindValue<TView, TValue, TElement>(
            TView view,
            ReactiveValue<TValue> source,
            TElement target,
            Action<TValue, TElement> render)
            where TView : ILifeTimeContext
            where TElement : VisualElement
        {
            if (view == null || source == null || target == null)
                return view;

            var subscription = source.Where(source, static (_, value) => value.HasValue)
                .Subscribe(target, render);
            RegisterBinding(view, subscription);
            return view;
        }

        private static void RegisterBinding<TView>(TView view, IDisposable binding)
            where TView : ILifeTimeContext
        {
            if (view is IUiToolkitView toolkit)
            {
                toolkit.AddUnbind(binding.Dispose);
                return;
            }

            binding.AddTo(view.LifeTime);
        }

        private sealed class TextFieldBinding : IDisposable
        {
            private readonly TextField _field;
            private readonly ReactiveValue<string> _value;

            public TextFieldBinding(TextField field, ReactiveValue<string> value)
            {
                _field = field;
                _value = value;
                field.RegisterValueChangedCallback(OnChanged);
            }

            public void Dispose() => _field.UnregisterValueChangedCallback(OnChanged);

            private void OnChanged(ChangeEvent<string> change) => _value.Value = change.newValue;
        }

        private sealed class SignalButtonBinding<TValue> : IDisposable
        {
            private readonly Button _button;
            private readonly ISignalValueProperty<TValue> _signal;
            private readonly TValue _value;

            public SignalButtonBinding(Button button, ISignalValueProperty<TValue> signal, TValue value)
            {
                _button = button;
                _signal = signal;
                _value = value;
                button.clicked += OnClicked;
            }

            public void Dispose() => _button.clicked -= OnClicked;

            private void OnClicked() => _signal.SetValue(_value);
        }
    }
}
