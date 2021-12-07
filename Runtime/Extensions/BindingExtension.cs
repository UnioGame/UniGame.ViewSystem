using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UniModules.UniCore.Runtime.Utils;
using UniModules.UniGame.Rx.Runtime.Extensions;
using UniModules.UniGame.UISystem.Runtime.Abstract;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UniModules.UniGame.UiSystem.Runtime.Extensions
{
    using System.Collections.Generic;
    using Core.Runtime.DataFlow.Interfaces;
    using global::UniCore.Runtime.ProfilerTools;
    using global::UniGame.Addressables.Reactive;
    using global::UniGame.Localization.Runtime.UniModules.UniGame.Localization.Runtime;
    using global::UniGame.UiSystem.Runtime;
    using UISystem.Runtime.Extensions;
    using UniCore.Runtime.Rx.Extensions;
    using UniModules.UniGame.Core.Runtime.Interfaces;
    using UnityEngine.AddressableAssets;
    using UnityEngine.Localization;
    using UniUiSystem.Runtime.Utils;
    using Object = UnityEngine.Object;

    public static class BindingExtension
    {
        
        #region ugui extensions
        
        public static TView Bind<TView>(this TView view, LocalizedString source, Action<string> command, int frameThrottle = 0)
            where TView : class,IView
        {
            if (source == null) return view;
            
            Bind(source.AsObservable(),command,frameThrottle)
                .AddTo(view.ModelLifeTime);
            return view;
        }
        
        public static TView Bind<TView,TValue>(this TView view, IEnumerable<TValue> source, Action<TValue> action)
            where TView : class,IView
        {
            if (source == null || action == null) return view;

            foreach (var value in source)
                action(value);
            
            return view;
        }

        public static TView Bind<TView>(this TView view, LocalizedString source, TextMeshProUGUI text, int frameThrottle = 0)
            where TView : class,IView
        {
            return Bind(view, source, x => text.text = x, frameThrottle);
        }
        
        public static TView Bind<TView,TValue>(this TView view, IObservable<TValue> source, Button command, int frameThrottle = 0)
            where TView : class,IView
        {
            Bind(source,x => command.onClick?.Invoke(),frameThrottle)
                .AddTo(view.ModelLifeTime);
            return view;
        }
        
        public static TView Bind<TView>(this TView view, Button source, Action<Unit> command,int throttleInMilliseconds = 0)
            where           TView : class,IView
        {
            if (!source) return view;

            var clickObservable = throttleInMilliseconds <= 0
                ? source.OnClickAsObservable()
                : source.OnClickAsObservable().ThrottleFirst(TimeSpan.FromMilliseconds(throttleInMilliseconds));
            
            Bind(clickObservable,command,0)
                .AddTo(view.ModelLifeTime);
            return view;
        }

        public static TView Bind<TView>(this TView view, Button source, Action command, int throttleInMilliseconds = 0)
            where TView : class,IView
        {
            return view.Bind(source, x => command(), throttleInMilliseconds);
        }
        
        public static TView Bind<TView,TValue>(this TView view, IObservable<TValue> source, Action command, int frameThrottle = 0)
            where TView : class,IView
        {
            return view.Bind(source, x => command(), frameThrottle);
        }
        
        public static TView Bind<TView>(this TView view, Button source, IReactiveCommand<Unit> command, int frameThrottle = 0)
            where TView : class,IView
        {
            if (!source) return view;
            Bind(source.OnClickAsObservable(),command,frameThrottle)
                .AddTo(view.ModelLifeTime);
            return view;
        }
        
        public static TView Bind<TView>(this TView view, IObservable<bool> source, Button button, int frameThrottle = 0)
            where TView : class,IView
        {
            if (!button) return view;
            Bind(source,x => button.interactable = x,frameThrottle)
                .AddTo(view.ModelLifeTime);
            return view;
        }
        
        public static TView Bind<TView>(this TView view, IObservable<bool> source, CanvasGroup group, int frameThrottle = 0)
            where TView : class,IView
        {
            if (!group) return view;
            Bind(source,x => group.interactable = x,frameThrottle).AddTo(view.ModelLifeTime);
            return view;
        }
        
        public static TView Bind<TView,TValue>(this TView view, IObservable<TValue> source, IReactiveCommand<TValue> command, int frameThrottle = 0)
            where TView : class,IView
        {
            Bind(source, command,frameThrottle).AddTo(view.ModelLifeTime);
            return view;
        }
        
        public static TView Bind<TView>(this TView view, IObservable<Sprite> source, Image image, int frameThrottle = 0)
            where TView : class,IView
        {
            return !image ? view : view.Bind(source.Where(x => x!=null), view.ModelLifeTime, x => image.sprite = x,frameThrottle);
        }
        
        public static TView Bind<TView>(this TView view, AssetReferenceT<Sprite> source, Image image, int frameThrottle = 0)
            where TView : class,IView
        {
            if (source.RuntimeKeyIsValid() == false) return view;
            return !image ? view : view.Bind(source.ToObservable(view.ModelLifeTime), image,frameThrottle);
        }

        public static TView Bind<TView,TValue>(this TView view, AssetReferenceT<TValue> source, Action<TValue> action, int frameThrottle = 0)
            where TView : class,IView
            where TValue : Object
        {
            if (source.RuntimeKeyIsValid() == false || action == null) return view;
            return view.Bind(source.ToObservable(view.ModelLifeTime), action,frameThrottle);
        }
        
        public static TView Bind<TView>(this TView view, IObservable<Sprite> source, Button button, int frameThrottle = 0)
            where TView : class,IView
        {
            if (!button || !button.image)
                return view;
            
            return view.Bind(source, view.ModelLifeTime, x => button.image.sprite = x,frameThrottle);
        }
        
        public static TView Bind<TView>(this TView view, IObservable<string> source, TextMeshProUGUI text, int frameThrottle = 0)
            where TView : class,IView
        {
            return view.Bind(source, view.ModelLifeTime, x => text.SetValue(x),frameThrottle);
        }

        public static TView Bind<TView>(this TView view, IObservable<string> source, TextMeshPro text, int frameThrottle = 0)
            where TView : class,IView
        {
            if (!text) return view;
            return view.Bind(source, view.ModelLifeTime, x => text.text = x,frameThrottle);
        }

        public static TView Bind<TView>(this TView view, IObservable<int> source, TextMeshPro text, int frameThrottle = 0)
            where TView : class,IView
        {
            return view.Bind(source, view.ModelLifeTime, x => text.text = x.ToStringFromCache(),frameThrottle);
        }
        
        public static TView Bind<TView,TValue>(this TView view, IObservable<TValue> source,Func<TValue,string> format, TextMeshProUGUI text, int frameThrottle = 0)
            where           TView : class,IView
        {
            var stringObservable = source.Select(format);
            return view.Bind(stringObservable, text,frameThrottle);
        }
        
        public static TView Bind<TView>(this TView view, IObservable<int> source, TextMeshProUGUI text, int frameThrottle = 0)
            where TView : class,IView
        {
            return view.Bind(source, view.ModelLifeTime, x => text.SetValue(x.ToStringFromCache()),frameThrottle);
        }
        
        #endregion


        public static TView Bind<TView,TValue>(this TView view, IObservable<TValue> source, IObserver<TValue> observer, int frameThrottle = 0)
            where TView : class,IView
        {
            return view.Bind(source, view.ModelLifeTime, observer.OnNext,frameThrottle);
        }
        
        public static async UniTask<TView> Bind<TView,TModel>(this TView view,TModel model, IView target) 
            where TView : class, IView
            where TModel : IViewModel
        {
            await target.Initialize(model);
            return view;
        }
        
        public static IDisposable Bind<T>(
            this IObservable<T> source, 
            Action<T> target, 
            int frameThrottle = 1)
        {
            if (source != null)
                return frameThrottle < 1
                    ? source.Subscribe(target)
                    : source.BatchPlayerTiming(frameThrottle, PlayerLoopTiming.LastPostLateUpdate).Subscribe(target);
            
            GameLog.LogWarning($"BindingExtension: NULL IObservable<T> detected with type {typeof(T).Name}");
            return Disposable.Empty;

        }
        public static IDisposable Bind<T>(
            this IObservable<T> source, 
            IReactiveCommand<T> target, 
            int frameThrottle = 1)
        {
            if(source == null)
                return Disposable.Empty;
            
            if (frameThrottle < 1)
            {
                return source.Where(x => target.CanExecute.Value).
                    Subscribe(x => target.Execute(x));
            }
            return source.
                BatchPlayerTiming(frameThrottle,PlayerLoopTiming.LastPostLateUpdate).
                Where(x => target.CanExecute.Value).
                Subscribe(x => target.Execute(x));
        }
        
        public static TSource Bind<TSource>(
            this TSource view,
            ILifeTime lifeTime, 
            Action target)
            where TSource : IView
        {
            lifeTime.AddCleanUpAction(target);
            return view;
        }
        
        public static TSource Bind<TSource>(
            this TSource view,
            ILifeTime lifeTime, 
            IDisposable target)
            where TSource : IView
        {
            lifeTime.AddDispose(target);
            return view;
        }
        
        public static TSource BindClose<TSource,TView>(
            this TSource view,
            TView source)
            where TSource : IView
            where TView : IView
        {
            source.CloseWith(view.ModelLifeTime);
            return view;
        }
        
        public static TSource BindUpdate<TSource>(
            this TSource view,
            Func<bool> predicate, 
            Action<long> target, 
            int frameThrottle = 1)
            where TSource : IView
        {
            Observable.EveryLateUpdate()
                .Where(x => predicate())
                .Bind(target,frameThrottle)
                .AddTo(view.ModelLifeTime);
            
            return view;
        }
        
        public static TSource Bind<TSource,T>(
            this TSource view,
            IObservable<T> source, 
            Action<T> target, 
            int frameThrottle = 1)
            where TSource : IView
        {
            source.Bind(target,frameThrottle).AddTo(view.ModelLifeTime);
            return view;
        }
        
        public static TSource BindClose<TSource,T>(
            this TSource view,
            IView target)
            where TSource : IView
            where T : IViewModel
        {
            target.CloseWith(view.ModelLifeTime);
            return view;
        }
        
        public static TSource Bind<TSource,T>(
            this TSource view,
            IObservable<T> source, 
            ViewBase target, 
            bool closeWith = false,
            int frameThrottle = 1)
            where TSource : ViewBase
            where T : IViewModel
        {
            if (closeWith) view.BindClose(target);
            
            return view.Bind(source, 
                x => target.Initialize(x, view.Layout).AttachExternalCancellation(view.ModelLifeTime.TokenSource).Forget(),
                frameThrottle);
        }
        
        
        public static TSource Bind<TSource,T>(
            this TSource view,
            IObservable<T> source, 
            Func<bool> predicate,
            Action<T> target, 
            int frameThrottle = 1)
            where TSource : IView
        {
            if(predicate != null && predicate())
                source.Bind(target,frameThrottle).AddTo(view.ModelLifeTime);
            return view;
        }
        
        public static TSource BindWhen<TSource,T>(
            this TSource view,
            IObservable<T> source, 
            Func<bool> predicate,
            Action<T> target, 
            int frameThrottle = 1)
            where TSource : IView
        {
            return Bind(view, source, predicate, target, frameThrottle);
        }
        
        public static TSource BindWhere<TSource,T>(
            this TSource view,
            Object indicator,
            IObservable<T> source, 
            Action<T> target, 
            int frameThrottle = 1)
            where TSource : IView
        {
            if(indicator)
                source.Bind(target,frameThrottle).AddTo(view.ModelLifeTime);
            return view;
        }
        
        public static IViewModel Bind<T>(
            this IViewModel model,
            IObservable<T> source, 
            Action<T> target, 
            int frameThrottle = 1)
        {
            source.Bind(target,frameThrottle).AddTo(model.LifeTime);
            return model;
        }

        public static TSource Bind<TSource, T>(this TSource view, IObservable<T> source, ILifeTime lifeTime, Action<T> target, int frameThrottle = 1) 
            where TSource : ILifeTimeContext
        {
            source.Bind(target, frameThrottle).AddTo(lifeTime);
            return view;
        }

    }
}
