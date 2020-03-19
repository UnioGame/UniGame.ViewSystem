using System;

namespace UniGame.UiSystem.Runtime
{
    using System.Collections.Generic;
    using System.Linq;
    using Abstracts;
    using UniGreenModules.UniCore.Runtime.DataFlow;
    using UniGreenModules.UniCore.Runtime.DataFlow.Interfaces;
    using UniGreenModules.UniCore.Runtime.ObjectPool.Runtime;
    using UniGreenModules.UniCore.Runtime.ObjectPool.Runtime.Extensions;
    using UniGreenModules.UniCore.Runtime.Rx.Extensions;
    using UniGreenModules.UniGame.Core.Runtime.Rx;
    using UniRx;
    using UnityEngine;
    using Object = UnityEngine.Object;
    
    public class ViewLayout : IViewLayout
    {
        protected readonly ReactiveCollection<IView> _views = new ReactiveCollection<IView>();
        private readonly LifeTimeDefinition _lifeTime = new LifeTimeDefinition();

        private Subject<IView> _onViewHidden = new Subject<IView>();
        private Subject<IView> _onViewShown = new Subject<IView>();
        private Subject<IView> _onViewClosed = new Subject<IView>();
        
        protected IReadOnlyReactiveCollection<IView> Views => _views;
        
        public Transform Layout { get; protected set; }

        public ILifeTime LifeTime => _lifeTime;
        
        #region IViewStatus

        public IObservable<IView> OnHidden => _onViewHidden;
        public IObservable<IView> OnShown => _onViewShown;
        public IObservable<IView> OnClosed => _onViewClosed;
        
        #endregion

        #region public methods

        public void Dispose() => _lifeTime.Terminate();

        public bool Contains(IView view) => _views.Contains(view);

        /// <summary>
        /// add view to controller
        /// </summary>
        public void Push<TView>(TView view) 
            where TView :class, IView
        {
            if (_views.Contains(view)) {
                return;
            }
            
            AddView(view);
            
            //custom user action on new view
            OnViewAdded(view);
        }

        public TView Get<TView>() where TView :class, IView
        {
            return (TView)_views.LastOrDefault(v => v is TView);
        }
        
        /// <summary>
        /// select all view of target type into new container
        /// </summary>
        public List<TView> GetAll<TView>() where TView :class, IView
        {
            var list = this.Spawn<List<TView>>();
            foreach (var view in _views) {
                if(view is TView targetView)
                    list.Add(targetView);
            }
            return list;
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
            where TView :class, IView
        {
            view.OnClosed.
                Do(x => _onViewClosed.OnNext(x)).
                Subscribe(x => Remove(x)).
                AddTo(view.LifeTime);
                
            view.OnShown.
                Subscribe(x => _onViewShown.OnNext(x)).
                AddTo(view.LifeTime);
                
            view.OnHidden.
                Subscribe(x => _onViewHidden.OnNext(x)).
                AddTo(view.LifeTime);
                
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
                if ((view is TView targetView) &&
                    predicate(targetView))
                {
                    action(targetView);
                }
            }
        }

        private void FirstViewAction<TView>(Action<TView> action)
            where TView : class,  IView
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
            if(Remove(view))
                view.Close();
        }

        /// <summary>
        /// user defined actions triggered before any view closed
        /// </summary>
        protected virtual void OnBeforeClose(IView view) { }

        /// <summary>
        /// user defined action on new view added to layout
        /// </summary>
        protected virtual void OnViewAdded<T>(T view) where T :class,  IView { }

        #endregion

    }
}
