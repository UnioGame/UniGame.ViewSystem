using System;
using TMPro;
using UniGame.Localization.Runtime.UniModules.UniGame.Localization.Runtime;
using UniGame.Core.Runtime;
using UnityEngine.Localization;

namespace UniModules.UniGame.UiSystem.Runtime.Extensions
{
    public static class SourcesBindingExtension
    {
        public static IDisposable BindTo<TSource>(this TSource source,LocalizedString localizedString,TextMeshProUGUI text, int frameThrottle = 1)
            where TSource : ILifeTimeContext
        {
            return localizedString.BindTo(x => text.text = x, frameThrottle).AddTo(source.LifeTime);
        }
        
        public static IDisposable BindTo<TSource>(this TSource source,LocalizedString localizedString,Action<string> action, int frameThrottle = 1)
            where TSource : ILifeTimeContext
        {
            return localizedString.BindTo(x => action(x), frameThrottle).AddTo(source.LifeTime);
        }
        
        public static IDisposable BindTo<TSource>(this TSource source,LocalizedString localizedString,TextMeshPro text, int frameThrottle = 1)
            where TSource : ILifeTimeContext
        {
            return localizedString.BindTo(x => text.text = x, frameThrottle).AddTo(source.LifeTime);
        }
    }
}