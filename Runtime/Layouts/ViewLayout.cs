namespace UniGame.UiSystem.Runtime
{
    using System.Collections.Generic;
    using System.Linq;
    using UniGame.Runtime.DataFlow;
    using UniGame.Runtime.ObjectPool;
    using UniGame.Runtime.ObjectPool.Extensions;
    using Core.Runtime;
    using ViewSystem.Runtime;
     
    using UnityEngine;
    using System;
    using ObservableCollections;
    using R3;
    using UnityEngine.Pool;

    [Serializable]
    public class ViewLayout : IViewLayout
    {
        private readonly ObservableList<IView> _views    = new();
        private readonly LifeTime        _lifeTime = new();

        private readonly Subject<IView>                _onViewHidden;
        private readonly Subject<IView>                _onViewBeginHide;
        private readonly Subject<IView>                _onViewShown;
        private readonly Subject<IView>                _onViewBeginShow;
        private readonly Subject<IView>                _onViewClosed;
        private readonly Subject<Type>                 _onViewIntent;
        private readonly ReactiveProperty<IView> _activeView;

        protected IReadOnlyList<IView> Views => _views;

        public Transform Layout { get; protected set; }

        public ILifeTime LifeTime => _lifeTime;
        
        #region IViewStatus

        public Observable<IView> OnHidden    => _onViewHidden;
        public Observable<IView> OnShown     => _onViewShown;
        public Observable<IView> OnBeginHide => _onViewBeginHide;
        public Observable<IView> OnBeginShow => _onViewBeginShow;
        public Observable<IView> OnClosed    => _onViewClosed;
        public Observable<Type>  OnIntent    => _onViewIntent;
        public ReadOnlyReactiveProperty<IView> ActiveView => _activeView;

        #endregion

        #region public methods

        public ViewLayout()
        {
            _onViewHidden    = new Subject<IView>().AddTo(LifeTime);
            _onViewBeginHide = new Subject<IView>().AddTo(LifeTime);
            _onViewShown     = new Subject<IView>().AddTo(LifeTime);
            _onViewBeginShow = new Subject<IView>().AddTo(LifeTime);
            _onViewClosed    = new Subject<IView>().AddTo(LifeTime);
            _onViewIntent    = new Subject<Type>().AddTo(LifeTime);
            _activeView = new ReactiveProperty<IView>(null).AddTo(LifeTime);

            _views.ObserveAdd().Do(e => { _activeView.Value = e.Value; }).Subscribe().AddTo(LifeTime);
            _views.ObserveRemove().Do(e =>
            {
                if (_views.Count > 0)
                    _activeView.Value = Views.Last();
                else
                    _activeView.Value = null;
            }).Subscribe().AddTo(LifeTime);
            _views.ObserveReset().Do(_ => _activeView.Value = null).Subscribe().AddTo(LifeTime);
        }

        public void Dispose()
        {
            _lifeTime.Terminate();

            var viewList = ListPool<IView>.Get();
            
            foreach (var view in _views)
            {
                if(view != null && view.GameObject!=null) 
                    viewList.Add(view);
            }

            foreach (var view in viewList)
                view?.Destroy();
            
            viewList.Despawn();
            _views.Clear();
        }

        public bool Contains(IView view) => _views.Contains(view);

        public void ApplyIntent<T>() where T : IView
        {
            _onViewIntent.OnNext(typeof(T));
        }

        public virtual LayoutIntentResult Intent(string viewKey) => new()
        {
            stopPropagation = false,
        };

        /// <summary>
        /// add view to controller
        /// </summary>
        public void Push(IView view)
        {
            if (_views.Contains(view))
            {
                return;
            }

            AddView(view);

            //custom user action on new view
            OnViewAdded(view);
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

        /// <summary>
        /// select all view of target type into new container
        /// </summary>
        public IEnumerable<TView> GetAll<TView>() where TView : class, IView
        {
            foreach (var view in _views)
            {
                if (view is TView targetView)
                    yield return targetView;
            }
        }

        public void ShowLast()
        {
            var lastView = Views.LastOrDefault(v => v != null);
            lastView?.Show();
        }

        public void Hide<T>() where T : Component, IView
        {
            FirstViewAction<T>(x => x.Hide());
        }

        public void HideAll<T>() where T : Component, IView
        {
            AllViewsAction<T>(x => true, y => y.Hide());
        }

        public void HideAll()
        {
            AllViewsAction<IView>(x => true, x => x.Hide());
        }

        public void Close<T>() where T : Component, IView
        {
            FirstViewAction<T>(x => Close(x));
        }

        public void CloseAll()
        {
            var buffer = ClassPool.Spawn<List<IView>>();
            buffer.AddRange(_views);

            _views.Clear();
            foreach (var view in buffer)
            {
                if (view != null)
                    view.Close();
            }

            buffer.Despawn();
        }

        public bool Close(IView view)
        {
            if (view == null || !Contains(view))
                return false;

            //custom user action before cleanup view
            OnBeforeClose(view);

            view.Close();

            return true;
        }

        public virtual void Suspend()
        {
        }

        public virtual void Resume()
        {
        }

        #endregion

        #region private methods

        protected void AddView<TView>(TView view)
            where TView : class, IView
        {
            view.StatusChanged
                .Subscribe(this,static (x,y) => y.ViewStatusChanged(x, x.Status.CurrentValue))
                .AddTo(LifeTime);
            
            Add(view);
        }

        protected void ViewStatusChanged<TView>(TView view, ViewStatus status)
            where TView : class, IView
        {
            switch (status)
            {
                case ViewStatus.Hidden:
                    _onViewHidden.OnNext(view);
                    break;
                case ViewStatus.Shown:
                    _onViewShown.OnNext(view);
                    break;
                case ViewStatus.Closed:
                    Remove(view);
                    _onViewClosed.OnNext(view);
                    break;
                case ViewStatus.Showing:
                    _onViewBeginShow.OnNext(view);
                    break;
                case ViewStatus.Hiding:
                    _onViewBeginHide.OnNext(view);
                    break;
            }
        }

        protected virtual bool IsAnyViewActive()
        {
            return _activeView.CurrentValue != null;
        }

        protected bool Remove(IView view)
        {
            if (view == null || !Contains(view))
                return false;
            return _views.Remove(view);
        }

        protected bool Add(IView view)
        {
            if (Contains(view)) return false;
            _views.Add(view);
            return true;
        }

        private void AllViewsAction<TView>(Func<TView, bool> predicate, Action<TView> action)
            where TView : IView
        {
            for (var i = 0; i < _views.Count; i++)
            {
                var view = _views[i];
                if (view is TView targetView &&
                    predicate(targetView))
                {
                    action(targetView);
                }
            }
        }

        private void FirstViewAction<TView>(Action<TView> action)
            where TView : class, IView
        {
            if (_views.FirstOrDefault(x => x is TView) is TView view)
                action(view);
        }

        /// <summary>
        /// close view with removing from collection
        /// </summary>
        /// <param name="view"></param>
        protected void CloseSilent(IView view)
        {
            if (Remove(view))
                view.Close();
        }

        /// <summary>
        /// user defined actions triggered before any view closed
        /// </summary>
        protected virtual void OnBeforeClose(IView view)
        {
        }

        /// <summary>
        /// user defined action on new view added to layout
        /// </summary>
        protected virtual void OnViewAdded<T>(T view) where T : class, IView
        {
        }

        #endregion
    }
}