using System;
using TMPro;
using UniGame.Localization.Runtime.UniModules.UniGame.Localization.Runtime;
using UniGame.Core.Runtime;
using UnityEngine.Localization;

namespace UniModules.UniGame.UiSystem.Runtime.Extensions
{
    using global::UniGame.ViewSystem.Runtime;

    public static class LocalizationBindingExtension
    {
        public static IDisposable Bind<TSource>(this TSource source,LocalizedString localizedString,TextMeshProUGUI text, int frameThrottle = 1)
            where TSource : ILifeTimeContext
        {
            return localizedString.Bind(x => text.SetValue(x), frameThrottle).AddTo(source.LifeTime);
        }
        
        public static IDisposable Bind<TSource>(this TSource source,LocalizedString localizedString,Action<string> action, int frameThrottle = 1)
            where TSource : ILifeTimeContext
        {
            return localizedString.Bind(action, frameThrottle).AddTo(source.LifeTime);
        }
        
        public static IDisposable Bind<TSource>(this TSource source,LocalizedString localizedString,TextMeshPro text, int frameThrottle = 1)
            where TSource : ILifeTimeContext
        {
            return localizedString.Bind(x => text.SetValue(x), frameThrottle).AddTo(source.LifeTime);
        }
        
        
    }
}