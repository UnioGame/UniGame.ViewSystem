using System;

namespace UniGreenModules.UniGame.UiSystem.Runtime
{
    using System.Collections.Generic;
    using System.Linq;
    using Abstracts;
    using UniCore.Runtime.DataFlow;
    using UniCore.Runtime.ObjectPool.Runtime;
    using UniCore.Runtime.ObjectPool.Runtime.Extensions;
    using UniCore.Runtime.Rx.Extensions;
    using UniRx;
    using UniRx.Async;
    using UnityEngine;
    using Object = UnityEngine.Object;

    // Если этот класс предназначен только для выдачи вьюшек запросившему
    // то у него должен быть только метод Open
    // так как это фабрика, которая понятия не имеет что и кому отдает
    // соответственно происходяшие при вызове методов close<T> Hide<T> и прочее
    // не очевидно даже вызывающей стороне
    public class ViewStackController : IViewStackController
    {
        private List<IView> views = new List<IView>();

        private LifeTimeDefinition lifeTime = new LifeTimeDefinition();

        private ReactiveCommand visibilityChanged = new ReactiveCommand();
        
        #region constructor
        
        public ViewStackController()
        {
            visibilityChanged.
                ThrottleFrame(1).
                Subscribe(x => VisibilityStatusChanged()).
                AddTo(lifeTime.LifeTime);
        }
        
        #endregion
        
        #region public methods

        public void Dispose() => lifeTime.Terminate();

        public bool Contains(IView view) => views.Contains(view);

        /// <summary>
        /// add view to controller
        /// </summary>
        public void Add<TView>(TView view) where TView : Component, IView
        {
            //register view
            views.Add(view);

            //update view properties
            OnViewAdded(view);
        }

        public void Hide<T>() where T : Component, IView
        {
            FirstViewAction<T>(x => x.Hide());
        }

        public void HideAll<T>() where T : Component, IView
        {
            AllViewsAction<T>(x => true,y => y.Hide());
        }
        
        public void HideAll()
        {
            AllViewsAction<IView>(x => true,x => x.Hide());
        }

        public void Close<T>() where T : Component, IView
        {
            FirstViewAction<T>(x => x.Close());
        }

        public void CloseAll()
        {
            var buffer = ClassPool.Spawn<List<IView>>();
            buffer.AddRange(views);
            foreach (var view in buffer) {
                view.Close();
            }
            buffer.DespawnCollection();
        }
        
        public bool Close<T>(T view) where T : Component, IView
        {
            if (!view)
                return false;
            
            //custom user action before cleanup view
            OnBeforeClose(view);
            
            //remove view Object
            if (!views.Remove(view)) {
                return false;
            }
            
            //make sure to view is close already
            view.Close();

            OnViewClosed(view);
            
            return true;
        }
        
        #endregion

        protected virtual void OnViewClosed<TView>(TView view) where TView : Component, IView
        {
            //destroy view GameObject
            Object.Destroy(view.gameObject);
        }
        
        /// <summary>
        /// proceed visibility changes on target view
        /// </summary>
        private void VisibilityStatusChanged()
        {
            OnVisibilityStatusChanged();
        }


        private void AllViewsAction<TView>(Func<TView,bool> predicate,Action<TView> action) 
            where TView : IView
        {
            for (var i = 0; i < views.Count; i++)
            {
                var view = views[i];
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
            var view = views.FirstOrDefault(x => x is TView) as TView;
            if(view) 
                action(view);
        }

        /// <summary>
        /// Create View instance and place it in controller space
        /// </summary>
        /// <param name="asset">asset source</param>
        /// <typeparam name="TView"></typeparam>
        /// <returns>created view</returns>
        private TView Create<TView>(TView asset) where TView : Component, IView
        {
            //create instance of view
            var view = Object.
                Instantiate(asset.gameObject).
                GetComponent<TView>();
            
            //add view to loaded view items
            views.Add(view);

            //custom view method call
            OnViewAdded(view);
            
            return view;
        }
        
        protected virtual void OnBeforeClose<T>(T view) where T : Component, IView {}

        protected virtual void OnViewAdded<T>(T view) where T : Component, IView {}

        protected virtual void OnVisibilityStatusChanged() { }
    }
}
