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
        private ReactiveCollection<IView> _views = new ReactiveCollection<IView>();
        private LifeTimeDefinition _lifeTime = new LifeTimeDefinition();
        
        private RecycleReactiveProperty<IView> _onHiddenView = new RecycleReactiveProperty<IView>();
        private RecycleReactiveProperty<IView> _onShownView = new RecycleReactiveProperty<IView>();
        private RecycleReactiveProperty<IView> _onClsedView = new RecycleReactiveProperty<IView>();

        protected IReactiveCollection<IView> Views => _views;
        
        public Transform Layout { get; protected set; }
        
        #region constructor

        public ViewLayout()
        {
            _lifeTime.AddCleanUpAction(() => _onHiddenView.Release());
            _lifeTime.AddCleanUpAction(() => _onShownView.Release());
            _lifeTime.AddCleanUpAction(() => _onClsedView.Release());
        }
        
        #endregion


        public ILifeTime LifeTime => _lifeTime;
        
        #region IViewStatus

        public IObservable<IView> OnHidden => _onHiddenView;
        public IObservable<IView> OnShown => _onShownView;
        public IObservable<IView> OnClosed => _onClsedView;
        
        #endregion

        #region public methods

        public void Dispose() => _lifeTime.Terminate();

        public bool Contains(IView view) => _views.Contains(view);

        /// <summary>
        /// add view to controller
        /// </summary>
        public void Push<TView>(TView view) 
            where TView : Component, IView
        {
            if (_views.Contains(view)) {
                return;
            }
            //register new view
            AddView(view);
            //update view properties
            OnViewAdded(view);
        }

        public TView Get<TView>() where TView : Component, IView
        {
            return (TView)_views.FirstOrDefault(v => v is TView);
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
            FirstViewAction<T>(x => CloseSilent(x));
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

        public bool Close<T>(T view) where T : Component, IView
        {
            if (!view || !Contains(view))
                return false;
            
            //custom user action before cleanup view
            OnBeforeClose(view);
            
            view.Close();

            return true;
        }

        #endregion

        #region private methods

        public void AddView<TView>(TView view) 
            where TView : Component, IView
        {
            view.OnClosed.
                Do(x => _onClsedView.Value = x).
                Subscribe(x => Remove(x)).
                AddTo(view.LifeTime);
                
            view.OnShown.
                Subscribe(x => _onShownView.Value = x).
                AddTo(view.LifeTime);
                
            view.OnHidden.
                Subscribe(x => _onHiddenView.Value = x).
                AddTo(view.LifeTime);
                
            Add(view);
        }
        
        protected bool Remove(IView view)
        {
            if (view == null || !Contains(view))
                return false;
            
            //remove view Object
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
            where TView : Object, IView
        {
            var view = _views.FirstOrDefault(x => x is TView) as TView;
            if (view)
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

        protected virtual void OnBeforeClose<T>(T view) where T : Component, IView { }

        protected virtual void OnViewAdded<T>(T view) where T : Component, IView { }

        #endregion

    }
}
