using System;

namespace UniGame.UiSystem.Runtime
{
    using System.Collections.Generic;
    using System.Linq;
    using Abstracts;
    using UniGreenModules.UniCore.Runtime.DataFlow;
    using UniGreenModules.UniCore.Runtime.ObjectPool.Runtime;
    using UniGreenModules.UniCore.Runtime.ObjectPool.Runtime.Extensions;
    using UniGreenModules.UniGame.UiSystem.Runtime.Abstracts;
    using UnityEngine;
    using Object = UnityEngine.Object;

    public class ViewStackController : IViewStackController
    {
        private List<IView> views = new List<IView>();

        private LifeTimeDefinition lifeTime = new LifeTimeDefinition();
        
        public IObservable<IView> StackTopChanged => throw new NotImplementedException();

        public Transform Layout { get; protected set; }


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

        public TView Get<TView>() where TView : Component, IView
        {
            return (TView)views.Find(v => v is TView);
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
            FirstViewAction<T>(x => x.Close());
        }

        public void CloseAll()
        {
            var buffer = ClassPool.Spawn<List<IView>>();
            buffer.AddRange(views);
            foreach (var view in buffer)
            {
                view.Close();
            }
            buffer.DespawnCollection();
        }

        public bool Remove<T>(T view) where T : Component, IView
        {
            if (!view)
                return false;

            //custom user action before cleanup view
            OnBeforeClose(view);

            //remove view Object
            return views.Remove(view);
        }

        #endregion

        private void AllViewsAction<TView>(Func<TView, bool> predicate, Action<TView> action)
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
            if (view)
                action(view);
        }

        protected virtual void OnBeforeClose<T>(T view) where T : Component, IView { }

        protected virtual void OnViewAdded<T>(T view) where T : Component, IView { }

    }
}
