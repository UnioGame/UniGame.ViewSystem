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
    using Core.Runtime.Extension;
    using UniModules.UniCore.Runtime.Utils;
    using UniModules.UniGame.Context.Runtime.Context;
    using ViewSystem.Runtime;
    using UniRx;
    using UnityEngine;
    using ViewSystem.Runtime.Binding;

    [Serializable]
    public class GameViewSystem : IGameViewSystem
    {
        public static readonly string ScreenType = ViewType.Screen.ToStringFromCache();
        public static readonly string WindowType = ViewType.Window.ToStringFromCache();
        public static readonly string OverlayType = ViewType.Overlay.ToStringFromCache();
        public static readonly string NoneType = ViewType.None.ToStringFromCache();
        
        public static IEnumerable<string> DefaultTypes {
            get
            {
                yield return ScreenType;
                yield return WindowType;
                yield return OverlayType;
                yield return NoneType;
            }
        }
        
        public static IGameViewSystem ViewSystem { get;private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Reset()
        {
            ViewSystem = null;
        }
        
        #region private fields

        private LifeTimeDefinition _lifeTimeDefinition = new LifeTimeDefinition();

        private IViewFactory _viewFactory;
        private IViewLayoutContainer _viewLayouts;
        private IViewFlowController _flowController;
        private IViewModelResolver _viewModelResolver;
        private IViewModelTypeMap _modelTypeMap;
        private IContext _defaultContext;
        private Subject<IView> _viewCreatedSubject;
        private IViewBinderProcessor _viewBinderProcessor;

        #endregion

        public GameViewSystem(
            IViewFactory viewFactory,
            IViewLayoutContainer viewLayouts,
            IViewFlowController flowController,
            IViewModelResolver viewModelResolver,
            IViewModelTypeMap modelTypeMap)
        {
            ViewSystem = this;
            
            _viewBinderProcessor = new ViewBinderProcessor();
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
            var view = await Create(viewModel, viewType.Name, skinTag, parent, viewName, stayWorld, ownerLifeTime);
            return view;
        }
        
        public async UniTask<IView> Create(
            IViewModel viewModel, 
            string viewType, 
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
            Transform parent = null, 
            string skinTag = "",
            string viewName = null,
            bool stayWorld = false,
            ILifeTime ownerLifeTime = null)
        {
            var view = await CreateView(viewType, skinTag, parent, viewName, stayWorld,ownerLifeTime);
            //fire view data
            _viewCreatedSubject.OnNext(view);
            
            return view;
        }
        
        public async UniTask<IView> Open(string viewType, string layout,
            string skinTag = "", 
            string viewName = null)
        {
            var view = await CreateView<IView>(viewType, layout, skinTag, viewName);
            
            view.Show();

            return view;
        }
        
        public async UniTask<IView> OpenWindow(string viewType, string skinTag = "", string viewName = null)
        {
            return await Open(viewType,WindowType,skinTag,viewName);
        }

        public async UniTask<IView> OpenScreen(string viewType, string skinTag = "", string viewName = null)
        {
            return await Open(viewType,ScreenType,skinTag,viewName);
        }

        public async UniTask<IView> OpenOverlay(string viewType, string skinTag = "", 
            string viewName = null)
        {
            return await Open(viewType,OverlayType,skinTag,viewName);
        }

        public async UniTask<IView> CreateWindow(string viewType, string skinTag = "", string viewName = null)
        {
            return await CreateView<IView>(viewType, WindowType, skinTag, viewName);
        }

        public async UniTask<IView> CreateScreen(string viewType, string skinTag = "", string viewName = null)
        {
            return await CreateView<IView>(viewType, ScreenType, skinTag, viewName);
        }

        public async UniTask<IView> CreateOverlay(string viewType, string skinTag = "", string viewName = null)
        {
            return await CreateView<IView>(viewType, OverlayType, skinTag, viewName);
        }
        
        public async UniTask<IView> OpenWindow(IViewModel viewModel, string viewType, string skinTag = "", string viewName = null)
        {
            var view = await OpenView(viewModel, viewType, WindowType, skinTag, viewName);
            return view;
        }

        public async UniTask<IView> OpenScreen(IViewModel viewModel, string viewType, string skinTag = "", string viewName = null)
        {
            var view = await OpenView(viewModel, viewType, ScreenType, skinTag, viewName);
            return view;
        }

        public async UniTask<IView> OpenOverlay(IViewModel viewModel, string viewType, string skinTag = "", string viewName = null)
        {
            var view = await OpenView(viewModel, viewType, OverlayType, skinTag, viewName);
            return view;
        }
        
        public async UniTask<IView> OpenView(IViewModel viewModel, string viewType,string layout, string skinTag = "", string viewName = null)
        {
            var view = await CreateViewInLayout(viewModel, viewType, layout, skinTag, viewName);
            view.Show();
            return view;
        }

        public async UniTask<IView> CreateWindow(IViewModel viewModel, string viewType, string skinTag = "", string viewName = null)
        {
            return await CreateViewInLayout<IView>(viewModel, viewType,
                WindowType, skinTag, viewName);
        }

        public async UniTask<IView> CreateScreen(IViewModel viewModel, string viewType, string skinTag = "", string viewName = null)
        {
            return await CreateViewInLayout<IView>(viewModel, viewType,
                ScreenType, skinTag, viewName);
        }

        public async UniTask<IView> CreateOverlay(IViewModel viewModel, string viewType, string skinTag = "", string viewName = null)
        {
            return await CreateViewInLayout<IView>(viewModel, viewType, OverlayType, skinTag, viewName);
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

        public bool HasLayout(string id)
        {
            return _viewLayouts.HasLayout(id);
        }

        public bool RegisterLayout(string id, IViewLayout layout)
        {
            return _viewLayouts.RegisterLayout(id, layout);
        }

        public bool RemoveLayout(string id)
        {
            return _viewLayouts.RemoveLayout(id);
        }

        public IViewLayout GetLayout(ViewType type) => _viewLayouts.GetLayout(type);
        
        public IViewLayout GetLayout(string id) => _viewLayouts.GetLayout(id);

        #endregion

        public void CloseAll()
        {
            _viewLayouts.CloseAll();
        }

        public void CloseAll(string id)
        {
            _viewLayouts.GetLayout(id)?.CloseAll();
        }

        public void CloseAll(ViewType viewType)
        {
            _viewLayouts.GetLayout(viewType)?.CloseAll();
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
            string viewType,
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
                    .AttachExternalCancellation(LifeTime.Token)
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
                GameLog.LogError($"ViewSystem: Try Create view with type: {viewType} | skin:{skinTag} | parent: {parent}: name: {viewName} FAILED");
                Destroy(view);
                return DummyView.Create();
            }
            
            await InitializeView(view, viewModel);
            
            return view;
        }

        public async UniTask<IView> CreateView(
            IViewModel viewModel,
            Type viewType,
            string skinTag = "",
            Transform parent = null,
            string viewName = null,
            bool stayWorld = false,
            ILifeTime ownerLifeTime = null)
        {
            var view = await CreateView(viewModel, viewType.Name, skinTag, parent, viewName, stayWorld, ownerLifeTime);
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
            var view = await CreateView(viewType.Name, skinTag, parent, viewName, stayWorld, ownerLifeTime);
            return view;
        }
        
        public async UniTask<IView> CreateView(
            string viewType,
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

        public async UniTask<IViewModel> CreateViewModel(string viewType)
        {
            var modelType = ModelTypeMap.GetViewModelType(viewType);
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

                
        public async UniTask<IView> Create(
            string viewType,
            ViewType layoutType,
            string skinTag = "",
            string viewName = null,
            ILifeTime ownerLifeTime = null)
        {
            return await Create(viewType, layoutType.ToStringFromCache(), skinTag, viewName, ownerLifeTime);
        }
        
        public async UniTask<IView> Create(
            string viewType,
            string layoutType,
            string skinTag = "",
            string viewName = null,
            ILifeTime ownerLifeTime = null)
        {
            var model = await CreateViewModel(viewType);
            
            var view = await CreateViewInLayout(model, viewType,
                layoutType, skinTag, viewName, ownerLifeTime);
            
            return view;
        }
        
        #endregion


        #region private methods

        private async UniTask<T> CreateView<T>(
            string viewType,
            string layoutType,
            string skinTag = "",
            string viewName = null,
            ILifeTime ownerLifeTime = null)
            where T : class, IView
        {
            var model = await CreateViewModel(viewType);
            
            var view = await CreateViewInLayout<T>(model, viewType,
                layoutType, skinTag, viewName, ownerLifeTime);
            
            return view;
        }
        
        private async UniTask<TView> CreateViewT<TView>(
            string viewType,
            ViewType layoutType,
            string skinTag = "",
            string viewName = null,
            ILifeTime ownerLifeTime = null)
            where TView : IView
        {
            var view = await Create(viewType,layoutType,skinTag,viewName,ownerLifeTime);
            
            if (view is TView resultView) return resultView;
            
            view.Close();
            return default;
        }
        
        private async UniTask<T> CreateView<T>(
            Type viewType,
            ViewType layoutType,
            string skinTag = "",
            string viewName = null,
            ILifeTime ownerLifeTime = null)
            where T : class, IView
        {
            var view = await Create(viewType.Name,layoutType,skinTag,viewName,ownerLifeTime);

            if (view is T resultView) return resultView;
            
            view.Close();
            return null;
        }

        private async UniTask<T> CreateView<T>(
            IViewModel viewModel,
            Type viewType,
            string layoutType,
            string skinTag = "",
            string viewName = null,
            ILifeTime ownerLifeTime = null)
            where T : class, IView
        {
            var view = await CreateViewInLayout<T>(viewModel,viewType.Name,layoutType,skinTag,viewName,ownerLifeTime);
            return view;
        }
        
        /// <summary>
        /// create view on target controller
        /// </summary>
        private async UniTask<T> CreateViewInLayout<T>(
            IViewModel viewModel,
            string viewType,
            string layoutType,
            string skinTag = "",
            string viewName = null,
            ILifeTime ownerLifeTime = null)
            where T : class, IView
        {
            var view = await CreateViewInLayout(viewModel, viewType, layoutType,
                skinTag, viewName, ownerLifeTime);
            return view as T;
        }
        
        /// <summary>
        /// create view on target controller
        /// </summary>
        private async UniTask<IView> CreateViewInLayout(
            IViewModel viewModel,
            string viewType,
            string layoutType,
            string skinTag = "",
            string viewName = null,
            ILifeTime ownerLifeTime = null)
        {
            var layout = _viewLayouts.GetLayout(layoutType);
            var intentResult = layout.Intent(viewType);
            if (intentResult.stopPropagation) return intentResult.view;
            
            var parent = layout?.Layout;
            
            var view = await CreateView(viewModel, viewType, skinTag, 
                parent, viewName, stayWorld:false, ownerLifeTime);

            layout?.Push(view);
            
            //fire view data
            _viewCreatedSubject.OnNext(view);

            return view;
        }

        /// <summary>
        /// Initialize View with model data
        /// </summary>
        public async UniTask<T> InitializeView<T>(T view, IViewModel viewModel)
            where T : IView
        {
            if(view == null || view.GameObject == null) 
                return view;
            
            if (view is ILayoutItem factoryView)
                factoryView.BindLayout(this);

            await view.Initialize(viewModel);

            //_viewBinderProcessor.Bind(view, viewModel);
            
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
            
            if(view.GameObject!=null)
                view.GameObject.SetActive(false);
            
            var target = asset.gameObject;
            target.DespawnNextFrame();
        }
        
        #endregion
    }
}