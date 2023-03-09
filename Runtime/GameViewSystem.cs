using UniGame.Runtime.ObjectPool.Extensions;
using UniGame.ViewSystem.Runtime;
using UniGame.ViewSystem.Runtime.Abstract;
using UnityEngine.Scripting;

[assembly: AlwaysLinkAssembly]
namespace UniGame.UiSystem.Runtime
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using UniCore.Runtime.ProfilerTools;
    using UniModules.UniCore.Runtime.DataFlow;
    using UniModules.UniGame.UiSystem.Runtime;
    using Core.Runtime;
    using UniModules.UniGame.Context.Runtime.Context;
    using ViewSystem.Runtime;
    using UniRx;
    using UnityEngine;

    [Serializable]
    public class GameViewSystem : IGameViewSystem
    {
        #region private fields

        private LifeTimeDefinition _lifeTimeDefinition = new LifeTimeDefinition();

        private readonly IViewFactory _viewFactory;
        private readonly IViewLayoutContainer _viewLayouts;
        private readonly IViewFlowController _flowController;
        private readonly IViewModelResolver _viewModelResolver;
        private readonly IViewModelTypeMap _modelTypeMap;
        private readonly IContext _defaultContext;
        private readonly Subject<IView> _viewCreatedSubject;

        #endregion

        public GameViewSystem(
            IViewFactory viewFactory,
            IViewLayoutContainer viewLayouts,
            IViewFlowController flowController,
            IViewModelResolver viewModelResolver,
            IViewModelTypeMap modelTypeMap)
        {
            _viewCreatedSubject = new Subject<IView>().AddTo(LifeTime);
            _defaultContext = new EntityContext();

            _viewFactory = viewFactory;
            _viewLayouts = viewLayouts;
            _flowController = flowController;
            _viewModelResolver = viewModelResolver;
            _modelTypeMap = modelTypeMap;

            _flowController.Activate(_viewLayouts);
        }
        
        public IViewModelTypeMap ModelTypeMap => _modelTypeMap;

        public ILifeTime LifeTime => _lifeTimeDefinition.LifeTime;

        /// <summary>
        /// reactive views stream
        /// </summary>
        public IObservable<IView> ViewCreated => _viewCreatedSubject;


        #region public methods

        /// <summary>
        /// terminate game view system lifetime
        /// </summary>
        public void Dispose() => _lifeTimeDefinition.Terminate();

        #region IViewModelProvider api

        public bool IsValid(Type modelType) => _viewModelResolver.IsValid(modelType);

        public async UniTask<IViewModel> CreateViewModel(IContext context, Type modelType)
        {
            if (_viewModelResolver == null)
                return null;
            
            return await _viewModelResolver.CreateViewModel(context,modelType);
        }
        
        #endregion
        
        #region ui system api

        public IObservable<TView> ObserveView<TView>()
            where TView : class, IView
        {
            var observable = ViewCreated.OfType<IView, TView>();
            var view       = Get<TView>();
            observable = view == null
                ? observable
                : observable.Merge(Observable.Return(view));
            return observable;
        }

        public async UniTask<IView> Create(IViewModel viewModel, 
            Type viewType, 
            string skinTag = "",
            Transform parent = null, 
            string viewName = null,
            bool stayWorld = false,
            ILifeTime ownerLifeTime = null)
        {
            var view = await CreateView(viewModel, viewType, skinTag, parent, viewName, stayWorld,ownerLifeTime);
            //fire view data
            _viewCreatedSubject.OnNext(view);
            return view;
        }

        public async UniTask<IView> Create(
            Type viewType, 
            string skinTag = "",
            Transform parent = null, 
            string viewName = null,
            bool stayWorld = false,
            ILifeTime ownerLifeTime = null)
        {
            var view = await CreateView(viewType, skinTag, parent, viewName, stayWorld,ownerLifeTime);
            //fire view data
            _viewCreatedSubject.OnNext(view);
            
            return view;
        }
        
        public async UniTask<IView> OpenWindow(Type viewType, 
            string skinTag = "",
            string viewName = null)
        {
            var view = await CreateViewAndPushToLayout<IView>(viewType, 
                ViewType.Window, skinTag, viewName);
            
            view.Show();

            return view;
        }

        public async UniTask<IView> OpenScreen(Type viewType, string skinTag = "", string viewName = null)
        {
            var view = await CreateViewAndPushToLayout<IView>(viewType, ViewType.Screen, 
                skinTag, viewName);
            view.Show();

            return view;
        }

        public async UniTask<IView> OpenOverlay(Type viewType, string skinTag = "", 
            string viewName = null)
        {
            var view = await CreateViewAndPushToLayout<IView>(viewType, ViewType.Overlay, skinTag, viewName);
            view.Show();

            return view;
        }

        public async UniTask<IView> CreateWindow(Type viewType, string skinTag = "", string viewName = null)
        {
            return await CreateViewAndPushToLayout<IView>(viewType, 
                ViewType.Window, skinTag, viewName);
        }

        public async UniTask<IView> CreateScreen(Type viewType, string skinTag = "", string viewName = null)
        {
            return await CreateViewAndPushToLayout<IView>(viewType, ViewType.Screen, skinTag, viewName);
        }

        public async UniTask<IView> CreateOverlay(Type viewType, string skinTag = "", string viewName = null)
        {
            return await CreateViewAndPushToLayout<IView>(viewType, ViewType.Overlay, skinTag, viewName);
        }
        
        public async UniTask<IView> OpenWindow(IViewModel viewModel, Type viewType, 
            string skinTag = "",
            string viewName = null)
        {
            var view = await CreateViewAndPushToLayout<IView>(viewModel, viewType, ViewType.Window, skinTag, viewName);
            view.Show();

            return view;
        }

        public async UniTask<IView> OpenScreen(IViewModel viewModel, Type viewType, string skinTag = "", string viewName = null)
        {
            var view = await CreateViewAndPushToLayout<IView>(viewModel, viewType, ViewType.Screen, skinTag, viewName);
            view.Show();

            return view;
        }

        public async UniTask<IView> OpenOverlay(IViewModel viewModel, Type viewType, string skinTag = "", string viewName = null)
        {
            var view = await CreateViewAndPushToLayout<IView>(viewModel, viewType, ViewType.Overlay, skinTag, viewName);
            view.Show();

            return view;
        }

        public async UniTask<IView> CreateWindow(IViewModel viewModel, Type viewType, string skinTag = "", string viewName = null)
        {
            return await CreateViewAndPushToLayout<IView>(viewModel, viewType, ViewType.Window, skinTag, viewName);
        }

        public async UniTask<IView> CreateScreen(IViewModel viewModel, Type viewType, string skinTag = "", string viewName = null)
        {
            return await CreateViewAndPushToLayout<IView>(viewModel, viewType, ViewType.Screen, skinTag, viewName);
        }

        public async UniTask<IView> CreateOverlay(IViewModel viewModel, Type viewType, string skinTag = "", string viewName = null)
        {
            return await CreateViewAndPushToLayout<IView>(viewModel, viewType, ViewType.Overlay, skinTag, viewName);
        }

        public T Get<T>() where T : class, IView
        {
            foreach (var controller in _viewLayouts.Controllers)
            {
                var v = controller.Get<T>();
                if (v != null)
                    return v;
            }

            return null;
        }

        #endregion

        #region layout container api

        public IReadOnlyViewLayout this[ViewType type] => _viewLayouts[type];

        public IEnumerable<IViewLayout> Controllers => _viewLayouts.Controllers;

        public IViewLayout GetLayout(ViewType type) => _viewLayouts.GetLayout(type);
        public IViewLayout GetLayout(string id) => _viewLayouts.GetLayout(id);

        #endregion

        public void CloseAll()
        {
            _viewLayouts.GetLayout(ViewType.Screen)?.CloseAll();
            _viewLayouts.GetLayout(ViewType.Window)?.CloseAll();
        }

        /// <summary>
        /// create new view element
        /// </summary>
        /// <param name="viewModel">target element model data</param>
        /// <param name="viewType">view type filter</param>
        /// <param name="skinTag">target element skin</param>
        /// <param name="parent">view parent</param>
        /// <param name="viewName"></param>
        /// <param name="stayWorld"></param>
        /// <param name="ownerLifeTime"></param>
        /// <returns>created view element</returns>
        public async UniTask<IView> CreateView(
            IViewModel viewModel,
            Type viewType,
            string skinTag = "",
            Transform parent = null,
            string viewName = null,
            bool stayWorld = false,
            ILifeTime ownerLifeTime = null)
        {
            var lifeTime = ownerLifeTime ?? LifeTime;
            if (lifeTime.IsTerminated)
                return DummyView.Create();
            
            var   failed = false;
            IView view = null;
            
            try
            {
                var viewResult = await _viewFactory
                    .Create(viewType, skinTag, parent, viewName, stayWorld)
                    .AttachExternalCancellation(LifeTime.TokenSource)
                    .SuppressCancellationThrow();

                failed = viewResult.IsCanceled;
                view = viewResult.Result;
            }
            catch (Exception e)
            {
                GameLog.LogError(e);
                failed = true;
            }
            
            if (failed)
            {
                GameLog.LogError($"ViewSystem: Try Create view with type: {viewType.Name} | skin:{skinTag} | parent: {parent}: name: {viewName} FAILED");
                Destroy(view);
                return DummyView.Create();
            }
            
            await InitializeView(view, viewModel);
            return view;
        }

        public async UniTask<IView> CreateView(
            Type viewType,
            string skinTag = "",
            Transform parent = null,
            string viewName = null,
            bool stayWorld = false,
            ILifeTime ownerLifeTime = null)
        {
            var model = await CreateViewModel(viewType);
            var view = await Create(model,viewType, skinTag, parent, viewName, stayWorld, ownerLifeTime);
            return view;
        }

        public async UniTask<IViewModel> CreateViewModel(Type viewType)
        {
            var modelType = ModelTypeMap.GetViewModelTypeByView(viewType);
            var model = await CreateViewModel(_defaultContext, modelType);
            return model;
        }
        
        /// <summary>
        /// create new view element
        /// </summary>
        /// <param name="viewModel">target element model data</param>
        /// <param name="skinTag">target element skin</param>
        /// <param name="parent">view parent</param>
        /// <param name="stayWorld"></param>
        /// <param name="ownerLifeTime"></param>
        /// <returns>created view element</returns>
        public async UniTask<T> CreateView<T>(
            IViewModel viewModel,
            string skinTag = "",
            Transform parent = null,
            bool stayWorld = false,
            ILifeTime ownerLifeTime = null)
            where T : class, IView
        {
            var view = await CreateView(viewModel, typeof(T), skinTag, parent, string.Empty, stayWorld,ownerLifeTime) as T;

            //fire view data
            _viewCreatedSubject.OnNext(view);
            return view;
        }

        #endregion


        #region private methods

        private async UniTask<T> CreateViewAndPushToLayout<T>(
            Type viewType,
            ViewType layoutType,
            string skinTag = "",
            string viewName = null,
            ILifeTime ownerLifeTime = null)
            where T : class, IView
        {
            var model = await CreateViewModel(viewType);
            
            var view = await CreateViewAndPushToLayout<T>(model, viewType,
                layoutType, skinTag, viewName, ownerLifeTime);
            
            return view;
        }
        
        /// <summary>
        /// create view on target controller
        /// </summary>
        private async UniTask<T> CreateViewAndPushToLayout<T>(
            IViewModel viewModel,
            Type viewType,
            ViewType layoutType,
            string skinTag = "",
            string viewName = null,
            ILifeTime ownerLifeTime = null)
            where T : class, IView
        {
            var layout = _viewLayouts.GetLayout(layoutType);
            var parent = layout?.Layout;
            
            var view = await CreateView(viewModel, 
                viewType, 
                skinTag, 
                parent,
                viewName,
                stayWorld:false,
                ownerLifeTime);

            layout?.Push(view);
            
            //fire view data
            _viewCreatedSubject.OnNext(view);

            return view as T;
        }

        /// <summary>
        /// Initialize View with model data
        /// </summary>
        public async UniTask<T> InitializeView<T>(T view, IViewModel viewModel)
            where T : IView
        {
            if (view is ILayoutItem factoryView)
                factoryView.BindLayout(this);

            await view.Initialize(viewModel);

            //destroy view when lifetime terminated
            var viewLifeTime = view.ViewLifeTime;
            viewLifeTime.AddCleanUpAction(() => Destroy(view));

            return view;
        }

        private void Destroy(IView view)
        {
            if (view == null) return;
            
            view.Destroy();
            var asset = view as Component;
            if (asset == null) return;
            
            var target = asset.gameObject;
            target.DespawnAsset();
        }

        #endregion
    }
}