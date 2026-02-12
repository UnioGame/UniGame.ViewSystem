namespace UniGame.UiSystem.Runtime
{
    using System;
    using System.Collections.Generic;
    using Core.Runtime;
    using R3;
    using UniGame.Runtime.DataFlow;
    using UniGame.Runtime.Rx;
    using UnityEngine;
    using ViewSystem.Runtime;

    public class VirtualLayout : IViewLayout
    {
        private LifeTime _lifeTime = new();
        private HashSet<IView> _views = new();
        private ReactiveValue<IView> _activeView = new();
        
        public void Dispose()
        {
            _lifeTime.Terminate();
            _views.Clear();
        }

        public ILifeTime LifeTime => _lifeTime;

        public Transform Layout => null;

        public Observable<IView> OnHidden => Observable.Empty<IView>();

        public Observable<IView> OnShown => Observable.Empty<IView>();

        public Observable<IView> OnBeginHide => Observable.Empty<IView>();

        public Observable<IView> OnBeginShow => Observable.Empty<IView>();

        public Observable<IView> OnClosed => Observable.Empty<IView>();

        public Observable<Type> OnIntent => Observable.Empty<Type>();

        public bool Contains(IView view)
        {
            return _views.Contains(view);
        }

        public TView Get<TView>() where TView : class, IView
        {
            foreach (var view in _views)
            {
                if(view is TView targetView) return  targetView;
            }
            return default;
        }

        public IView Get(Type viewType)
        {
            foreach (var view in _views)
            {
                if(view.GetType() == viewType) 
                    return  view;
            }

            return default;
        }

        public IEnumerable<TView> GetAll<TView>() where TView : class, IView
        {
            foreach (var view in _views)
            {
                if (view is TView targetView)
                    yield return targetView;
            }
        }

        public ReadOnlyReactiveProperty<IView> ActiveView => _activeView;

        public LayoutIntentResult Intent(string viewKey)
        {
            return new LayoutIntentResult()
            {
                stopPropagation = false,
            };  
        }

        public void Push(IView view)
        {
            if (Contains(view)) return;
            
            _views.Add(view);
            _activeView.Value = view;
            
            view.OnClosed()
                .Subscribe(this, static (x, y) => y._views.Remove(x))
                .AddTo(_lifeTime);
        }

        public void HideAll()
        {
        }

        public void CloseAll()
        {
        }

        public void ShowLast()
        {

        }

        public void Suspend()
        {
        }

        public void Resume()
        {
        }
    }
}