namespace UniGame.ViewSystem.Runtime.Extensions
{
    using TMPro;
    using UniGame.Core.Runtime;
    using UniGame.Runtime.Common;
    using UniGame.Runtime.Rx;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>Exposes dependency-focused bindings for common View components.</summary>
    public static class ViewComponentBindingExtensions
    {
        /// <summary>Binds reactive text to a TextMesh Pro component.</summary>
        public static TView Bind<TView>(this TView view, ReactiveValue<string> source, TMP_Text target)
            where TView : ILifeTimeContext
        {
            return UniGame.Runtime.Rx.Runtime.Extensions.ViewBindingExtension.Bind(view, source, target);
        }

        /// <summary>Binds reactive availability to a button.</summary>
        public static TView Bind<TView>(this TView view, ReactiveValue<bool> source, Button target)
            where TView : ILifeTimeContext
        {
            return UniGame.Runtime.Rx.Runtime.Extensions.ViewBindingExtension.Bind(view, source, target);
        }

        /// <summary>Binds a button click to a one-update boolean signal.</summary>
        public static TView Bind<TView>(this TView view, Button source, ISignalValueProperty<bool> target)
            where TView : ILifeTimeContext
        {
            return UniGame.Runtime.Rx.Runtime.Extensions.ViewBindingExtension.Bind(view, source, target);
        }

        /// <summary>Updates Text Mesh Pro text only when the value changes.</summary>
        public static bool SetValue(this TMP_Text target, string value)
        {
            return ValuesExtensions.SetValue(target, value);
        }

        /// <summary>Updates a Text Mesh Pro color.</summary>
        public static bool SetValue(this TMP_Text target, Color value)
        {
            return ValuesExtensions.SetValue(target, value);
        }
    }
}
