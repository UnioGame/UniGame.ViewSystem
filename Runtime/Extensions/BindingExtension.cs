using System;
using Cysharp.Threading.Tasks;
using UniModules.UniGame.Rx.Runtime.Extensions;
using UniRx;

namespace UniModules.UniGame.UiSystem.Runtime.Extensions
{
    using Core.Runtime.DataFlow.Interfaces;
    using global::UniCore.Runtime.ProfilerTools;
    using UniCore.Runtime.Rx.Extensions;
    using UniModules.UniGame.Core.Runtime.Interfaces;

    public static class BindingExtension
    {
        public static IDisposable Bind<T>(
            this IObservable<T> source, 
            Action<T> target, 
            int frameThrottle = 1)
        {
            if (source == null) {
                GameLog.LogWarning($"BindingExtension: NULL IObservable<T> detected with type {typeof(T).Name}");
                return Disposable.Empty;
            }

            return frameThrottle < 1 ? 
                source.Subscribe(target) :
                source.BatchPlayerTiming(frameThrottle,PlayerLoopTiming.LastPostLateUpdate).
                Subscribe(target);
        }
        
        public static IDisposable Bind<T>(
            this IObservable<T> source, 
            IReactiveCommand<T> target, 
            int frameThrottle = 1)
        {
            return source.
                BatchPlayerTiming(frameThrottle,PlayerLoopTiming.LastPostLateUpdate).
                Where(x => target.CanExecute.Value).
                Subscribe(x => target.Execute(x));
        }
        
        public static TSource Bind<TSource,T>(
            this TSource view,
            IObservable<T> source, 
            Action<T> target, 
            int frameThrottle = 1)
            where TSource : ILifeTimeContext
        {
            source.Bind(target,frameThrottle).
                AddTo(view.LifeTime);
            return view;
        }
        

        public static TSource Bind<TSource, T>(this TSource view, IObservable<T> source, ILifeTime lifeTime, Action<T> target, int frameThrottle = 1) 
            where TSource : ILifeTimeContext
        {
            source
                .Bind(target, frameThrottle)
                .AddTo(lifeTime);
            
            return view;
        }

        public static TSource Bind<TSource,T>(
            this TSource view,
            IObservable<T> source, 
            IReactiveCommand<T> target, 
            int frameThrottle = 1)
            where TSource : ILifeTimeContext
        {
            source
                .Bind(target,frameThrottle)
                .AddTo(view.LifeTime);
            return view;
        }
    }
}
