using UniGame.Addressables.Reactive;
using UniGame.AddressableTools.Runtime;

namespace UniGame.Rx.Runtime.Extensions
{
    using System;
    using Cysharp.Threading.Tasks;
    using TMPro;
    using global::UniGame.Core.Runtime;
    using global::UniGame.ViewSystem.Runtime;
    using UniModules.UniGame.UISystem.Runtime.Extensions;
    using UnityEngine;
    using UnityEngine.UI;
    using global::UniGame.Localization.Runtime.UniModules.UniGame.Localization.Runtime;
    using global::UniGame.UiSystem.Runtime;
    using UnityEngine.AddressableAssets;
    using UnityEngine.Localization;
    using UnityEngine.Localization.Components;
    using Object = UnityEngine.Object;
    
    public static class ViewBindingExtension
    {
        
        #region ugui extensions
        
        public static TView Bind<TView>(this TView view, LocalizedString source, Action<string> command)
            where TView : class,IView
        {
            return source == null ? view : view.Bind(source.AsObservable(), command);
        }

        public static TView Bind<TView>(this TView view, LocalizedString source, TextMeshProUGUI text)
            where TView : class,IView
        {
            return source == null ? view : view.Bind(source.AsObservable(), text);
        }
        
        public static TView Bind<TView>(this TView view, LocalizeStringEvent source, TextMeshProUGUI text)
            where TView : class,IView
        {
            return source == null || source.StringReference == null || source.StringReference.IsEmpty
                ? view : view.Bind(source.StringReference.AsObservable(), text);
        }
        
        public static TView Bind<TView>(this TView view, LocalizeStringEvent source, Action<string> command)
            where TView : class,IView
        {
            return source == null || source.StringReference == null || source.StringReference.IsEmpty
                ? view : view.Bind(source.StringReference.AsObservable(), command);
        }
        
        public static TView Bind<TView>(this TView view, AssetReferenceT<Sprite> source, Image image)
            where TView : class,IView
        {
            if (source.RuntimeKeyIsValid() == false) return view;
            return !image ? view : view.Bind(source.ToObservable(view.LifeTime), image);
        }
        
        public static TView Bind<TView,TValue>(this TView view, AssetReferenceT<TValue> source, Action<TValue> action)
            where TView : class,IView
            where TValue : Object
        {
            if (source.RuntimeKeyIsValid() == false || action == null) return view;
            AddressableAction(view.LifeTime,source,action).Forget();
            return view;
        }

        private static async UniTask AddressableAction<TValue>(ILifeTime lifeTime, AssetReferenceT<TValue> source,Action<TValue> action)
            where TValue : Object
        {
            if(action == null || lifeTime.IsTerminated)
                return;
            var value = await source.LoadAssetTaskAsync(lifeTime);
            action(value);
        }

        #endregion

        public static async UniTask<TView> Bind<TView,TModel>(this TView view,TModel model, IView target) 
            where TView : class, IView
            where TModel : IViewModel
        {
            await target.Initialize(model);
            return view;
        }
        
        public static TSource BindClose<TSource,TView>(
            this TSource view,
            TView source)
            where TSource : IView
            where TView : IView
        {
            source.CloseWith(view.LifeTime);
            return view;
        }
        
        public static TSource BindClose<TSource,T>(
            this TSource view,
            IView target)
            where TSource : IView
            where T : IViewModel
        {
            target.CloseWith(view.LifeTime);
            return view;
        }
        
        public static TSource Bind<TSource,T>(
            this TSource view,
            IObservable<T> source, 
            ViewBase target, 
            bool closeWith = false)
            where TSource : ViewBase
            where T : IViewModel
        {
            if (closeWith) view.BindClose(target);
            
            view.Bind(source, x => target.Initialize(x, view.Layout)
                    .AttachExternalCancellation(view.ModelLifeTime.TokenSource)
                    .Forget());

            return view;
        }

        public static TSource BindToWindow<TSource>(
            this TSource view,
            IObservable<IViewModel> source,
            Type viewType)
            where TSource : ViewBase
        {
            return view.Bind(source, x => view.OpenAsWindowAsync(x,viewType));
        }
        
        public static TSource BindWhere<TSource,T>(
            this TSource view,
            Object indicator,
            IObservable<T> source, 
            Action<T> target)
            where TSource : IView
        {
            return indicator != null ? view.Bind(source, target) : view;
        }

    }
}
