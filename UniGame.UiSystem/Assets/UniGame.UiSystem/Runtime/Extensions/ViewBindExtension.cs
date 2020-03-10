using System;
using UniRx;

namespace UniGreenModules.UniGame.UiSystem.Runtime.Extensions
{
    using Abstracts;
    using UniCore.Runtime.Interfaces;
    using UniCore.Runtime.Rx.Extensions;

    public static class ViewBindExtension
    {
        public static IDisposable Bind<T>(
            this IObservable<T> source, 
            Action<T> target, 
            int frameThrottle = 1)
        {
            return source.
                ThrottleFrame(frameThrottle).
                Subscribe(target);
        }
        
        public static IDisposable Bind<T>(
            this IObservable<T> source, 
            IReactiveCommand<T> target, 
            int frameThrottle = 1)
        {
            return source.
                ThrottleFrame(frameThrottle).
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
        
        public static TSource Bind<TSource,T>(
            this TSource view,
            IObservable<T> source, 
            IReactiveCommand<T> target, 
            int frameThrottle = 1)
            where TSource : ILifeTimeContext
        {
            source.Bind(target,frameThrottle).
                AddTo(view.LifeTime);
            return view;
        }

    }
}
