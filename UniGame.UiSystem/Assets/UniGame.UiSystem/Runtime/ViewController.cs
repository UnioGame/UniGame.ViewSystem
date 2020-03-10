namespace UniGreenModules.UniGame.UiSystem.Runtime
{
    using System.Collections.Generic;
    using System.Linq;
    using Abstracts;
    using Core.Runtime.Rx;
    using Taktika.Addressables.Reactive;
    using UniCore.Runtime.DataFlow;
    using UniCore.Runtime.ObjectPool.Runtime;
    using UniCore.Runtime.ObjectPool.Runtime.Extensions;
    using UniCore.Runtime.ProfilerTools;
    using UniCore.Runtime.Rx.Extensions;
    using UniRx;
    using UniRx.Async;
    using UnityEngine;
    using Object = UnityEngine.Object;

    public class ViewController : IViewController
    {
        private readonly IViewFactory viewFactory;
        private readonly IViewElementFactory elementFactory;

        private List<IView> views = new List<IView>();

        private LifeTimeDefinition lifeTime = new LifeTimeDefinition();

        private ReactiveCommand visibilityChanged = new ReactiveCommand();
        
        #region constructor
        
        public ViewController(IViewFactory viewFactory,IViewElementFactory elementFactory)
        {
            this.viewFactory = viewFactory;
            this.elementFactory = elementFactory;

            visibilityChanged.
                ThrottleFrame(1).
                Subscribe(x => VisibilityStatusChanged()).
                AddTo(lifeTime.LifeTime);
        }
        
        #endregion
        
        #region public methods

        public void Dispose() => lifeTime.Terminate();

        /// <summary>
        /// Open new view element
        /// </summary>
        /// <param name="viewModel">target element model data</param>
        /// <param name="skinTag">target element skin</param>
        /// <returns>created view element</returns>
        public async UniTask<T> Open<T>(IViewModel viewModel,string skinTag = "") 
            where T : Component, IView
        {
            
            var view = await viewFactory.Create<T>(skinTag);
            
            //register view
            views.Add(view);
            
            //initialize view with model data
            InitializeView(view, viewModel);

            //update view properties
            OnViewOpen(view);
            
            return view;

        }

        public bool Hide<T>() where T : Component, IView
        {
            var view = Select<T>();
            view?.Hide();
            return view != null;
        }

        public void HideAll<T>() where T : Component, IView
        {
            foreach (var view in views) {
                if(view is T targetView)
                    targetView.Hide();
            }
        }
        
        public void HideAll()
        {
            foreach (var view in views) {
                view.Hide();
            }
        }

        public bool Close<T>() where T : Component, IView
        {
            var view = Select<T>();
            view?.Close();
            return view!=null;
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
        
        private T InitializeView<T>(T view, IViewModel viewModel)
            where T : Component, IView
        {
                        
            //initialize view with model data
            view.Initialize(viewModel,elementFactory);
            
            //bind disposable to View lifeTime
            var viewLifeTime = view.LifeTime;
            
            viewLifeTime.AddCleanUpAction(() => Close(view));

            //handle all view visibility changes
            view.IsActive.
                Subscribe(x => visibilityChanged.Execute()).
                AddTo(viewLifeTime);
            
            //update view active state by base view model data
            viewModel.IsActive.
                Where(x => x).
                Subscribe(x => view.Show()).
                AddTo(view.LifeTime);
            
            viewModel.IsActive.
                Where(x => !x).
                Subscribe(x => view.Hide()).
                AddTo(view.LifeTime);

            return view;
        }
        
   
        private TView Select<TView>() where TView : Object, IView
        {
            return views.FirstOrDefault(x => typeof(TView) == x.GetType()) as TView;
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
            OnViewOpen(view);
            
            return view;
        }
        
        protected virtual void OnBeforeClose<T>(T view) where T : Component, IView {}

        protected virtual void OnViewOpen<T>(T view) where T : Component, IView {}

        protected virtual void OnVisibilityStatusChanged() { }
    }
}
