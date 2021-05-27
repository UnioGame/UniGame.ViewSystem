using System;

namespace UniGame.UiSystem.Runtime
{
    using System.Collections.Generic;
    using System.Linq;
    using Backgrounds.Abstract;
    using UniModules.UniCore.Runtime.DataFlow;
    using UniModules.UniCore.Runtime.ObjectPool.Runtime;
    using UniModules.UniCore.Runtime.ObjectPool.Runtime.Extensions;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniModules.UniGame.UISystem.Runtime;
    using UniModules.UniGame.UISystem.Runtime.Abstract;
    using UniRx;
    using UnityEngine;

    public class ViewLayout : IViewLayout
    {
        private readonly ReactiveCollection<IView> _views = new ReactiveCollection<IView>();
        private readonly LifeTimeDefinition _lifeTime = new LifeTimeDefinition();

        private readonly Subject<IView> _onViewHidden;
        private readonly Subject<IView> _onViewHiding;
        private readonly Subject<IView> _onViewShown;
        private readonly Subject<IView> _onViewShowing;
        private readonly Subject<IView> _onBecameVisible;
        private readonly Subject<IView> _onBecameHidden;
        private readonly Subject<IView> _onViewClosed;
        private readonly ReactiveProperty<ViewStatus> _viewStatus = new ReactiveProperty<ViewStatus>();

        protected IReadOnlyReactiveCollection<IView> Views => _views;

        public Transform Layout { get; protected set; }

        public ILifeTime LifeTime => _lifeTime;

        #region IViewStatus

        public IReadOnlyReactiveProperty<ViewStatus> Status => _viewStatus;
        public IObservable<IView> OnHidden => _onViewHidden;
        public IObservable<IView> OnShown => _onViewShown;
        public IObservable<IView> OnHiding => _onViewHiding;
        public IObservable<IView> OnShowing => _onViewShowing;
        public IObservable<IView> OnClosed => _onViewClosed;

        public IObservable<IView> OnBecameVisible => _onBecameVisible;
        public IObservable<IView> OnBecameHidden => _onBecameHidden;

        #endregion

        #region public methods

        public ViewLayout()
        {
            _onViewHidden = new Subject<IView>().AddTo(LifeTime);
            _onViewHiding = new Subject<IView>().AddTo(LifeTime);
            _onViewShown = new Subject<IView>().AddTo(LifeTime);
            _onViewShowing = new Subject<IView>().AddTo(LifeTime);
            _onBecameVisible = new Subject<IView>().AddTo(LifeTime);
            _onBecameHidden = new Subject<IView>().AddTo(LifeTime);
            _onViewClosed = new Subject<IView>().AddTo(LifeTime);
        }

        public void Dispose() => _lifeTime.Terminate();

        public bool Contains(IView view) => _views.Contains(view);

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
            return (TView)_views.LastOrDefault(v => v is TView);
        }

        /// <summary>
        /// select all view of target type into new container
        /// </summary>
        public List<TView> GetAll<TView>() where TView : class, IView
        {
            var list = this.Spawn<List<TView>>();
            foreach (var view in _views)
            {
                if (view is TView targetView)
                    list.Add(targetView);
            }
            return list;
        }

        public void ShowLast()
        {
            var lastView = Views.LastOrDefault(v => v is IView);
            if (lastView != null)
                lastView.Show();
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

        #endregion

        #region private methods


        protected void AddView<TView>(TView view)
            where TView : class, IView
        {
            var lifeTime = view.LifeTime;

            view.OnClosed
                .Do(x => Remove(x))
                .Do(_onViewClosed.OnNext)
                .Subscribe()
                .AddTo(lifeTime);

            view.OnShown
                .Subscribe(_onViewShown.OnNext)
                .AddTo(lifeTime);

            view.OnHidden
                .Subscribe(_onViewHidden.OnNext)
                .AddTo(lifeTime);

            view.Status
                .Subscribe(x => _viewStatus.SetValueAndForceNotify(x))
                .AddTo(lifeTime);

            view.OnHiding
                .Subscribe(_onViewHiding.OnNext)
                .AddTo(lifeTime);

            view.OnShowing
                .Subscribe(_onViewShowing.OnNext)
                .AddTo(lifeTime);

            view.IsVisible
                .Where(x => !x)
                .Subscribe(x => _onBecameHidden.OnNext(view))
                .AddTo(lifeTime);

            view.IsVisible
                .Where(x => x)
                .Subscribe(x => _onBecameVisible.OnNext(view))
                .AddTo(lifeTime);

            Add(view);
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
        protected virtual void OnBeforeClose(IView view) { }

        /// <summary>
        /// user defined action on new view added to layout
        /// </summary>
        protected virtual void OnViewAdded<T>(T view) where T : class, IView { }

        #endregion

    }
}
